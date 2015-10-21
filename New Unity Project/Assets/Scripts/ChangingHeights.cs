using UnityEngine;
using System.Collections;
using Leap;
using System;
using UnityEngine.UI;

public delegate void LeapEventDelegate();

public class ChangingHeights: MonoBehaviour {
    public enum Modes { Editor, Interactive, Playing };
    public Terrain terrain;
    public TerrainColorChanger terrainColorChanger;
    public TerrainAccessories terrainAccessories;
    InteractionBox iBox;
    private FrameListener frameListener;
    public HandController rightHandController;
    public Slider terrainSelectionSizeSlider;
    public Slider terrainIncrementSlider;
    public Text modeText;
    public Text treesLeft;
    public Text groundLeft;
    private float actualGroundCost;
    public Vector3 positionOfThumb = new Vector3(0,0,0);
    int goUp = 1;
    int hmWidth; // heightmap width 
    int hmHeight; // heightmap height
    bool pinchActive = false;
    bool tapActive = false;
    bool mouseIsDown = false;
    float grabDistance = 10;
    int speed = 1;
    int counter = 0;
    Vector3 mouse;
    int posXInTerrain; // position of the mouse in terrain width (x axis) 
    int posYInTerrain; // position of the mouse in terrain height (z axis)
    int size = 10; // the diameter of terrain portion that will raise  
    float heightChange = 0;
    private Frame currentFrame;
    private long currentFrameId = -1;
    public LeapEventDelegate eventDelegate;
    private int thumbID;
    private int indexFingerID;
    private int rightHandID;
    private Vector3 positionOfTap;
    private TreeInstance tree;
    private int terrainLayerMask;
    private int middleOfCircle;

    private Modes mode;
    public Modes Mode {
        get { return mode; }
        set {
            mode = value;
            modeText.text = "Mode: " + value.ToString();
        }
    }

    private int treesRemaining;
    public int TreesRemaining {
        get {
            return treesRemaining;
        }
        set {
            treesRemaining = value;
            treesLeft.text = "Trees left: " + value.ToString();
        }
    }

    private int groundRemaining;
    public int GroundRemaining {
        get {
            return groundRemaining;
        }
        set {
            groundRemaining = value;
            groundLeft.text = "Ground left: " + value.ToString();
        }
    }

    public Controller controller { get; private set; }
    public static ChangingHeights Instance { get; private set; }

    FrameListener.LeapEventDelegate delegateReference;    

    void Awake() {
        controller = new Controller();
        Instance = this;
       // frameListener = new FrameListener();
       // controller.AddListener(frameListener);
    }
    void Start() {
     //  delegateReference = new FrameListener.LeapEventDelegate(parseFrame);
     //  frameListener.eventDelegate += delegateReference;
        controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
        terrain = Terrain.activeTerrain;
        hmWidth = terrain.terrainData.heightmapWidth;
        hmHeight = terrain.terrainData.heightmapHeight;
        terrainSelectionSizeSlider.value = size;
        terrainSelectionSizeSlider.onValueChanged.AddListener(resizeTerrainSelection);
        terrainIncrementSlider.value = speed;
        terrainIncrementSlider.onValueChanged.AddListener(ChangeTerrainIncrement);
        Mode = Modes.Editor;
        TreesRemaining = 5;
        GroundRemaining = 10000;
        actualGroundCost = -1; //as in not yet counted
        terrainLayerMask = LayerMask.NameToLayer("Terrain");
        terrainLayerMask = ~terrainLayerMask;
    }

    void OnDestroy() {
        //frameListener.eventDelegate -= new FrameListener.LeapEventDelegate(parseFrame);
       // controller.RemoveListener(frameListener);
    }
    
    void OnMouseDown() {
        mouseIsDown = true;
    }
    void OnMouseUp() {
        mouseIsDown = false;
    }
    public void LeapEventNotification() {
        Debug.Log("Event notif");
        // this.parseFrame();
    }
    private void parseFrame() {
       /* //skips every second frame
        if((counter % 2) == 0 ){
            counter++;
            return;
        }*/
        Camera camera = Camera.FindObjectOfType<Camera>();

        #region keyTap
        //gets keyTap position. Currently unused
        if(currentFrame != null && mode == Modes.Editor && ((currentFrame.Gestures() != null && !currentFrame.Gestures().IsEmpty)
            || Input.GetMouseButtonDown(1))) {
            foreach(Gesture g in currentFrame.Gestures()) {
                if(g.Type == Gesture.GestureType.TYPE_KEY_TAP && getRightHand() != null) {
                    positionOfTap = rightHandController.
                        transform.TransformPoint(getIndexFinger(getRightHand()).TipPosition.ToUnityScaled());
                    tapActive = true;
                }
            }
            if(tapActive || Input.GetMouseButtonDown(1)) {
                RaycastHit tapHit;
                Ray tapRay = Input.GetMouseButtonDown(1) ? camera.ScreenPointToRay(Input.mousePosition)
                    : new Ray(positionOfTap, positionOfTap - camera.transform.position);
                Vector3 tapHitCoord = new Vector3(0, 0, 0);
                if(Physics.Raycast(tapRay, out tapHit, 1000)) {
                    tapHitCoord = tapHit.point;
                    //Debug.DrawRay(positionOfThumb, positionOfThumb - camera.transform.position, Color.red, 400);
                }
                // get the normalized position of hit relative to the terrain        
                Vector3 coord2;
                coord2.x = tapHitCoord.x / terrain.terrainData.size.x;
                coord2.y = tapHitCoord.z / terrain.terrainData.size.z;
                // get the position of the terrain heightmap where hit is
                positionOfTap = new Vector3((int)(coord2.x * hmWidth), (int)(coord2.y * hmHeight), 0);
                positionOfTap = new Vector3(positionOfTap.x, positionOfTap.y,
                    terrain.terrainData.GetHeight((int)positionOfTap.x, (int)positionOfTap.y));
                Vector3 normalizedPositionOfTap = new Vector3(positionOfTap.x / terrain.terrainData.heightmapWidth,
                    positionOfTap.z / 1000, positionOfTap.y / terrain.terrainData.heightmapHeight);
                if(!terrainAccessories.removeTree(normalizedPositionOfTap)) {
                    if(TreesRemaining > 0) {
                        terrainAccessories.AddTree(normalizedPositionOfTap);
                        TreesRemaining--;
                    }
                } else {
                    TreesRemaining++;
                }
                tapActive = false;
            }
        }
        #endregion

        Finger thumb;
        //if correct gesture is detected
        if(checkForPinch(currentFrame, out thumb, 25)){
            HandModel rightGraphicHand = getRightGraphicHand();
            //pinch detected but hund not in scene, so something is wrong
            if(rightGraphicHand == null || rightGraphicHand.fingers.Length == 0) {
                Debug.Log("graphicHand not found");
                pinchActive = false;
                return;
            }
            pinchActive = true;
            FingerModel graphicThumb = rightGraphicHand.fingers[0];
            positionOfThumb = graphicThumb.GetTipPosition();
            if(mode == Modes.Editor) {
                if(thumb.TipPosition.y > 160) {
                    heightChange = 0.001f;
                } else {
                    if(thumb.TipPosition.y <= 160) {
                        heightChange = -0.001f;
                    } else {
                        heightChange = 0;
                    }
                }
            }
            if(mode == Modes.Interactive) {
                Collider[] close_things = Physics.OverlapSphere(positionOfThumb, grabDistance);
                Vector3 distance = new Vector3(grabDistance, 0.0f, 0.0f);
                Collider grabbedObject = null;
                for(int j = 0; j < close_things.Length; j++) {
                    Vector3 new_distance = positionOfThumb - close_things[j].transform.position;

                    if(close_things[j].tag == "Interactable" && close_things[j].GetComponent<Rigidbody>() != null && new_distance.magnitude < distance.magnitude) {
                        grabbedObject = close_things[j];
                        distance = new_distance;
                    }
                }
                if(grabbedObject != null) {
                    grabbedObject.gameObject.transform.position = positionOfThumb;
                    //Vector3 movedDistance = positionOfThumb - grabbedObject.transform.position;
                    //grabbedObject.GetComponent<Rigidbody>().AddForce(2f * distance);
                    grabbedObject.transform.rotation = Quaternion.identity;
                }
            }
        } else {
            pinchActive = false;
        }

        RaycastHit hit;
        Ray ray = mouseIsDown ? camera.ScreenPointToRay(Input.mousePosition) : new Ray(positionOfThumb, 
            positionOfThumb - camera.transform.position);
        Vector3 tempCoord = new Vector3(0, 0, 0);   
        if(Physics.Raycast(ray, out hit, 1000,terrainLayerMask)) {
            tempCoord = hit.point;
           // Debug.DrawRay(positionOfThumb, positionOfThumb - camera.transform.position, Color.red, 1000);
        }
        
        // get the normalized position of hit relative to the terrain   
        Vector3 coord;
        coord.x = tempCoord.x / terrain.terrainData.size.x;
        coord.y = tempCoord.z / terrain.terrainData.size.z;

        // get the position of the terrain heightmap where hit is
        posXInTerrain = (int)(coord.x * hmWidth);
        posYInTerrain = (int)(coord.y * hmHeight);
        // we set an offset so that all the raising terrain is under this point, rather than at edge
        if(mode == Modes.Editor && posXInTerrain + size < hmWidth && posYInTerrain + size < hmHeight && posXInTerrain > 0 && posYInTerrain > 0 &&
             (pinchActive || mouseIsDown)) {
        
                 if(!pinchActive && mouseIsDown) {
                     heightChange = 0.001f;
                 }
            RaiseGround();
        }
        
            //change everytime, but only on the size*size square
            /* if(counter > 50) {
                 terrainColorChanger.recolorSquare(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
                 counter = 0;
             }
             counter++;*/
        }
    void Update() {
        if(Input.GetKeyDown(KeyCode.LeftShift)) {
            speed = 10;
            GroundAmmountRequired();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)) {
            speed = 1;
            GroundAmmountRequired();
        }
        if(Input.GetKeyDown(KeyCode.LeftControl)) {
            goUp *= -1;
            GroundAmmountRequired();
        }
        if(Input.GetKeyDown("1")) {
            Mode = Modes.Editor;
        }
        if(Input.GetKeyDown("2")) {
            Mode = Modes.Interactive;
           // changeColor of hand
        }
        if(Input.GetKeyDown("3")) {
            Mode = Modes.Playing;
        }
        if(Input.GetKey("3")) {
            terrainSelectionSizeSlider.value++;
        }
        if(Input.GetKey("4")) {
            terrainSelectionSizeSlider.value--;
        }
        if(Input.GetKey("5")) {
            terrainIncrementSlider.value++;
        }
        if(Input.GetKey("6")) {
            terrainIncrementSlider.value--;
        }
        if(Input.GetKeyDown("space")) {
            GroundAmmountRequired();
        }
        //debuging purposes
        if(!controller.IsConnected) {
            heightChange = 0.001f;
            parseFrame();
        }
        currentFrame = controller.Frame();
        //if frame already processed
        if(currentFrame.Id == currentFrameId ) {
            return;
        }
        currentFrameId = currentFrame.Id;
        frameEvent();
        parseFrame();
    }

    private void Smooth(int startX, int startY, int startWidth, int startHeight) {

        float[,] height = terrain.terrainData.GetHeights(startX, startY, startWidth, startHeight);
        float k = 0.98f;
        /* Rows, left to right */
        for(int x = 1; x < startWidth; x++)
            for(int z = 0; z < startHeight; z++)
                height[x, z] = height[x - 1, z] * (1 - k) +
                          height[x, z] * k;

        /* Rows, right to left*/
        for(int x = startWidth - 2; x < -1; x--)
            for(int z = 0; z < startHeight; z++)
                height[x, z] = height[x + 1, z] * (1 - k) +
                          height[x, z] * k;

        /* Columns, bottom to top */
        for(int x = 0; x < startWidth; x++)
            for(int z = 1; z < startHeight; z++)
                height[x, z] = height[x, z - 1] * (1 - k) +
                          height[x, z] * k;

        /* Columns, top to bottom */
        for(int x = 0; x < startWidth; x++)
            for(int z = startHeight; z < -1; z--)
                height[x, z] = height[x, z + 1] * (1 - k) +
                          height[x, z] * k;

        terrain.terrainData.SetHeights(startX, startY, height);
    }

    public void resizeTerrainSelection(float size) {
        this.size = (int) size;
        GroundAmmountRequired();
    }

    public void ChangeTerrainIncrement(float increment) {
        speed = (int) increment;
        GroundAmmountRequired();
    }
    public HandModel getRightGraphicHand() {
        if(currentFrame == null) {
            return null;
        }
        HandModel[] hands = rightHandController.GetAllGraphicsHands();
        foreach(HandModel hand in hands) {
            if(hand.GetLeapHand().IsRight) {
                return hand;
            }
        }
        return null;
    }
    private Hand getRightHand(){
        if(currentFrame == null) {
            return null;
        }
        Hand rightHand = currentFrame.Hand(rightHandID);
        if(!rightHand.IsValid){
            rightHand = null;
            foreach (Hand hand in currentFrame.Hands){
                if(hand.IsRight){
                    rightHand = hand;
                    rightHandID = rightHand.Id;
                    break;
                }
            }
        }
        return rightHand;
    }
    private Finger getThumb(Hand rightHand) {
        if(rightHand == null) {
            return null;
        }
        Finger thumb = rightHand.Finger(thumbID);
        if(!thumb.IsValid) {
            thumb = null;
            foreach(Finger f in rightHand.Fingers){
                if(f.Type() == Finger.FingerType.TYPE_THUMB){
                    thumb = f;
                    thumbID = thumb.Id;
                    break;
                }
            }
        }
        return thumb;
    }
    private Finger getIndexFinger(Hand rightHand) {
        if(rightHand == null) {
            return null;
        }
        Finger indexFinger = rightHand.Finger(indexFingerID); 
        if(!indexFinger.IsValid){
            indexFinger = null;
            foreach(Finger f in rightHand.Fingers){
                if(f.Type() == Finger.FingerType.TYPE_INDEX){
                    indexFinger = f;
                    indexFingerID = indexFinger.Id;
                    break;
                }
            }
        }
        return indexFinger;
    }

    private bool checkForPinch(Frame frame, out Finger thumb, float pinchingDistance) {
        Hand rightHand = getRightHand();
        thumb = getThumb(rightHand);
        Finger indexFinger = getIndexFinger(rightHand);
        if(thumb == null || indexFinger == null){
            thumb = null;
            return false;
        }
        
        if(Vector3.Distance(thumb.TipPosition.ToUnity(), indexFinger.TipPosition.ToUnity()) < pinchingDistance) {
            return true;
        }        
        return false;
    }
    
    private void frameEvent() {
        if(eventDelegate != null) {
            eventDelegate();
        }
    }

    public Terrain[] GetTerrains() {
        return GameObject.FindObjectsOfType<Terrain>();
    }

    private void GroundAmmountRequired() {
        int xCor;
        int yCor;
        float totalHeightChange = 0;
        float low = 1;
        float high = 0;
        middleOfCircle = (middleOfCircle == 0) ? size/2 : middleOfCircle;
        for(int i = 0; i < size; i++) {
            for(int j = 0; j < size; j++) {
                //always a bit different. Sometimes about 1.6 bigger
                //circle equation
                if((i - middleOfCircle) * (i - middleOfCircle) + (j - middleOfCircle) * (j - middleOfCircle) < middleOfCircle * middleOfCircle) {
                    xCor = i - middleOfCircle;
                    yCor = j - middleOfCircle;
                    if(xCor < 0)
                        xCor = -xCor;
                    if(yCor < 0)
                        yCor = -yCor;
                    float newHeightChange = heightChange * ((middleOfCircle - xCor) + (middleOfCircle - yCor)) / 2;
                    if(((middleOfCircle - xCor) + (middleOfCircle - yCor)) / 2 >= 15) {
                        //slow down raising closer to the middleOfCircle of circle;
                        newHeightChange *= 0.6f;
                    }
                    totalHeightChange += newHeightChange * 1000;
                }
            }
        }
        if(goUp < 0) {
            if(totalHeightChange > 0) {
                totalHeightChange *= -1;
            }
        } else {
            if(totalHeightChange < 0) {
                totalHeightChange *= -1;
            }
        }
        totalHeightChange *= speed;
        actualGroundCost = totalHeightChange;
    }

    private void RaiseGround() {
        if(actualGroundCost == -1) {
            GroundAmmountRequired();
        }
        if(actualGroundCost > groundRemaining) {
            Debug.Log("Not enough ground left. Needs: " + actualGroundCost + ", has: " + groundRemaining);
            return;
        }
        int offset = size / 2;
        // get the heights of the terrain at given position
        float[,] heights = terrain.terrainData.GetHeights(posXInTerrain - offset, posYInTerrain - offset, size, size);
        heightChange *= (speed * 0.55f);
        heightChange *= goUp;
        // we set each sample of the terrain in the size to the desired height and in the shape of a circle
        middleOfCircle = size / 2;
        int xCor;
        int yCor;
        for(int i = 0; i < size; i++) {
            for(int j = 0; j < size; j++) {
                //circle equation
                if((i - middleOfCircle) * (i - middleOfCircle) + (j - middleOfCircle) * (j - middleOfCircle) < middleOfCircle * middleOfCircle) {
                    //      if(exp ^ -((1 / 2) * (i - middleOfCircle) * (i - middleOfCircle) + 1 / 2 * (j - middleOfCircle) * (j - middleOfCircle)) < middleOfCircle * middleOfCircle) { 
                    //  heights[i, j] += heightChange;
                    xCor = i - middleOfCircle;
                    yCor = j - middleOfCircle;
                    if(xCor < 0)
                        xCor = -xCor;
                    if(yCor < 0)
                        yCor = -yCor;
                    //not working, making cross
                    //heights[i, j] += heightChange * (((middleOfCircle - xCor) / middleOfCircle) + ((middleOfCircle - yCor) / middleOfCircle)) / 2;
                    //try this. If not, try to not add xcor and ycor together...
                    float newHeightChange = heightChange * ((middleOfCircle - xCor) + (middleOfCircle - yCor)) / 2;
                    if(((middleOfCircle - xCor) + (middleOfCircle - yCor)) / 2 >= 15) {
                        //slow down raising closer to the middleOfCircle of circle;
                        newHeightChange *= 0.6f;
                    }
                    heights[i, j] += newHeightChange;
                    //  heights[i, j] += ((middleOfCircle - i)) > 0 ? (middleOfCircle - i) / 20 : 0;
                    //heights[i, j] += ((middleOfCircle - j) / 20) > 0? (middleOfCircle-j)/20 : 0;
                }
            }
        }
        #region log terrain
        /*string message = "";
           if(message != "s") {
            for(int i = 0; i < heights.GetLength(0); i++) {
                for(int j = 0; j < heights.GetLength(1); j++) {
                    message += (heights[i, j]) + ";";//.ToString("F");
                }
                message += ", height";
            }
           // Debug.Log(message);
            message = "s";
        }
        */
        #endregion
        // set the new height
        terrain.terrainData.SetHeights(posXInTerrain - offset, posYInTerrain - offset, heights);
        terrainColorChanger.recolorSquare(posXInTerrain - offset, posYInTerrain - offset, size, size);
        GroundRemaining -= (int)actualGroundCost;
        // Smooth(posXInTerrain - offset, posYInTerrain - offset, size, size);
        
    }
}

   

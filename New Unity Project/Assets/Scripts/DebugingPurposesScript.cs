using UnityEngine;
using System.Collections;
using Leap;

public class DebugingPurposesScript : MonoBehaviour {
	// Use this for initialization
    public Controller controller = new Controller();
    private int i = 0;
    private RaycastHit hit;
    public Vector3 position;
	void Start () {
	
	}
	
	// Update is called once per frame
   /* void Update() {
        if(i >=0) {
            Frame currentFrame;
            currentFrame = controller.Frame();
            foreach(Hand h in currentFrame.Hands) {
                foreach(Finger f in h.Fingers) {
                    if(f.Type() == Finger.FingerType.TYPE_INDEX) {
                        //Debug.Log("ToUnityScaled is " + transform.TransformPoint(f.TipPosition.ToUnityScaled()));
                     //   Debug.Log("Should be around (202,0,299), (198,307)");
                        Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(transform.TransformPoint(f.TipPosition.ToUnityScaled()));
                        Vector3 tempCoord = new Vector3(0, 0, 0);
                        if(Physics.Raycast(ray, out hit, 1000)) {
                            tempCoord = hit.point;
                            Debug.Log("HIT! coords at cube:" + tempCoord);
                        }
                        position = transform.TransformPoint(f.TipPosition.ToUnityScaled());
                        Debug.DrawRay(FindObjectOfType<Camera>().transform.position, transform.TransformPoint(f.TipPosition.ToUnityScaled()) - FindObjectOfType<Camera>().transform.position, Color.red, 10000);
              
                    }
                }
            }
            i = 0;
        }

        i++;
    }*/
}

using UnityEngine;
using System.Collections;
using Leap;
using System;
public class FollowHand : MonoBehaviour {
    public float BallsSpeed {
        get; set;
    }
    private DirrectionLightUp leftArrow;
    private DirrectionLightUp rightArrow;
    private DirrectionLightUp upArrow;
    private DirrectionLightUp downArrow;

    // Use this for initialization
    private Vector3 cameraStartingPosition;
    public Controller controller;
    HandController leftHandController;
    HandController rightHandController;
    private Quaternion cameraStartingRotation;
    private Quaternion leftHandControllerStartingRotation;
    private Quaternion rightHandControllerStartingRotation;
    private Vector3 leftHandControllerStartingPosition;
    private Vector3 rightHandControllerStartingPosition;
    Transform leftHandControllerTransform;
    Transform rightHandControllerTransform;
    private Frame currentFrame;
    private LeapEventDelegate delegateReference;
    private int leftHandID;
    private int rightHandID;
    Vector3 cameraMovementVector;
    Vector3 cameraRotationVector;
    float maxBallSize = 0;
    Vector3[] fingerVectors;
    int counter;
    private float offset;

    private Vector3 ballStartingPosition;
    private Vector3 oldRightHandMovementScale;
    private Vector3 overBallControllerPosition;
    private Quaternion overBallControllerRotation;
    private Projector shadowProjector;
    private Transform shadowProjectorTransform;
    private Transform ball;
    private Rigidbody ballsRigidbody;
    // The distance in the x-z plane to the target
    private float distance = 35.0f;
    // the height we want the camera to be above the target
    private float height = 15.0f;
    private float rotationDamping = 0.01f;
    private float heightDamping = 0.25f;

    void Awake() {
        shadowProjectorTransform = GameObject.Find("BlobShadowProjector").transform;
        shadowProjector = shadowProjectorTransform.gameObject.GetComponent<Projector>();
    }
    void Start() {

        leftArrow = GameObject.Find("LeftArrow").GetComponent<DirrectionLightUp>();
        rightArrow = GameObject.Find("RightArrow").GetComponent<DirrectionLightUp>();
        upArrow = GameObject.Find("UpArrow").GetComponent<DirrectionLightUp>();
        downArrow = GameObject.Find("DownArrow").GetComponent<DirrectionLightUp>();
        cameraStartingPosition = transform.position;
        cameraStartingRotation = transform.rotation;
        delegateReference = new LeapEventDelegate(parseFrameAndRotate);
        cameraMovementVector = new Vector3(0, 0, 0);
        cameraRotationVector = new Vector3(0, 0, 0);
        fingerVectors = new Vector3[5];
        GameObject ballGameObject = GameObject.Find("Ball");
        if(ballGameObject != null) {
            ball = ballGameObject.transform;
            ballsRigidbody = ball.GetComponent<Rigidbody>();
        }

       

        leftHandControllerTransform = GameObject.FindGameObjectWithTag("LeftHand").transform;
        rightHandControllerTransform = GameObject.FindGameObjectWithTag("RightHand").transform;
        leftHandController = leftHandControllerTransform.gameObject.GetComponent<HandController>();
        rightHandController = rightHandControllerTransform.gameObject.GetComponent<HandController>();

        leftHandControllerStartingPosition = leftHandControllerTransform.position;
        leftHandControllerStartingRotation = leftHandControllerTransform.rotation;
        rightHandControllerStartingPosition = rightHandControllerTransform.position;
        rightHandControllerStartingRotation = rightHandControllerTransform.rotation;
        BallsSpeed = 15f;
        offset = 0;

    }

    public void SetUp() {
        ChangingHeights.Instance.eventDelegate += delegateReference;
        controller = ChangingHeights.Instance.Controller;

        shadowProjector.orthographicSize = ChangingHeights.Instance.size;
    }

    void LateUpdate() {
        if(ChangingHeights.Instance.JustChangedMode == true && 
            (ChangingHeights.Instance.oldMode == ChangingHeights.Modes.Playing 
            || ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing)) {

            if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
                changeHandControllerPosition(true);
                shadowProjector.orthographicSize = 4;
                ChangingHeights.Instance.JustChangedMode = false;
            } else {
                shadowProjector.orthographicSize = ChangingHeights.Instance.size;
                changeHandControllerPosition(false);
            }
        }

        //allows keyboard and mouse based movements of camera
        //if(Input.GetKey(KeyCode.LeftArrow) && ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
        //    transform.position += new Vector3(-1, 0, 0);
        //}
        //if(Input.GetKey(KeyCode.RightArrow) & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
        //    transform.position += new Vector3(1, 0, 0);
        //}
        //if(Input.GetKey(KeyCode.UpArrow) & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
        //    transform.position += new Vector3(0, 0, 1);
        //}
        //if(Input.GetKey(KeyCode.DownArrow) & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
        //    transform.position += new Vector3(0, 0, -1);
        //}
        //if (Input.GetAxis("Mouse ScrollWheel") < 0 & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing)    {
        //    transform.position += new Vector3(0, 1, 0);
        //}
        //if (Input.GetAxis("Mouse ScrollWheel") > 0 & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
        //    transform.position += new Vector3(0, -1, 0);
        //}
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Editor && ChangingHeights.Instance.JustChangedMode) {
            transform.rotation = cameraStartingRotation;
            transform.position = cameraStartingPosition;
            leftHandControllerTransform.rotation = leftHandControllerStartingRotation;
            leftHandControllerTransform.position = leftHandControllerStartingPosition;
            rightHandControllerTransform.rotation = rightHandControllerStartingRotation;
            rightHandControllerTransform.position = rightHandControllerStartingPosition;

            ChangingHeights.Instance.JustChangedMode = false;
        }
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing && !ChangingHeights.Instance.JustChangedMode) {
            leftHandControllerTransform.position = ball.position + new Vector3(-1.5f, 6, -2) - transform.forward;

            rightHandControllerTransform.position = ball.position;
            rightHandControllerTransform.rotation = rightHandControllerStartingRotation;
        }
        
        //camera follows ball
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
            // Early out if we don't have a target
            if(!ball)
                return;

            // Calculate the current rotation angles
            var wantedRotationAngle = ball.eulerAngles.y;
            var wantedHeight = ball.position.y + height;

            var currentRotationAngle = transform.eulerAngles.y;
            var currentHeight = transform.position.y;


            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.parent.position = ball.position;
            transform.localPosition = Vector3.zero;
            transform.localPosition -= Vector3.forward * distance;
            transform.parent.Rotate(Vector3.up, cameraMovementVector.x/3);
            leftHandControllerTransform.rotation = transform.rotation;
            leftHandControllerTransform.Rotate(Vector3.up, -30, Space.World);
            if((offset > -5 && cameraMovementVector.z < 0)||(offset < 5 && cameraMovementVector.z > 0)) {
                offset += cameraMovementVector.z / 10;
            }

         // Set the height of the camera
            transform.position = new Vector3(transform.position.x ,wantedHeight , transform.position.z);
            // Always look at the target
            transform.LookAt(ball.position + new Vector3(0,offset,0));
            cameraMovementVector = Vector3.zero;

            MoveShadow(false);
        } else {
            MoveShadow(true);
        }
    }
    private void restrictRightHandMovement() {
        Hand justHand = ChangingHeights.Instance.getRightHand();
      //  SkeletalHand skeletalHand = (SkeletalHand) justHand;
        HandModel rightGraphicHand = ChangingHeights.Instance.getRightGraphicHand();
        if(rightGraphicHand == null) {
            return;
        }
    }
    private Hand getLeftHand() {
        if(currentFrame == null) {
            return null;
        }
        Hand leftHand = currentFrame.Hand(leftHandID); 
        if(!leftHand.IsValid) {
            leftHand = null;
            foreach(Hand h in currentFrame.Hands) {
                if(h.IsLeft){
                    leftHand = h;
                    leftHandID = leftHand.Id;
                    break;
                }
            }
        }
        return leftHand;
    }

   
    private void parseFrameAndRotate() {

        currentFrame = controller.Frame();

        Hand leftHand = getLeftHand();
        if(leftHand != null) {

            //hand rolled left
            if(leftHand.PalmNormal.Roll > 1.20f) {
                cameraMovementVector.x = Time.deltaTime * 15;
            }
            //hand rolled right
            if(leftHand.PalmNormal.Roll < -0.25f) {
                cameraMovementVector.x = -Time.deltaTime * 15;
            }
            //hand tilted. Tips of fingers pointing upwards
            if(leftHand.Direction.Pitch > 1) {
                cameraMovementVector.z = Time.deltaTime * 20;
            }
            //hand tilted. Tips of fingers pointing downards
            if(leftHand.Direction.Pitch < -0.10f) {
                cameraMovementVector.z = -Time.deltaTime * 20;
            }
            //hand is opened wide
            if(leftHand.SphereRadius > 150.0f || checkOpenedHand()) {
                cameraMovementVector.y = Time.deltaTime * 20;
            }
            //hand is closed
            if(leftHand.SphereRadius < 40) {
                cameraMovementVector.y = -Time.deltaTime * 20;
            }
            //hand is far from user
            /*if(leftHand.PalmPosition.z < -80.0f) {
                // cameraMovementVector.z = Time.deltaTime * 20;
            }
            //hand is close to user
            if(leftHand.PalmPosition.z > 50.0f) {
                //cameraMovementVector.z = -Time.deltaTime * 20;
            }
            //hand is left from leap
            if(leftHand.PalmPosition.x < -75.0f) {
                //cameraMovementVector.x = -Time.deltaTime * 20;
            }
            //hand is right from leap
            if(leftHand.PalmPosition.x > 50.0f) {
                //cameraMovementVector.x = Time.deltaTime * 20;
            }*/
            //reset rotation
            if(leftHand.PalmPosition.y < 40 && ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
                cameraMovementVector = new Vector3(0, 0, 0);
                cameraRotationVector = new Vector3(0, 0, 0);
                transform.position = cameraStartingPosition;
                transform.rotation = cameraStartingRotation;
            }
            if(ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
                transform.Translate(cameraMovementVector, Space.World);
                transform.RotateAround(transform.position, Vector3.right, cameraRotationVector.x);
                transform.RotateAround(transform.position, Vector3.up, cameraRotationVector.y);
                cameraMovementVector = Vector3.zero;
                cameraRotationVector = Vector3.zero;
            } else {
                //Vector3 movement = Vector3.Cross(transform.forward, new Vector3(cameraMovementVector.x, 0.0f, cameraMovementVector.z));// + new Vector3(cameraMovementVector.x, 0.0f, cameraMovementVector.z);
                Vector3 movement = new Vector3(transform.right.x * cameraMovementVector.x, 0, transform.forward.z * cameraMovementVector.z);
                //turning to oposite direction
                if((cameraMovementVector.x > 0 && ballsRigidbody.velocity.x < 0)
                    || (cameraMovementVector.x < 0 && ballsRigidbody.velocity.x > 0)) {
                    movement.x *= 2;
                }
                if(cameraMovementVector.x < 0) {
                    leftArrow.ChangeState(true);
                } else {
                    leftArrow.ChangeState(false);
                }
                if(cameraMovementVector.x > 0) {
                    rightArrow.ChangeState(true);
                } else {
                    rightArrow.ChangeState(false);
                }
                if(cameraMovementVector.z > 0) {
                    upArrow.ChangeState(true);
                } else {
                    upArrow.ChangeState(false);
                }
                if(cameraMovementVector.z < 0) {
                    downArrow.ChangeState(true);
                } else {
                    downArrow.ChangeState(false);
                }
                if(Time.timeScale > 0.1f) {
                    movement.y = 0;
                    //    ballsRigidbody.AddForce(movement * BallsSpeed);
                    if(cameraMovementVector.z > 0) {
                        var HelperForward = Helper.transform.TransformDirection(Vector3.forward);

                        rigidbody.AddForce(HelperForward * MoveSpeed);
                    }

                    if(Input.GetKey("s")) {
                        var HelperBack = Helper.transform.TransformDirection(Vector3.back);

                        rigidbody.AddForce(HelperBack * MoveSpeed);
                    }

                    if(Input.GetKey("a")) {
                        var HelperLeft = Helper.transform.TransformDirection(Vector3.left);

                        rigidbody.AddForce(HelperLeft * MoveSpeed);
                    }

                    if(Input.GetKey("d")) {
                        var HelperRight = Helper.transform.TransformDirection(Vector3.right);

                        rigidbody.AddForce(HelperRight * MoveSpeed);
                    }
                }
            }
        } else {
            leftArrow.ChangeState(false);
            rightArrow.ChangeState(false);
            upArrow.ChangeState(false);
            downArrow.ChangeState(false);
        }
    }
    void OnDestroy() {
        if(Application.loadedLevel != 0) {
            ChangingHeights.Instance.eventDelegate -= delegateReference;
        }
    }


    private bool checkOpenedHand() {
        Hand leftHand = getLeftHand();
        Vector3 leftHandVector = new Vector3(leftHand.Direction.x, leftHand.Direction.y, leftHand.Direction.z);
        Vector3 referenceRight = Vector3.Cross(transform.up, leftHandVector);
        //hand is closed

        for(int i = 0; i < leftHand.Fingers.Count; i++) {
            fingerVectors[i] = new Vector3(leftHand.Fingers[i].Direction.x, leftHand.Fingers[i].Direction.y, leftHand.Fingers[i].Direction.z);
            float angle = Vector3.Angle(fingerVectors[i], leftHandVector);
            float sign = Mathf.Sign(Vector3.Dot(fingerVectors[i], referenceRight));
            //+ is up
            float finalAngle = sign * angle;
            //thumb is in different angle
            if(leftHand.Fingers[i].Type() != Finger.FingerType.TYPE_THUMB) {
                if(sign * angle < -25) {
                    return false;
                }
            } else {
                if(sign * angle > -10) {
                 //   return false;
                }
            }
        }
        return true;
    }

    private void changeHandControllerPosition(bool getOverBall) {
        if(getOverBall) {
            oldRightHandMovementScale = rightHandController.handMovementScale;
            leftHandController.handMovementScale = Vector3.zero;
            rightHandController.handMovementScale = new Vector3(4, 1.5f, 4);
        } else {
            rightHandController.handMovementScale = oldRightHandMovementScale;
            leftHandController.handMovementScale = Vector3.one;
        }

    }

    private void MoveShadow(bool fromCamera) {
        RaycastHit hit;
        HandModel rightGraphicHand = ChangingHeights.Instance.getRightGraphicHand();
        if(!rightGraphicHand) {
            return;
        }
        //if from camera take thumb, else take index finger
        FingerModel finger = rightGraphicHand.fingers[fromCamera ? 0 : 1];
        Vector3 fingerTipPosition = finger.GetTipPosition();
        Ray ray = fromCamera ? new Ray(fingerTipPosition, fingerTipPosition - ChangingHeights.Instance.camera.transform.position): new Ray(fingerTipPosition,Vector3.down);
        if(Physics.Raycast(ray, out hit, 200, ChangingHeights.Instance.terrainLayerMask)) {
            shadowProjectorTransform.position = hit.point + new Vector3(0, 15, 0);//maybe 22
        }
    }
}

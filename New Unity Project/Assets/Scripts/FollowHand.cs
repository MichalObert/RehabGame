using UnityEngine;
using System.Collections;
using Leap;
using System;
public class FollowHand : MonoBehaviour {

	// Use this for initialization
    private Vector3 position;
    public Controller controller;
    HandController leftHandController;
    private Transform controllerTransform;
    private Quaternion cameraStartingRotation;
    private Frame currentFrame;
    private LeapEventDelegate delegateReference;
    private int leftHandID;
    private int rightHandID;
    private bool cameraIsRotated = false;
    Vector3 cameraMovementVector;
    Vector3 cameraRotationVector;
    float maxBallSize = 0;
    Vector3[] fingerVectors;
    int counter;
    //System.Diagnostics.Stopwatch stopwatchTest;
    //System.Diagnostics.Stopwatch stopwatchGood;
    //long goodTime = 0;
    //long testTime = 0;
    private Vector3 oldControllerPosition;
    private Vector3 overBallControllerPosition;
    private Quaternion overBallControllerRotation;
    private Transform ball;
    private Rigidbody ballsRigidbody;
    private float ballsSpeed;
    // The distance in the x-z plane to the target
    private float distance = 30.0f;
    // the height we want the camera to be above the target
    private float height = 20.0f;
    private bool justChangedMode = false;
    private float rotationDamping;
    private float heightDamping;

	void Start () {
        controller = ChangingHeights.Instance.Controller;
        position = transform.position;
        cameraStartingRotation = transform.localRotation;
        delegateReference = new LeapEventDelegate(parseFrameAndRotate);
        ChangingHeights.Instance.eventDelegate += delegateReference;
        cameraMovementVector = new Vector3(0, 0, 0);
        cameraRotationVector = new Vector3(0, 0, 0);
        fingerVectors = new Vector3[5];
        GameObject ballGameObject = GameObject.Find("Ball");
        if(ballGameObject != null) {
            ball = ballGameObject.transform;
            ballsRigidbody = ball.GetComponent<Rigidbody>();
        }
        leftHandController = GameObject.FindGameObjectWithTag("LeftHand").GetComponent<HandController>();
        controllerTransform = leftHandController.transform;
        overBallControllerRotation = controllerTransform.rotation;

        //                              use custom hand
        /*RigidHand rigidHand = (RigidHand)ChangingHeights.Instance.rightHandController.rightPhysicsModel;
        if(rigidHand.palm == null) {
            Debug.Log("wrong hand asigned");
        }
        ChangingHeights.Instance.rightHandController.rightPhysicsModel = null;
        ChangingHeights.Instance.rightHandController.rightPhysicsModel = ChangingHeights
            .Instance.rightHandController.gameObject.GetComponent<customHandModel>();
        ((customHandModel)ChangingHeights.Instance.rightHandController.rightPhysicsModel)
            .initCustomHandModel(rigidHand.palm, rigidHand.forearm, rigidHand.wristJoint);*/

    }
    void FixedUpdate() {
    }
    void LateUpdate() {
        if(ChangingHeights.Instance.JustChangedMode == true) {
            ChangingHeights.Instance.JustChangedMode = false;
            if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
                changeHandControllerPosition(true);
            } else {
                changeHandControllerPosition(false);
            }
        }

        if(Input.GetKey(KeyCode.LeftArrow) && ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
            transform.position += new Vector3(-1, 0, 0);
        }
        if(Input.GetKey(KeyCode.RightArrow) & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
            transform.position += new Vector3(1, 0, 0);
        }
        if(Input.GetKey(KeyCode.UpArrow) & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
            transform.position += new Vector3(0, 0, 1);
        }
        if(Input.GetKey(KeyCode.DownArrow) & ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
            transform.position += new Vector3(0, 0, -1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)    {
            transform.position += new Vector3(0, 1, 0);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            transform.position += new Vector3(0, -1, 0);
        }
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Editor && cameraIsRotated) {
            transform.RotateAround(transform.position, transform.right, 65);
            transform.Translate(Vector3.up * 80, Space.World);
            
            GameObject leftHandController = GameObject.FindGameObjectWithTag("LeftHand"); 
            GameObject rightHandController = GameObject.FindGameObjectWithTag("RightHand");
            leftHandController.transform.RotateAround(leftHandController.transform.position, transform.right, -65);
            rightHandController.transform.RotateAround(rightHandController.transform.position, transform.right, -65);
            cameraIsRotated = false;
        } 
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Interactive && !cameraIsRotated) {
            transform.RotateAround(transform.position, transform.right, -65);
            transform.Translate(-Vector3.up * 80, Space.World);
            GameObject leftHandController = GameObject.FindGameObjectWithTag("LeftHand");
            GameObject rightHandController = GameObject.FindGameObjectWithTag("RightHand");
            leftHandController.transform.RotateAround(leftHandController.transform.position, leftHandController.transform.right, 65);
            rightHandController.transform.RotateAround(rightHandController.transform.position, leftHandController.transform.right, 65);
            cameraIsRotated = true;
        }
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
        
            // Early out if we don't have a target
            if (!ball)
                return;

            // Calculate the current rotation angles
            var wantedRotationAngle = ball.eulerAngles.y;
            var wantedHeight = ball.position.y + height;

            var currentRotationAngle = transform.eulerAngles.y;
            var currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = ball.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            transform.position = new Vector3(transform.position.x ,currentHeight , transform.position.z);

            // Always look at the target
            transform.LookAt(ball);
        }
        
       // if(canStart)
        //transform.position += Vector3.Lerp(transform.position, cameraMovementVector + transform.position, Time.deltaTime);
        restrictRightHandMovement();
        controllerTransform.rotation = overBallControllerRotation;
    }
    private void restrictRightHandMovement() {
        Hand justHand = ChangingHeights.Instance.getRightHand();
      //  SkeletalHand skeletalHand = (SkeletalHand) justHand;
        HandModel rightGraphicHand = ChangingHeights.Instance.getRightGraphicHand();
        if(rightGraphicHand == null) {
            return;
        }
        Vector3 isInCameraFOV = ChangingHeights.Instance.camera.WorldToViewportPoint(rightGraphicHand.fingers[0].GetTipPosition());
        if(isInCameraFOV.x > 1 || isInCameraFOV.x < 0 || isInCameraFOV.y > 1 || isInCameraFOV.y < 0) {
            Debug.Log("out");
            //try hand.palm.transform.position
            //ChangingHeights.Instance.rightHandController. 
        }

        /*                                                            ctrl+c from web
        public Vector3 ProjectedPalmPosition() {
	Leap.InteractionBox ibox = controller_.GetFrame().InteractionBox;
	Leap.Vector leapPalmPosition = hand_.PalmPosition;
	Leap.Vector normalizedPalmPosition = ibox.NormalizePoint(leapPalmPosition, true);
	Vector3 position = new Vector3(normalizedPalmPosition.x, normalizedPalmPosition.y, normalizedPalmPosition.z);
	position.z = (cam.farClipPlane - cam.nearClipPlane) - position.z * (cam.farClipPlane - cam.nearClipPlane);

	return cam.ViewportToWorldPoint(position);
            
}*/
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
            //stopwatchGood.Start();
            //stopwatchTest.Start();

            //hand rolled left
            if(leftHand.PalmNormal.Roll > 0.80f || Input.GetKey(KeyCode.LeftArrow)) {
                //cameraRotationVector.y = -Time.deltaTime * 20;   //was rotation to the left
                cameraMovementVector.x = -Time.deltaTime * 20;
            }
            //hand rolled right
            if(leftHand.PalmNormal.Roll < -0.30f) {
                //cameraRotationVector.y = Time.deltaTime * 20;  //was rotation to the right
                cameraMovementVector.x = Time.deltaTime * 20;
            }
            //hand tilted. Tips of fingers pointing upwards
            if(leftHand.Direction.Pitch > 0.8f) {
                //cameraRotationVector.x = -Time.deltaTime * 20;  //was looking up
                cameraMovementVector.z = -Time.deltaTime * 20;
            }
            //hand tilted. Tips of fingers pointing downards
            if(leftHand.Direction.Pitch < -0.15f) {
                //cameraRotationVector.x = Time.deltaTime * 20;   //was looking down
                cameraMovementVector.z = Time.deltaTime * 20;
            }
            //hand is opened wide
            if(leftHand.SphereRadius > 150.0f  || checkOpenedHand()) {
                //transform.Translate(Vector3.up * Time.deltaTime * 20,Space.World);
                cameraMovementVector.y = Time.deltaTime * 20;
            }
      
            if(leftHand.SphereRadius < 40) {
                //transform.Translate(-Vector3.up * Time.deltaTime * 20, Space.World);
                cameraMovementVector.y = -Time.deltaTime * 20;
            }
            //hand is far from user
            if(leftHand.PalmPosition.z < -80.0f) {
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
            }
            //reset rotation
            if(leftHand.PalmPosition.y < 40 && ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
                Debug.Log("rotation reset");
                cameraMovementVector = new Vector3(0, 0, 0);
                cameraRotationVector = new Vector3(0, 0, 0);
                transform.position = position;
                transform.localRotation = cameraStartingRotation;
                switch(ChangingHeights.Instance.Mode) {
                    case ChangingHeights.Modes.Editor:
                        //nothing necessary
                        break;
                    case ChangingHeights.Modes.Interactive:
                        transform.RotateAround(transform.position, transform.right, -65);
                        transform.Translate(-Vector3.up * 80, Space.World);
                        break;
                }
            }
            if(ChangingHeights.Instance.Mode != ChangingHeights.Modes.Playing) {
                transform.Translate(cameraMovementVector, Space.World);
                transform.RotateAround(transform.position, Vector3.right, cameraRotationVector.x);
                transform.RotateAround(transform.position, Vector3.up, cameraRotationVector.y);
                cameraMovementVector = Vector3.zero;
                cameraRotationVector = Vector3.zero;
            } else {

                Vector3 movement = new Vector3(cameraMovementVector.x, 0.0f, cameraMovementVector.z);
                ballsSpeed = 10;
                ballsRigidbody.AddForce(movement * ballsSpeed);
            }
        }
    }
    void OnDestroy() {
        transform.position = position;
        transform.rotation = cameraStartingRotation;
        ChangingHeights.Instance.eventDelegate -= delegateReference;
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
            leftHandController.transform.parent = ball;
            oldControllerPosition = controllerTransform.position;
            overBallControllerPosition = ball.position + new Vector3(0,10,0);
            controllerTransform.position = overBallControllerPosition;
            leftHandController.handMovementScale = Vector3.zero;
        } else {
            leftHandController.transform.parent = ChangingHeights.Instance.camera.transform;
            leftHandController.handMovementScale = Vector3.one;
            controllerTransform.position = oldControllerPosition;
        }
    }
   // public void LeapEventNotification() {
       // parseFrame();
   // }
}
// The target we are following

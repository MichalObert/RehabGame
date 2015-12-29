using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
    public Transform target;
    private Vector3 startingPosition;
    // Use this for initialization
    Transform ball;
    Rigidbody ballRigidBody;
    GameObject targetForBall;

    void Start() {
        
    }

    void FixedUpdate() {
        if(!ball) {
            Debug.Log("ball not found");
            return;
        }
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
            if(ball.transform.position.y < ChangingHeights.Instance.terrain.transform.position.y - 5) {
                Debug.Log("Ball has fallen off of the terrain. Reseting balls position...");
                ballRigidBody.velocity = Vector3.zero;
                ballRigidBody.angularVelocity = Vector3.zero;
                ball.transform.position = startingPosition;
            }
        } 
    }

    public void ReturnToStart() {
        transform.position = startingPosition;
        ballRigidBody.velocity = Vector3.zero;
        ballRigidBody.angularVelocity = Vector3.zero;

        ChangingHeights.Instance.camera.transform.parent.LookAt(target);
    }

    public void SetUp() {
        GameObject ballGameObject = GameObject.Find("Ball");
        if(ballGameObject != null) {
            ball = ballGameObject.transform;
        }
        ballRigidBody = ball.gameObject.GetComponent<Rigidbody>();
        GameObject ballStartingPointGameObject;
        ballStartingPointGameObject = GameObject.FindGameObjectWithTag("BallSpawnPoint");

        if(ballStartingPointGameObject != null) {
            startingPosition = ballStartingPointGameObject.transform.position;
        } else {
            startingPosition = transform.position;
        }
        startingPosition += new Vector3(0, 22, 0);
        transform.position = startingPosition;
        targetForBall = GameObject.Find("TargetForBall");
        if(targetForBall) {
            ChangingHeights.Instance.camera.transform.parent.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, targetForBall.transform.position - transform.position, 2, 0.0f));
        }
        ballRigidBody.velocity = Vector3.zero;
        ballRigidBody.angularVelocity = Vector3.zero;
    }
    
   
    // Update is called once per frame
    
}

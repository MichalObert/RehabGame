using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {
    private Vector3 startingPosition;
    // Use this for initialization
    Transform ball;
    void Start() {
        GameObject ballGameObject = GameObject.Find("Ball");
        if(ballGameObject != null) {
            ball = ballGameObject.transform;
        }
        GameObject ballStartingPointGameObject;
        ballStartingPointGameObject = GameObject.FindGameObjectWithTag("BallSpawnPoint");

        if(ballStartingPointGameObject != null) {
            startingPosition = ballStartingPointGameObject.transform.position;
        } else {
            Debug.Log("Did not find cross");
            startingPosition = transform.position;
        }
        startingPosition += new Vector3(0, 22, 0);
        transform.position = startingPosition;
        //ball.gameObject.SetActive(false);
    }

    void FixedUpdate() {
        if(!ball) {
            Debug.Log("ball not found");
            return;
        }
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
            //if(!ball.gameObject.activeSelf) {
            //    ball.gameObject.SetActive(true);
            //}
            if(ball.transform.position.y < ChangingHeights.Instance.terrain.transform.position.y - 5) {
                Debug.Log("Ball has fallen off of the terrain. Reseting balls position...");
                Rigidbody ballRigidBody = ball.gameObject.GetComponent<Rigidbody>();
                ballRigidBody.velocity = Vector3.zero;
                ballRigidBody.angularVelocity = Vector3.zero;
                ball.transform.position = startingPosition;
            }
        } 
        //else {
        //    if(ball.gameObject.activeSelf) {
        //        ball.gameObject.SetActive(false);
        //    }
        //}
    }
    
   
    // Update is called once per frame
    
}

using UnityEngine;
using System.Collections;

public class Returner : MonoBehaviour {
    public int distance;
    private Transform ball;
    private Collider returnerCollider;
    private BallScript ballScript;

	void Start () {
        ball = ChangingHeights.Instance.ball;
        returnerCollider = gameObject.GetComponent<Collider>();
        ballScript = ball.gameObject.GetComponent<BallScript>();
	}

    void OnTriggerEnter() {
        ballScript.Invoke("ReturnToStart", 1.2f);
    }
        // Update is called once per frame
        void Update () {
        if(!returnerCollider && Vector3.Distance(transform.position, ball.position) < distance) {
            ballScript.Invoke("ReturnToStart", 1.2f);
        }
	}
}

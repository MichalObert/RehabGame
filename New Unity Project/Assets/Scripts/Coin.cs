using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {
    private float distanceToBall;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up, 1, Space.World);
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing) {
            distanceToBall = Vector3.Distance(transform.position, ChangingHeights.Instance.ball.position);
            if(distanceToBall < 5.5f) {
                Player.Instance.Score++;
                ChangingHeights.Instance.numberOfRemainingCoinsInLevel--;
                Destroy(gameObject);
            }
        }
	}
}

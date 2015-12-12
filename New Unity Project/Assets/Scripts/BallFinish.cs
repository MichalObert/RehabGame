using UnityEngine;
using System.Collections;

public class BallFinish : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing 
            && Vector3.Distance(transform.position, ChangingHeights.Instance.ball.position) < 17) {
            Player.Instance.isInFinish = true;
            this.enabled = false;
        }
        
	}
}

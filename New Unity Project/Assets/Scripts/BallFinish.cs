using UnityEngine;
using System.Collections;

public class BallFinish : MonoBehaviour {


	// Update is called once per frame
	void Update () {
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing 
            && Vector3.Distance(transform.position, ChangingHeights.Instance.ball.position) < 19) {
            Player.Instance.isInFinish = true;
            this.enabled = false;
        }
        
	}
}

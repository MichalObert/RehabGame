using UnityEngine;
using System.Collections;

public class DebugingPurposesScript2 : MonoBehaviour {
    public ChangingHeights script;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       // transform.position = script.positionOfThumb;
        HandModel[] hands = script.rightHandController.GetAllGraphicsHands();
        HandModel aHand;
        if(hands.Length > 0) {
            aHand = hands[0];
            FingerModel finger = aHand.fingers[0]; //thumb is index 0
            transform.position = finger.GetTipPosition();
        }
	}
}

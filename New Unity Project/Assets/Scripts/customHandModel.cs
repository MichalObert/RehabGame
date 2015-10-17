using UnityEngine;
using System.Collections;
using Leap;

public class customHandModel : RigidHand {

	public override void InitHand(){
        base.InitHand();
    }
    public void initCustomHandModel(GameObject palm, GameObject forearm, GameObject wristJoint) {
        this.palm = palm;
        this.forearm = forearm;
        this.wristJoint = wristJoint;
    }
    public override void UpdateHand() {
        Debug.Log("palm pos: " + base.hand_.PalmPosition);
        base.UpdateHand();
    }
}

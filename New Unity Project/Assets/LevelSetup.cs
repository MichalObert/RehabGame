using UnityEngine;
using UnityEngine.UI;

public class LevelSetup : MonoBehaviour {

	// Use this for initialization
	void Start() {
        GameObject.FindObjectOfType<FollowHand>().SetUp();
        GameObject.FindObjectOfType<BallScript>().SetUp();
    }
}

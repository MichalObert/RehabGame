using UnityEngine;
using UnityEngine.UI;

public class LevelSetup : MonoBehaviour {

    GameObject canvas;
    GameObject mode;
    GameObject trees;
	// Use this for initialization
	void Start() {
        GameObject.FindObjectOfType<FollowHand>().SetUp();
        GameObject.FindObjectOfType<BallScript>().SetUp();
        if(Application.loadedLevel == 5) {
            canvas = GameObject.Find("Canvas");
            mode = canvas.transform.FindChild("Mode").gameObject;
            trees = canvas.transform.FindChild("Trees").gameObject;
            mode.SetActive(true);
            trees.SetActive(false);
        }
    }
    void OnDestroy() {
        if(Application.loadedLevel == 5) {
            mode.SetActive(false);
            trees.SetActive(true);
        }
    }
}

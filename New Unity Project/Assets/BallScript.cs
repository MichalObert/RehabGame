using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
    void OnCollisionEnter(Collision col) {
        Debug.Log("collision detected. Name: " + col.gameObject.name);
    }
    // Update is called once per frame
    void Update () {
	
	}
}

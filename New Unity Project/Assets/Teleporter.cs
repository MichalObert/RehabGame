using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour {

    public Transform location;
    public Transform futureTarget;
    // Use this for initialization
	void Start () {
        //this is end of the one way portal, no need for script to be working
        if(!location) {
            Destroy(this);
        }
	}

    void OnTriggerEnter(Collider other) {
        other.transform.position = location.position;
        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ChangingHeights.Instance.camera.transform.parent.Rotate(Vector3.up, 60);
        //_!_TODO finish this (line up camera with futureTarget and ball
    }



}

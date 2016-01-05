using UnityEngine;
using System.Collections;

public class PersistableEnabler : MonoBehaviour {

    // Use this for initialization
    GameObject persistable;
	void Start () {
        persistable = GameObject.Find("Persistable");
        foreach(Transform child in persistable.transform) {
            if(child.gameObject.name != "Ball") {
                child.gameObject.SetActive(false);
            }
        }
	}

    void OnDestroy() {
        if(persistable) {
            foreach(Transform child in persistable.transform) {
                child.gameObject.SetActive(true);
            }
            foreach(Transform child in GameObject.Find("Arrows").transform) {
                child.gameObject.SetActive(true);
            }
        }
    }
}

using UnityEngine;
using System.Collections;

public class DirrectionLightUp : MonoBehaviour {

    void Start() {
        gameObject.SetActive(true);
    }

    public void ChangeState(bool turnOn) {
        if(turnOn) {
            //this.enabled
        } else {
            //this.SetActive(false);
        }
    }
}

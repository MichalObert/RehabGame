using UnityEngine;
using System.Collections;

public class DirrectionLightUp : MonoBehaviour {

    void Start() {
    }

    public void ChangeState(bool turnOn) {
            gameObject.SetActive(turnOn);
    }
}

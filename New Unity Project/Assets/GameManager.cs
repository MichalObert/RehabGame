using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    GameObject escapeMenu;
    bool canClick;
	// Use this for initialization
	void Awake () {
        escapeMenu = GameObject.Find("EscapeMenu");
        escapeMenu.SetActive(false);
        canClick = true;
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Escape) && canClick) {
            canClick = false;
            if(Time.timeScale != 0.00001f) {
                Time.timeScale = 0.00001f;
                escapeMenu.SetActive(true);
                Invoke("allowClick", Time.timeScale);
            } else {
                escapeMenu.SetActive(false);
                Time.timeScale = 1;
                Invoke("allowClick", Time.timeScale);
            }
        }
        if(Input.GetKeyDown(KeyCode.E) && Time.timeScale == 0.00001f){
            escapeMenu.SetActive(false);
            Time.timeScale = 1;
            Application.LoadLevel(0);
        }
    }
    private void allowClick() {
        canClick = true;
    }
}

using UnityEngine;
using UnityEngine.UI;


public class GUIButtonsReactions : MonoBehaviour {

    GameObject levelMenu;
    GameObject quitCheck;
    public Button startGame;
    public Button quitGame;
    public Button chooseLevel;
	// Use this for initialization
	void Start () {
        levelMenu = GameObject.Find("LevelMenu");
        quitCheck = GameObject.Find("QuitCheck");
        startGame = GameObject.Find("StartGame").GetComponent<Button>();
        quitGame = GameObject.Find("QuitGame").GetComponent<Button>();
        //chooseLevel = GameObject.Find("ChooseLevel").GetComponent<Button>();
        quitCheck.SetActive(false);
        levelMenu.SetActive(false);
	}

    public void LoadLevel(int level) {
      Application.LoadLevel(level);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void StartGame() {
        Application.LoadLevel(1);
    }

    public void ChooseLevel() {
        levelMenu.SetActive(true);
        chooseLevel.interactable = false;
        quitGame.interactable = false;
        startGame.interactable = false;
    }

    public void CancelChooseLevel() {
        levelMenu.SetActive(false);
        chooseLevel.interactable = true;
        quitGame.interactable = true;
        startGame.interactable = true;
    }

    public void ExitGameProp() {
        quitCheck.SetActive(true);
        chooseLevel.interactable = false;
        quitGame.interactable = false;
        startGame.interactable = false;
        
    }
    public void CancelExitGameProp() {
        quitCheck.SetActive(false);
        chooseLevel.interactable = true;
        quitGame.interactable = true;
        startGame.interactable = true;
    }
}

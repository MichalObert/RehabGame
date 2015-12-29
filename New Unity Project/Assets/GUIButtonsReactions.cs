using UnityEngine;
using UnityEngine.UI;


public class GUIButtonsReactions : MonoBehaviour {

    GameObject levelMenu;
    GameObject quitCheck;
    GameObject statsImage;
    GameObject escapeMenu;
    public Button startGame;
    public Button quitGame;
    public Button chooseLevel;
    public Button stats;
    Text score;
    Text playedTime;
    Text levelsFinished;
    Text coinsCollected;
	// Use this for initialization
	void Start () {
        levelMenu = GameObject.Find("LevelMenu");
        statsImage = GameObject.Find("StatsImage");
        escapeMenu = GameObject.Find("EscapeMenu");
        quitCheck = GameObject.Find("QuitCheck");
        startGame = GameObject.Find("StartGame").GetComponent<Button>();
        quitGame = GameObject.Find("QuitGame").GetComponent<Button>();
        stats = GameObject.Find("Stats").GetComponent<Button>();
        score = GameObject.Find("TotalScore").GetComponent<Text>();
        levelsFinished = GameObject.Find("LevelsFinished").GetComponent<Text>();
        playedTime = GameObject.Find("PlayedTime").GetComponent<Text>();
        coinsCollected = GameObject.Find("CoinsCollected").GetComponent<Text>();
        //chooseLevel = GameObject.Find("ChooseLevel").GetComponent<Button>();
        quitCheck.SetActive(false);
        levelMenu.SetActive(false);
        statsImage.SetActive(false);
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
        stats.interactable = false;
    }

    public void CancelChooseLevel() {
        levelMenu.SetActive(false);
        chooseLevel.interactable = true;
        quitGame.interactable = true;
        stats.interactable = true;
        startGame.interactable = true;
    }

    public void ExitGameProp() {
        quitCheck.SetActive(true);
        chooseLevel.interactable = false;
        quitGame.interactable = false;
        startGame.interactable = false;
        stats.interactable = false;
    }
    public void CancelExitGameProp() {
        quitCheck.SetActive(false);
        chooseLevel.interactable = true;
        quitGame.interactable = true;
        startGame.interactable = true;
        stats.interactable = true;
    }

    public void ShowStats() {
        chooseLevel.interactable = false;
        quitGame.interactable = false;
        startGame.interactable = false;
        stats.interactable = false;
        statsImage.SetActive(true);
        score.text = "Total Score: " + Player.Instance.TotalScore;
        playedTime.text = "Time played: " + Player.Instance.PlayedTimeInHours();
        levelsFinished.text = "Levels finished: " + Player.Instance.TotalNumberOfLevelsFinished;
        coinsCollected.text = "Coins collected: " + Player.Instance.TotalNumberOfCoinsCollected;
    }

    public void CancelShowStats() {
        chooseLevel.interactable = true;
        quitGame.interactable = true;
        startGame.interactable = true;
        stats.interactable = true;
        statsImage.SetActive(false);
    }


}

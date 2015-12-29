using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public static Player Instance {
        get; private set;
    }

    private int score;
    public int Score {
        get {
            return score;
        }
        set {
            score = value;
           if(scoreText != null)
                scoreText.text = "Score: " + value;
        }
    }
    public int TotalScore {
        get; set;
    }
    public float timePlayed {
        get; set;
    }
    Text scoreText;
    public Transform ball;
    public bool isInFinish = false;
    public int TotalNumberOfCoinsCollected {
        get; set;
    }
    public int TotalNumberOfLevelsFinished{
        get; set;
    }

    // Use this for initialization
    void Start () {
        timePlayed = 0;
        Instance = this;
        TotalNumberOfCoinsCollected = 0;
        TotalNumberOfLevelsFinished = 0;
        scoreText = GameObject.Find("Score").GetComponent<Text>();
        Score = 0;
        TotalScore = 0;
    }

    void Awake() {
        if(FindObjectsOfType(GetType()).Length > 1) {
            Destroy(gameObject);
        }
        GameObject ballGameObject = GameObject.Find("Ball");
        if(ballGameObject != null) {
            ball = ballGameObject.transform;
        }
        DontDestroyOnLoad(gameObject);
    }
	// Update is called once per frame
	void Update () {
        if(Application.loadedLevel != 0) {
            timePlayed += Time.deltaTime;
        } else {
            return;
        }
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing && isInFinish) {
            Debug.Log("In finish");
            isInFinish = false;
            TotalNumberOfLevelsFinished++;
            float levelCompletitionScore =
    (ChangingHeights.Instance.numberOfCoinsInLevel - ChangingHeights.Instance.numberOfRemainingCoinsInLevel)
    / (ChangingHeights.Instance.numberOfCoinsInLevel * 1.0f);
            Debug.Log("Coins in level: " + ChangingHeights.Instance.numberOfCoinsInLevel);
            Debug.Log("Remaining coins in level: " + ChangingHeights.Instance.numberOfRemainingCoinsInLevel);
            Score += 10;
            Score += (int) (10 * levelCompletitionScore);
            TotalScore += Score;
            TotalNumberOfCoinsCollected += ChangingHeights.Instance.numberOfCoinsInLevel 
                - ChangingHeights.Instance.numberOfRemainingCoinsInLevel;
            Debug.Log("Level completed at " + levelCompletitionScore * 100 + "%");
                Application.LoadLevel((Application.loadedLevel + 1) % Application.levelCount);
        }
    }

    public string PlayedTimeInHours() {
        int minutes = Mathf.FloorToInt(timePlayed / 60F);
        int seconds = Mathf.FloorToInt(timePlayed - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}

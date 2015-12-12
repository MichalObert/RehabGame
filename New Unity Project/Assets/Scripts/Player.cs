using UnityEngine;
using System.Collections;

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
            //collected coin
            if(value == score + 1) {
                Debug.Log("Coin collected");
                totalNumberOfCoinsCollected++;
            }
            score = value;
        }
    }
    public Transform ball;
    public bool isInFinish = false;
    private int totalNumberOfCoinsCollected = 0;
    private int totalNumberOfLevelsCompleted = 0;

    // Use this for initialization
    void Start () {
        Instance = this;
        Score = 0;
        GameObject ballGameObject = GameObject.Find("Ball");
        if(ballGameObject != null) {
            ball = ballGameObject.transform;
        }
    }

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }
	// Update is called once per frame
	void Update () {
        if(ChangingHeights.Instance.Mode == ChangingHeights.Modes.Playing && isInFinish) {
            Debug.Log("In finish");
            isInFinish = false;
            totalNumberOfLevelsCompleted++;
            score += 10;
            float levelCompletitionScore = 
                (ChangingHeights.Instance.numberOfCoinsInLevel - ChangingHeights.Instance.numberOfRemainingCoinsInLevel) 
                / ChangingHeights.Instance.numberOfCoinsInLevel;
            Debug.Log("Level completed at " + levelCompletitionScore * 100 + "%");
        }
    }
}

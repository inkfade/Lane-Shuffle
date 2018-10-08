using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TrackObjectManager), typeof(PlayerController), typeof(MouseAndTouchManager))]
public class GameController : MonoBehaviour
{
    private TrackObjectManager trackObjectManager;
    private PlayerController playerController;
    private MouseAndTouchManager mouseAndTouchManager;

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private ScoreText inGameScoreText;
    [SerializeField]
    private Text finalScoreText;


    [SerializeField]
    private float initialTrackSpeed = 1f;
    [SerializeField]
    private float finalTrackSpeed = 2f;
    [SerializeField]
    private float trackAccelerationRate = 1f;
    private float trackSpeed = 2f;

    [SerializeField]
    private float gameOverDelay = 1f;

    private int score = 0;
    private bool gameIsInProgress = false;
    public bool GameIsInProgress { get { return gameIsInProgress; } }


    private void Awake()
    {
        trackObjectManager = GetComponent<TrackObjectManager>();
        playerController = GetComponent<PlayerController>();
        mouseAndTouchManager = GetComponent<MouseAndTouchManager>();
    }


    public void AddToScore(int amount)
    {
        score += amount;
        inGameScoreText.UpdateScore(score);
        inGameScoreText.Bounce();
    }


    private void Start()
    {
        StartGame();
    }


    // Start game
    public void StartGame()
    {
        gameOverPanel.SetActive(false);
        mouseAndTouchManager.SetInputEnabled(true);
        trackObjectManager.InitializeTrack();
        trackSpeed = initialTrackSpeed;
        trackObjectManager.TargetMoveSpeed = trackSpeed;
        playerController.SpawnPlayer();
        score = 0;
        inGameScoreText.UpdateScore(0);
        gameIsInProgress = true;
    }

    // Game over
    public void GameOver()
    {
        trackObjectManager.TargetMoveSpeed = 0f;
        gameIsInProgress = false;
        mouseAndTouchManager.SetInputEnabled(false);

        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(gameOverDelay);
        gameOverPanel.SetActive(true);

        int topScore = PlayerPrefs.GetInt("Top Score");
        // 0 can't be a top score because they haven't scored anything.
        if (topScore > 0)
        {
            if (score > topScore)
            {
                finalScoreText.text = "New record!\nYour score: " + score + "\nPrevious top score: " + topScore;
                PlayerPrefs.SetInt("Top Score", score);
            }
            else
            {
                finalScoreText.text = "Your score: " + score + "\nTop score: " + topScore;
            }
        }
        else
        {
            finalScoreText.text = "Your score: " + score;
            PlayerPrefs.SetInt("Top Score", score);
        }


    }

    private void Update()
    {
        // accelerate the track
        if (gameIsInProgress)
        {
            trackSpeed = Mathf.Lerp(trackSpeed, finalTrackSpeed, Time.deltaTime * trackAccelerationRate);
            trackObjectManager.TargetMoveSpeed = trackSpeed;
            //Debug.Log(trackSpeed);
        }
    }
}

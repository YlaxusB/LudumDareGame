using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ScoreHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public int score = 0;
    public float timeLeft = 45f; // The starting time in seconds
    public RoadGenerator roadGenerator;
    public StartScreenScript startScreenScript;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI highestScoreTextMainUI;
    public int speedGain = 5;
    int scoreIndex = 0;
    int highestScore = 0;

    public Transform timeOverScreen;
    public TextMeshProUGUI timeOverHighestScoreText;
    public TextMeshProUGUI timeOverScoreText;
    public Button timeOverRestartButton;
    public GameObject scoreUI;
    public PlayerController playerController;

    private Canvas canvas;
    private RectTransform canvasRectTransform;
    private float screenWidth;
    private float screenHeight;
    private void Start()
    {

    }
    public void UpdateScore(float scoreGain)
    {
        transform.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
        roadGenerator.gameSpeed += speedGain;
        scoreIndex++;
        /*
        if(scoreGain >= 90)
        {
            roadGenerator.gameSpeed += 5;
        } else
        {
            roadGenerator.gameSpeed += 10;
        }
        */
        timeLeft += 4;
    }

    private void Update()
    {
        // If theres more than 0 seconds update the time text
        if (timeLeft > 0 && startScreenScript.gameSpeed > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = "Time Left: " + ((int)timeLeft).ToString();
        }
        else if (startScreenScript.gameSpeed > 0)
        {
            // Open the time over screen and reset score, time and gameSpeed
            roadGenerator.gameSpeed -= speedGain * scoreIndex;
            if (score > highestScore) {
                highestScore = score;
            }
            playerController.isRestarting = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            timeOverScreen.gameObject.SetActive(true);
            scoreUI.SetActive(false);
            timeOverScoreText.text = "Score: " + score.ToString();
            timeOverHighestScoreText.text = "Highest Score: " + highestScore.ToString();
            score = 0;
            timeLeft = 45.0f; // The starting time in seconds
            transform.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
            highestScoreTextMainUI.text = "Highest Score: " + highestScore.ToString();
            timerText.text = "Time: " + ((int)timeLeft).ToString();
            scoreIndex = 0;
            // Open time over screen

        }
    }
}

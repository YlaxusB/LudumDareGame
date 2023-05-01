using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public int score = 0;
    public float timeLeft = 60.0f; // The starting time in seconds
    public RoadGenerator roadGenerator;
    public TextMeshProUGUI timerText;
    public int speedGain = 5;
    int scoreIndex = 0;
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
        timeLeft += 1;
    }

    private void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = "Time: " + ((int)timeLeft).ToString();
        }
        else
        {
            roadGenerator.gameSpeed -= speedGain * scoreIndex;
            score = 0;
            timeLeft = 60.0f; // The starting time in seconds
            transform.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
            timerText.text = "Time: " + ((int)timeLeft).ToString();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public int score = 0;
    public void UpdateScore()
    {
        transform.GetComponent<TextMeshProUGUI>().text = "Score: " + score.ToString();
    }
}

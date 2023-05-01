using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class TimeOverScreenScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button timeOverScreenRestartButton;

    public RoadGenerator roadGenerator;
    public StartScreenScript startScreenScript;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI highestScoreTextMainUI;
    public Transform timeOverScreen;
    public TextMeshProUGUI timeOverHighestScoreText;
    public TextMeshProUGUI timeOverScoreText;
    public Button timeOverRestartButton;
    public Transform chunks;
    public ScoreHandler scoreHandler;
    public GameObject scoreUI;

    public Transform playerCamera;
    public PlayerController playerController;
    public Transform playerTransform;
    void Start()
    {
        timeOverScreenRestartButton.onClick.AddListener(RestartGame);
    }
     void RestartGame()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        gameObject.SetActive(false);
        scoreUI.SetActive(true);

        playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
        playerCamera.localRotation = Quaternion.Euler(0, 0, 0);
        playerController.currentMouseY = 0;
        playerController.isRestarting = false;
        scoreHandler.score = 0;
    }
}

using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenScript : MonoBehaviour
{
    public Button startButton;
    public Transform startScreen;
    public Transform startScreenCamera;
    public Transform startScreenCanvas;
    public Transform scoreUI;
    public Transform playerCamera;
    public RoadGenerator roadGenerator;
    public ScoreHandler scoreHandler;

    public PlayerController playerController;

    public Transform playerTransform;

    float cameraSpeed = 100f;
    bool starting = false;
    bool started = false;

    public int gameSpeed = 70;
    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        starting = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Does the camera movement in the start
        if(starting && Vector3.Distance(startScreenCamera.position, playerCamera.position) >= 5f)
        {
            startScreenCamera.position = Vector3.MoveTowards(startScreenCamera.position, playerCamera.position, cameraSpeed * Time.deltaTime);
        } else if(Vector3.Distance(startScreenCamera.position, playerCamera.position) < 5f && starting)
        {
            // Disable the start UI and enables the game main UI
            scoreUI.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(true);
            Camera mainCamera = Camera.main;
            mainCamera = playerCamera.GetComponent<Camera>();
            playerCamera.GetComponent<Camera>().enabled = true;
            startScreen.gameObject.SetActive(false);

            roadGenerator.gameSpeed = gameSpeed;

            // Restart camera and player, so they are in the center
            playerTransform.localRotation = Quaternion.Euler(0, 0, 0);
            playerCamera.localRotation = Quaternion.Euler(0, 0, 0);
            playerController.currentMouseY = 0;

            // Lock mouse
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;

            starting = false;
        }
    }
}

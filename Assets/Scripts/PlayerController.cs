using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
//using UnityEditor.Build.Player;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform playerBody;
    public Transform cameraTransform;
    public GameObject playerObject;
    public CharacterController controller;
    public Transform handTransform;


    public int playerSpeed = 100;
    public int leftSensitivity = 150;
    public int upSensitivity = 200;
    public float currentMouseY;

    public float jumpHeight = 35;
    public bool isGrounded = true;
    public float gravity = -39.81f;


    public Vector3 velocity = new Vector3();

    public Texture tex;

    public bool isRestarting = false;

    bool open = false;

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), tex);
    }


    void Update()
    {
        if (open == true && Input.anyKeyDown)
        {
            open = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
            isRestarting = !isRestarting;
        }
        // Mute music
        if (Input.GetKeyDown(KeyCode.M))
        {
            playerObject.GetComponent<AudioSource>().mute = !playerObject.GetComponent<AudioSource>().mute;
        }
        // Esc for unlocking mouse
        if (Input.GetKeyDown(KeyCode.Escape) && !open)
        {
            open = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            isRestarting = !isRestarting;
        } else if (Input.GetKeyDown(KeyCode.Escape) && open)
        {
            open = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            isRestarting = !isRestarting;
        }

        // Get the horizontal and vertical values (by default they are WASD)
        float horizontal = Input.GetAxis("Horizontal") * (playerSpeed / 2) * Time.deltaTime;
        controller.Move(new Vector3(horizontal, 0, 0));

        // Apply gravity and make jumps
        // Check if theres floor below the player
        int layersToIgnore = LayerMask.NameToLayer("Ignore");
        bool thereIsFloorBelowPlayer = Physics.Raycast(playerObject.transform.position, Vector3.down, 4.0f, layersToIgnore);
        if (Input.GetButtonDown("Jump") && thereIsFloorBelowPlayer)
        {
            velocity.y = jumpHeight;
        }
        else
        {
            velocity.y += gravity * (Time.deltaTime);
        }
        controller.Move(velocity * (Time.deltaTime * Mathf.Abs(gravity)));

        // Only move mouse if player is not on restart screen
        if (!isRestarting)
        {
            // Get mouse X and Y values
            float mouseX = (Input.GetAxis("Mouse X") * leftSensitivity) * Time.deltaTime;
            float mouseY = (Input.GetAxis("Mouse Y") * upSensitivity) * Time.deltaTime;
            currentMouseY -= mouseY;
            // Clamp camera Y rotation, so it cant be less than -90 and more than 90
            currentMouseY = Mathf.Clamp(currentMouseY, -40, 70);
            // Apply mouseY to camera and hand then apply mouseX to player object
            cameraTransform.localRotation = Quaternion.Euler(currentMouseY, 0, 0);
            handTransform.localRotation = Quaternion.Euler(currentMouseY, 0, 0);
            playerObject.transform.localRotation = Quaternion.Euler(playerObject.transform.eulerAngles + new Vector3(0, mouseX, 0));
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}

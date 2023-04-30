using System.Collections;
using System.Collections.Generic;
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
    float currentMouseY;

    public float jumpHeight = 35;
    public bool isGrounded = true;
    public float gravity = -39.81f;


    Vector3 velocity = new Vector3();

    public Texture tex;

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width / 2, Screen.height / 2, 10, 10), tex);
    }


    void Update()
    {
        // Lock mouse in the screen with K
        if (Input.GetKeyDown(KeyCode.K))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
        // Get the horizontal and vertical values (by default they are WASD)
        float horizontal = Input.GetAxis("Horizontal") * (playerSpeed / 2) * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * playerSpeed * Time.deltaTime;
        controller.Move(playerObject.transform.right * horizontal + playerObject.transform.forward * vertical);

        // Apply gravity and make jumps
        if (Input.GetButtonDown("Jump") && transform.position.y < 7f) // This verification for jumping is temporary
        {
            velocity.y = jumpHeight;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
        controller.Move(velocity * Time.deltaTime);



        // Get mouse X and Y values
        float mouseX = Input.GetAxis("Mouse X") * leftSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * upSensitivity * Time.deltaTime;
        currentMouseY -= mouseY;
        // Clamp camera Y rotation, so it cant be less than -90 and more than 90
        currentMouseY = Mathf.Clamp(currentMouseY, -90, 90);
        // Apply mouseY to camera and hand then apply mouseX to player object
        cameraTransform.localRotation = Quaternion.Euler(currentMouseY, 0, 0);
        handTransform.localRotation = Quaternion.Euler(currentMouseY, 0, 0);
        playerObject.transform.localRotation = Quaternion.Euler(playerObject.transform.eulerAngles + new Vector3(0, mouseX, 0));
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

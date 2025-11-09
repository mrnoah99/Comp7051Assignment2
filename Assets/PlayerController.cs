using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float xSens = 30;
    [SerializeField]
    private float ySens = 30;
    [SerializeField]
    private bool invertX = false;
    [SerializeField]
    private bool invertY = false;
    [SerializeField]
    private int moveSpeed = 25;
    [SerializeField]
    private GameObject winText;

    public Transform orientation;

    private float xRotation;
    private float yRotation;
    private Rigidbody rb;
    private InputActions inputActions;
    private InputAction movement;
    private InputAction lookMouse;
    private InputAction lookController;

    void Start()
    { // Lock cursor, get components relevant to player control.
        Cursor.lockState = CursorLockMode.Locked;
        inputActions = new();
        rb = GetComponent<Rigidbody>();
        movement = inputActions.Player.Move;
        movement.Enable();
        lookMouse = inputActions.Player.LookMouse;
        lookMouse.Enable();
        lookController = inputActions.Player.LookController;
        lookController.Enable();

        // Hide win text until the user wins the game.
        winText.SetActive(false);
    }

    void FixedUpdate()
    { // All of this code comes from the example done here: https://www.youtube.com/watch?v=f473C43s8nE
        // Handles player rotation controls
        Vector2 lookMouseVal = lookMouse.ReadValue<Vector2>();
        Vector2 lookControllerVal = lookController.ReadValue<Vector2>();

        float mouseX = lookMouseVal.x;
        if (Math.Abs(lookControllerVal.x) > Math.Abs(lookMouseVal.x)) mouseX = lookControllerVal.x * 10;
        mouseX = mouseX * Time.fixedDeltaTime * xSens * -1;


        float mouseY = lookMouseVal.y;
        if (Math.Abs(lookControllerVal.y) > Math.Abs(lookMouseVal.y)) mouseY = lookControllerVal.y * 10;
        mouseY = mouseY * Time.fixedDeltaTime * ySens;

        if (invertX) mouseX *= -1;
        if (invertY) mouseY *= -1;

        // Apply rotation to the player, locking the vertical from 90 deg down to 90 deg up.
        yRotation -= mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);

        // Handles movement input
        Vector2 input = movement.ReadValue<Vector2>();
        Vector3 moveDirection = input.y * moveSpeed * transform.forward + input.x * moveSpeed * transform.right;

        if (movement.enabled)
        { // Always active but will be turned off at some point so the check is used to stop the player from controlling when we don't want them to.
            rb.AddForce(moveDirection, ForceMode.Force);
        }
        // Limit the player's maximum velocity
        float lockedX = Mathf.Clamp(rb.linearVelocity.x, -1, 1);
        float lockedY = Mathf.Clamp(rb.linearVelocity.y, -1, 1);
        float lockedZ = Mathf.Clamp(rb.linearVelocity.z, -1, 1);
        rb.linearVelocity = new Vector3(lockedX, lockedY, lockedZ);
    }

    private void DisplayWinText()
    {
        winText.SetActive(true);
    }
    
    void OnTriggerEnter(Collider other)
    { // Used only for displaying the win text when the player exits the maze.
        if (other.gameObject.CompareTag("Finish"))
        {
            DisplayWinText();
        }
    }
}

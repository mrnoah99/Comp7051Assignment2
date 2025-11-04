using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float xSens = 30;
    public float ySens = 30;
    public bool invertX = false;
    public bool invertY = false;
    public int moveSpeed = 25;

    public Transform orientation;

    private float xRotation;
    private float yRotation;
    private Rigidbody rb;
    private InputActions inputActions;
    private InputAction movement;
    private InputAction look;

    void Start()
    { // Lock cursor, get components relevant to player control.
        Cursor.lockState = CursorLockMode.Locked;
        inputActions = new();
        rb = GetComponent<Rigidbody>();
        movement = inputActions.Player.Move;
        movement.Enable();
        look = inputActions.Player.Look;
        look.Enable();
    }

    void FixedUpdate()
    { // All of this code comes from the example done here: https://www.youtube.com/watch?v=f473C43s8nE
        // Handles player rotation controls
        float mouseX = look.ReadValue<Vector2>().x * Time.fixedDeltaTime * xSens * -1;
        float mouseY = look.ReadValue<Vector2>().y * Time.fixedDeltaTime * xSens;
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
        
        if (movement.enabled) { // Always active but will be turned off at some point so the check is used to stop the player from controlling when we don't want them to.
            rb.AddForce(moveDirection, ForceMode.Force);
        }
    }
}

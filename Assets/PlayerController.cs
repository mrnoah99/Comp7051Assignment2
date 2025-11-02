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
    {
        Cursor.lockState = CursorLockMode.Locked;
        inputActions = new();
        rb = GetComponent<Rigidbody>();
        movement = inputActions.Player.Move;
        movement.Enable();
        look = inputActions.Player.Look;
        look.Enable();
    }

    void FixedUpdate()
    {
        float mouseX = look.ReadValue<Vector2>().x * Time.fixedDeltaTime * xSens * -1;
        float mouseY = look.ReadValue<Vector2>().y * Time.fixedDeltaTime * xSens;
        if (invertX) mouseX *= -1;
        if (invertY) mouseY *= -1;
        yRotation -= mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        orientation.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        Vector2 input = movement.ReadValue<Vector2>();
        Vector3 moveDirection = transform.forward * input.y * moveSpeed + transform.right * input.x * moveSpeed;
        if (movement.enabled) {
            rb.AddForce(moveDirection, ForceMode.Force);
        }
    }
}

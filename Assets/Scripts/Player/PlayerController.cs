using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float lookSensitivity = 4f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Camera playerCamera;
    private Vector3 velocity;
    private float xRotation;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        SnapToTerrain();
    }

    private void SnapToTerrain()
    {
        Vector3 origin = transform.position + Vector3.up * 100f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 200f, LayerMask.GetMask("Environment"));
        float bestY = float.MinValue;
        foreach (var hit in hits)
        {
            if (hit.collider is TerrainCollider)
                bestY = Mathf.Max(bestY, hit.point.y);
        }
        if (bestY > float.MinValue)
        {
            Vector3 pos = transform.position;
            pos.y = bestY + 2f;
            controller.enabled = false;
            transform.position = pos;
            controller.enabled = true;
        }
    }

    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    private Vector2 GetMoveInput()
    {
        if (Keyboard.current != null)
        {
            Vector2 input = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            return Vector2.ClampMagnitude(input, 1f);
        }
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    private Vector2 GetMouseDelta()
    {
        if (Mouse.current != null)
        {
            Vector2 delta = Mouse.current.delta.ReadValue() * 0.1f;
            return delta;
        }
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private bool GetJumpDown()
    {
        if (Keyboard.current != null)
            return Keyboard.current.spaceKey.wasPressedThisFrame;
        return Input.GetButtonDown("Jump");
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = GetMouseDelta();
        float mouseX = mouseDelta.x * lookSensitivity;
        float mouseY = mouseDelta.y * lookSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = GetMoveInput();

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (GetJumpDown() && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

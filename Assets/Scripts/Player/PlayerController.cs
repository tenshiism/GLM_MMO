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
    private WeaponManager weaponManager;
    private PlayerStats playerStats;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
            playerCamera = Camera.main;

        weaponManager = GetComponent<WeaponManager>();
        playerStats = GetComponent<PlayerStats>();

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
        HandleCombat();
        HandleInteraction();
    }

    private void HandleCombat()
    {
        if (weaponManager == null) return;

        if (GetFireHeld() && weaponManager.ActiveWeapon != null)
        {
            Vector3 origin = playerCamera.transform.position;
            Vector3 direction = playerCamera.transform.forward;
            weaponManager.Fire(origin, direction);
        }

        if (GetReloadDown())
            weaponManager.Reload();

        float scroll = GetScrollDelta();
        if (scroll > 0f)
            weaponManager.SwitchNext();
        else if (scroll < 0f)
        {
            int prev = weaponManager.ActiveSlot - 1;
            if (prev < 0) prev = 2;
            weaponManager.SwitchToSlot(prev);
        }

        for (int i = 0; i < 3; i++)
        {
            if (GetSlotKeyDown(i))
                weaponManager.SwitchToSlot(i);
        }
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

    private bool GetFireHeld()
    {
        if (Mouse.current != null)
            return Mouse.current.leftButton.isPressed;
        return Input.GetButton("Fire1");
    }

    private bool GetReloadDown()
    {
        if (Keyboard.current != null)
            return Keyboard.current.rKey.wasPressedThisFrame;
        return Input.GetKeyDown(KeyCode.R);
    }

    private float GetScrollDelta()
    {
        if (Mouse.current != null)
            return Mouse.current.scroll.ReadValue().y;
        return Input.GetAxis("Mouse ScrollWheel");
    }

    private bool GetSlotKeyDown(int slot)
    {
        if (Keyboard.current == null) return false;
        return slot switch
        {
            0 => Keyboard.current.digit1Key.wasPressedThisFrame,
            1 => Keyboard.current.digit2Key.wasPressedThisFrame,
            2 => Keyboard.current.digit3Key.wasPressedThisFrame,
            _ => false
        };
    }

    private void HandleMouseLook()
    {
        Vector2 mouseDelta = GetMouseDelta();
        float sens = SettingsManager.mouseSensitivity;
        float mouseX = mouseDelta.x * sens;
        float mouseY = mouseDelta.y * sens;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = GetMoveInput();

        float speed = playerStats != null ? playerStats.MoveSpeed : moveSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (GetJumpDown() && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private bool GetInteractDown()
    {
        if (Keyboard.current != null)
            return Keyboard.current.eKey.wasPressedThisFrame;
        return Input.GetKeyDown(KeyCode.E);
    }

    private void HandleInteraction()
    {
        if (!GetInteractDown()) return;

        var interactable = GetComponentInChildren<Interactable>();
        if (interactable != null && interactable.playerInRange)
        {
            interactable.OnInteract(gameObject);
            return;
        }

        Interactable[] all = FindObjectsByType<Interactable>(FindObjectsSortMode.None);
        foreach (var i in all)
        {
            if (i.playerInRange)
            {
                i.OnInteract(gameObject);
                return;
            }
        }
    }
}

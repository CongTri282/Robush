using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RobotController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float pushForce = 8f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [Header("Energy")]
    public float maxEnergy = 100f;
    public float energyPerPush = 10f;
    public float currentEnergy;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private bool isSprinting = false;
    private bool isGrounded = false;
    private bool isPushing = false;

    private CameraController cameraController;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputActions = new PlayerInputActions();
        currentEnergy = maxEnergy;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Sprint.performed += ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => isSprinting = false;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cameraController = Camera.main.GetComponent<CameraController>();
    }

    void Update()
    {
        CheckGrounded();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        if (cameraController == null || moveInput == Vector2.zero) return;

        // Get movement relative to camera yaw
        Quaternion camYaw = cameraController.GetCameraYawRotation();
        Vector3 direction = camYaw * new Vector3(moveInput.x, 0f, moveInput.y);
        direction.Normalize();

        float speed = isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);

        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CheckGrounded()
    {
        // Cast ray from the bottom of the capsule
        Vector3 checkPos = transform.position + Vector3.down * 0.5f;
        float checkRadius = 0.25f;

        isGrounded = Physics.CheckSphere(checkPos, checkRadius, groundLayer);
        Debug.DrawRay(checkPos, Vector3.up * 0.5f, isGrounded ? Color.green : Color.red);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box") && currentEnergy >= energyPerPush)
        {
            if (moveInput.magnitude > 0 && !isPushing)
            {
                isPushing = true;

                Vector3 pushDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;
                Rigidbody boxRb = collision.rigidbody;
                if (boxRb != null)
                {
                    boxRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                    currentEnergy -= energyPerPush;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Box"))
        {
            isPushing = false;
        }
    }

    public void CollectBattery()
    {
        currentEnergy = maxEnergy;
    }
}
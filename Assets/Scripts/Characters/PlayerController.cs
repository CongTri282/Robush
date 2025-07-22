using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public CharacterController controller;
    [SerializeField] GameObject activeChar;
    [SerializeField] Vector3 playerVelocity;
    [SerializeField] float playerSpeed = 6f;
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float pushingSpeed = 2f;
    [SerializeField] bool isPushingCube = false;
    [SerializeField] bool isGrounded;
    [SerializeField] float gravityValue = -20f;
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] bool isJumping;

    [SerializeField] private InputActionAsset inputActionsAsset;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction pushAction;
    private InputAction walkAction;

    private EnergyCubeController attachedCube;

    void Start()
    {
        if (inputActionsAsset != null)
        {
            var gameplayMap = inputActionsAsset.FindActionMap("Gameplay", true);
            moveAction = gameplayMap.FindAction("Move", true);
            jumpAction = gameplayMap.FindAction("Jump", true);
            pushAction = gameplayMap.FindAction("Push", true);
            walkAction = gameplayMap.FindAction("Walk", true);
            moveAction.Enable();
            jumpAction.Enable();
            pushAction.Enable();
            walkAction.Enable();
        }

        controller.minMoveDistance = 0.001f;
        
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            isJumping = false;
        }

        // Handle push interaction
        if (isPushingCube)
        {
            // Detach if F pressed again
            if (pushAction != null && pushAction.triggered)
            {
                isPushingCube = false;
                if (attachedCube != null) attachedCube.Detach();
                attachedCube = null;
                activeChar.GetComponent<Animator>().Play("Idle");
                return;
            }

            // Only allow pushing forward (player's facing direction)
            Vector2 pushMoveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
            float pushAmount = pushMoveInput.y; // Only forward input
            if (pushAmount > 0f)
            {
                Vector3 pushDir = transform.forward * pushAmount;
                activeChar.GetComponent<Animator>().Play("Pushing");
                attachedCube.Push(pushDir.normalized, pushingSpeed);
                controller.Move(pushDir.normalized * pushingSpeed * Time.deltaTime);
            }
            else
            {
                activeChar.GetComponent<Animator>().Play("Push_Pose");
            }

            return;
        }

        // --- Normal movement code below ---
        Vector2 moveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        // Get camera reference
        Transform cam = Camera.main.transform;

        // Calculate movement relative to camera
        Vector3 camForward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = cam.right;
        Vector3 move = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // Set speed based on walk action
        float currentSpeed = (walkAction != null && walkAction.ReadValue<float>() > 0.5f) ? walkSpeed : playerSpeed;

        // Rotate player to face movement direction (if moving)
        if (move.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(move),
                Time.deltaTime * rotateSpeed
            );
        }

        // Jump input
        if (jumpAction != null && jumpAction.triggered && isGrounded)
        {
            isJumping = true;
            activeChar.GetComponent<Animator>().Play("Jump");
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;

        // Move horizontally and vertically (allows movement while jumping)
        Vector3 movement = move * currentSpeed;
        movement.y = playerVelocity.y;
        controller.Move(movement * Time.deltaTime);

        // Animation logic
        if (move.magnitude > 0.1f)
        {
            if (!isJumping)
            {
                activeChar.GetComponent<Animator>().Play(
                    (walkAction != null && walkAction.ReadValue<float>() > 0.5f) ? "Walking" : "Running"
                );
            }
        }
        else
        {
            if (!isJumping)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
            }
        }
    }

    // Called by EnergyCube
    public void AttachToCube(EnergyCubeController cube)
    {
        attachedCube = cube;
        isPushingCube = true;
        activeChar.GetComponent<Animator>().Play("Push_Pose");

        // Find the closest side of the cube to the player
        Vector3[] directions = {
            cube.transform.forward,    // front
            -cube.transform.forward,   // back
            cube.transform.right,      // right
            -cube.transform.right      // left
        };

        Vector3 playerToCube = (cube.transform.position - transform.position).normalized;
        float maxDot = float.MinValue;
        Vector3 bestDir = directions[0];

        foreach (var dir in directions)
        {
            float dot = Vector3.Dot(playerToCube, dir);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestDir = dir;
            }
        }

        // Snap player to that side
        float cubeExtent = Mathf.Max(cube.GetComponent<Collider>().bounds.extents.x, cube.GetComponent<Collider>().bounds.extents.z);
        Vector3 snapPosition = cube.transform.position - bestDir * (cubeExtent + controller.radius + 0.1f);
        snapPosition.y = transform.position.y; // Keep player's current height
        transform.position = snapPosition;

        // Face the cube
        transform.rotation = Quaternion.LookRotation(bestDir);
    }
}

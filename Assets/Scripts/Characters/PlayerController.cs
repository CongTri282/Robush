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
    private float pushAttachCooldown = 0.2f; // seconds
    private float pushAttachTimer = 0f;

    // Energy variables
    [SerializeField] float maxEnergy = 1f;
    [SerializeField] float currentEnergy = 1f;
    [SerializeField] float energyDrainPerSecond = 0.2f;
    [SerializeField] EnergyBarUI energyBarUI;

    // Looping SFX
    [Header("Looping SFX")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource pushSource;

    private bool isFootstepLooping = false;
    private bool isPushLooping = false;

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
        
        currentEnergy = maxEnergy;
        controller.minMoveDistance = 0.001f;

        energyBarUI = GameplayCanvasController.Instance?.energyBarUI.GetComponent<EnergyBarUI>();

        // Setup looping SFX sources if not assigned
        if (!footstepSource)
        {
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
        }
        if (!pushSource)
        {
            pushSource = gameObject.AddComponent<AudioSource>();
            pushSource.loop = true;
            pushSource.playOnAwake = false;
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (pushAttachTimer > 0f)
            pushAttachTimer -= Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
            if (isJumping)
            {
                SFXManager.Instance?.PlaySFX(SoundType.Landing);
            }
            isJumping = false;
        }

        // Handle push interaction
        if (isPushingCube)
        {
            // Stop footstep loop if entering push mode
            StopFootstepLoop();

            // Detach if F pressed again, but only if cooldown has elapsed
            if (pushAction != null && pushAction.triggered && pushAttachTimer <= 0f)
            {
                isPushingCube = false;
                if (attachedCube != null) attachedCube.Detach();
                attachedCube = null;
                activeChar.GetComponent<Animator>().Play("Idle");
                StopPushLoop();
                return;
            }

            // Only allow pushing if energy > 0
            if (currentEnergy > 0f)
            {
                Vector2 pushMoveInput = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
                float pushAmount = pushMoveInput.y; // Only forward input
                if (pushAmount > 0f)
                {
                    Vector3 pushDir = transform.forward * pushAmount;
                    activeChar.GetComponent<Animator>().Play("Pushing");
                    attachedCube.Push(pushDir.normalized, pushingSpeed);
                    controller.Move(pushDir.normalized * pushingSpeed * Time.deltaTime);
                    StartPushLoop();

                    // Drain energy while pushing
                    currentEnergy = Mathf.Max(0f, currentEnergy - energyDrainPerSecond * Time.deltaTime);
                    if (energyBarUI) energyBarUI.SetEnergy(currentEnergy / maxEnergy);

                    // If energy runs out, stop pushing
                    if (currentEnergy <= 0f)
                    {
                        isPushingCube = false;
                        if (attachedCube != null) attachedCube.Detach();
                        attachedCube = null;
                        activeChar.GetComponent<Animator>().Play("Idle");
                        StopPushLoop();
                        return;
                    }
                }
                else
                {
                    activeChar.GetComponent<Animator>().Play("Push_Pose");
                    StopPushLoop();
                }
            }
            else
            {
                // No energy, can't push
                activeChar.GetComponent<Animator>().Play("Push_Pose");
                StopPushLoop();
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
                bool isWalking = (walkAction != null && walkAction.ReadValue<float>() > 0.5f);
                activeChar.GetComponent<Animator>().Play(isWalking ? "Walking" : "Running");
                StartFootstepLoop(isWalking, isPushingCube);
            }
            else
            {
                StopFootstepLoop(); // Stop footsteps while jumping
            }
        }
        else
        {
            if (!isJumping)
            {
                activeChar.GetComponent<Animator>().Play("Idle");
            }
            StopFootstepLoop();
        }

        // --- Looping SFX helpers ---
        void StartFootstepLoop(bool isWalking, bool isPushing)
        {
            float pitch = 1.0f;
            if (isWalking)
                pitch = 0.7f;

            if (!isFootstepLooping)
            {
                var group = SFXManager.Instance?.soundsSO.GetSoundGroup(SoundType.Footstep);
                if (group != null && group.sounds.Length > 0)
                {
                    footstepSource.clip = group.sounds[Random.Range(0, group.sounds.Length)];
                    footstepSource.volume = group.volume;
                    footstepSource.loop = true;
                    footstepSource.pitch = pitch;
                    footstepSource.Play();
                    isFootstepLooping = true;
                }
            }
            else
            {
                // Adjust pitch if already looping
                footstepSource.pitch = pitch;
            }
        }
        void StopFootstepLoop()
        {
            if (isFootstepLooping)
            {
                footstepSource.Stop();
                isFootstepLooping = false;
            }
        }
        void StartPushLoop()
        {
            if (!isPushLooping)
            {
                var group = SFXManager.Instance?.soundsSO.GetSoundGroup(SoundType.CubePush);
                if (group != null && group.sounds.Length > 0)
                {
                    pushSource.clip = group.sounds[Random.Range(0, group.sounds.Length)];
                    pushSource.volume = group.volume;
                    pushSource.loop = true;
                    pushSource.Play();
                    isPushLooping = true;
                }
            }
        }
        void StopPushLoop()
        {
            if (isPushLooping)
            {
                pushSource.Stop();
                isPushLooping = false;
            }
        }
    }

    // Called by EnergyCube
    public void AttachToCube(EnergyCubeController cube)
    {
        attachedCube = cube;
        isPushingCube = true;
        activeChar.GetComponent<Animator>().Play("Push_Pose");
        pushAttachTimer = pushAttachCooldown;

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

    public void AddEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, maxEnergy);
        if (energyBarUI) energyBarUI.SetEnergy(currentEnergy / maxEnergy);
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        bool enableInput = (newState == GameManager.GameState.Playing);
        SetMovementInputEnabled(enableInput);
    }

    private void SetMovementInputEnabled(bool enabled)
    {
        if (enabled)
        {
            moveAction?.Enable();
            jumpAction?.Enable();
            pushAction?.Enable();
            walkAction?.Enable();
        }
        else
        {
            moveAction?.Disable();
            jumpAction?.Disable();
            pushAction?.Disable();
            walkAction?.Disable();
        }
    }

    public static void SetAllSFXMute(bool isMuted)
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            if (player.footstepSource != null)
                player.footstepSource.mute = isMuted;
            if (player.pushSource != null)
                player.pushSource.mute = isMuted;
        }
    }
}

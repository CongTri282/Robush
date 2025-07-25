using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float distance = 3f;
    [SerializeField] float height = 2f;
    [SerializeField] float mouseSensitivity = 0.2f;

    [SerializeField] private InputActionAsset inputActionsAsset;
    private InputAction lookAction;

    float yaw = 0f;
    float pitch = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (inputActionsAsset != null)
        {
            var gameplayMap = inputActionsAsset.FindActionMap("Gameplay", true);
            lookAction = gameplayMap.FindAction("Look", true);
            lookAction.Enable();
        }
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
        if (lookAction != null)
        {
            if (enableInput)
                lookAction.Enable();
            else
                lookAction.Disable();
        }

        if (enableInput)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void LateUpdate()
    {
        Vector2 lookDelta = lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
        yaw += lookDelta.x * mouseSensitivity;
        pitch -= lookDelta.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance) + Vector3.up * height;
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }
}
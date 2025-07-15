using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, 0f);
    public float distance = 6f;

    [Header("Rotation")]
    public float rotationSpeed = 0.2f;
    public float minYAngle = 5f;
    public float maxYAngle = 60f;

    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Mouse movement (Input System)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * rotationSpeed;
        pitch -= mouseDelta.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        // Rotate and position the camera
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 cameraOffset = rotation * new Vector3(0, 0, -distance);
        transform.position = target.position + offset + cameraOffset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    public Quaternion GetCameraYawRotation()
    {
        return Quaternion.Euler(0f, yaw, 0f);
    }
}
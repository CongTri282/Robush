using UnityEngine;

public class Box : MonoBehaviour
{
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void ResetPosition()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = initialPosition;
        transform.rotation = initialRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoalZone"))
        {
            GameManager.Instance.CompleteLevel();
        }
    }
}
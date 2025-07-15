using UnityEngine;

public class EnergyBattery : MonoBehaviour
{
    public float energyRestoreAmount = 100f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isCollected = false;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
        gameObject.SetActive(true);
        isCollected = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
            RobotController robot = other.GetComponent<RobotController>();
            if (robot != null)
            {
                robot.CollectBattery();
                isCollected = true;
                gameObject.SetActive(false);
            }
        }
    }

    public void ResetBattery()
    {
        isCollected = false;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        gameObject.SetActive(true);
    }
}
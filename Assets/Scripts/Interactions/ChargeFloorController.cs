using UnityEngine;

public class ChargeFloorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Renderer floorRenderer;
    [SerializeField] private Color chargedColor = Color.green;
    [SerializeField] private Color defaultColor = Color.gray;
    [SerializeField] private GameObject elevator;
    [SerializeField] private GameObject elevatorSwitch;
    private bool isCharged = false;

    void Start()
    {
        if (floorRenderer == null)
            floorRenderer = GetComponent<Renderer>();
        SetFloorColor(defaultColor);

    }

    void OnTriggerEnter(Collider other)
    {
        EnergyCubeController cube = other.GetComponent<EnergyCubeController>();
        if (cube != null && !isCharged)
        {
            isCharged = true;
            SetFloorColor(chargedColor);
            SFXManager.Instance?.PlaySFX(SoundType.ChargeOn);
            SFXManager.Instance?.PlaySFX(SoundType.ElevatorOpen);
            StartCoroutine(OpenElevatorAfterDelay());
        }
    }

    private System.Collections.IEnumerator OpenElevatorAfterDelay()
    {
        yield return new WaitForSeconds(2f); // Wait for sound to play
        if (elevator != null)
        {
            var anim = elevator.GetComponent<Animator>();
            if (anim != null)
                anim.Play("OpenElevator");
        }
        if (elevatorSwitch != null)
        {
            elevatorSwitch.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        EnergyCubeController cube = other.GetComponent<EnergyCubeController>();
        if (cube != null && isCharged)
        {
            isCharged = false;
            SetFloorColor(defaultColor);
            SFXManager.Instance?.PlaySFX(SoundType.ElevatorOpen);
            if (elevator != null)
            {
                var anim = elevator.GetComponent<Animator>();
                if (anim != null)
                    anim.Play("CloseElevator");
            }
        }
    }

    private void SetFloorColor(Color color)
    {
        if (floorRenderer != null)
        {
            floorRenderer.material.color = color;
        }
    }
}

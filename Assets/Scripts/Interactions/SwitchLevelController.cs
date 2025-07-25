using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;

public class SwitchLevelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string nextLevelName;
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private GameObject elevator;
    [SerializeField] private GameObject infoUI;
    [SerializeField] private InputActionAsset inputActionsAsset;

    private InputAction interactAction;
    private bool isPlayerNearby = false;
    private PlayerController player;
    private bool allowSwitch = false;

    private float switchCooldown = 4f; // seconds
    private float cooldownTimer = 0f;

    void Start()
    {
        if (inputActionsAsset != null)
        {
            var gameplayMap = inputActionsAsset.FindActionMap("Gameplay", true);
            interactAction = gameplayMap.FindAction("Interact", true);
            interactAction.Disable(); // Disable until cooldown is over
        }

        infoUI = GameplayCanvasController.Instance?.infoUI;

        cooldownTimer = switchCooldown;
        allowSwitch = false;
    }

    void Update()
    {
        // Handle cooldown
        if (!allowSwitch)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                allowSwitch = true;
                interactAction?.Enable();
            }
        }

        // Player lookup
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj) player = playerObj.GetComponent<PlayerController>();
        }

        if (player == null || interactAction == null || !allowSwitch) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        isPlayerNearby = dist < interactionDistance;

        if (infoUI)
        {
            infoUI.GetComponent<TMPro.TMP_Text>().text = isPlayerNearby ? "Press E to interact" : "";
            infoUI.SetActive(isPlayerNearby);
        }

        if (isPlayerNearby && interactAction != null && interactAction.triggered)
        {
            SFXManager.Instance?.PlaySFX(SoundType.Switch);
            cooldownTimer = switchCooldown;
            allowSwitch = false;
            interactAction.Disable();
            if (infoUI) infoUI.SetActive(false);
            StartCoroutine(SwitchLevel());
        }
    }

    private IEnumerator SwitchLevel()
    {
        // Close elevator animation
        if (elevator != null)
        {
            SFXManager.Instance?.PlaySFX(SoundType.ElevatorOpen);
            yield return new WaitForSeconds(2f); // Wait for sound to play
            var anim = elevator.GetComponent<Animator>();
            if (anim != null)
                anim.Play("CloseElevator");
        }
        // Wait for elevator animation to finish
        yield return new WaitForSeconds(2f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadLevel(nextLevelName, 1);
        }
        else
        {
            // Fallback: load scene directly if GameManager is missing
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
        }
    }
}
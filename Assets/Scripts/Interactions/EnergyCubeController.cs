using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EnergyCubeController : MonoBehaviour
{
    [SerializeField] float interactDistance = 2f;
    [SerializeField] GameObject infoUI;
    [SerializeField] private InputActionAsset inputActionsAsset;
    private InputAction pushAction;
    private PlayerController player;
    [SerializeField] bool isPlayerNearby = false;
    private bool isBeingPushed = false;
    [SerializeField] float detachCooldown = 0.2f;
    private float detachTimer = 0f;

    void Start()
    {
        if (inputActionsAsset != null)
        {
            var gameplayMap = inputActionsAsset.FindActionMap("Gameplay", true);
            pushAction = gameplayMap.FindAction("Push", true);
            pushAction.Enable();
        }
    }

    void Update()
    {
        if (detachTimer > 0f)
            detachTimer -= Time.deltaTime;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj) player = playerObj.GetComponent<PlayerController>();
        }

        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        isPlayerNearby = dist < interactDistance && !isBeingPushed;

        // Check if player is on top of the cube
        float cubeTopY = transform.position.y + GetComponent<Collider>().bounds.extents.y;
        float playerFeetY = player.transform.position.y - player.controller.height / 2f + player.controller.radius;
        bool playerOnTop = playerFeetY > cubeTopY - 0.1f;

        if (infoUI)
        {
            // Only show infoUI if player is NOT on top
            bool showInfo = isPlayerNearby && !playerOnTop;
            infoUI.GetComponent<TMPro.TMP_Text>().text = showInfo ? "Press F to push the cube" : "";
            infoUI.SetActive(showInfo);
        }

        // Only attach if not being pushed, not on top, and cooldown finished
        if (isPlayerNearby && !isBeingPushed && detachTimer <= 0f && !playerOnTop)
        {
            if (pushAction != null && pushAction.triggered)
            {
                isBeingPushed = true;
                player.AttachToCube(this);
                if (infoUI) infoUI.SetActive(false);
            }
        }
    }

    public void Detach()
    {
        isBeingPushed = false;
        detachTimer = detachCooldown; // Start cooldown after detach
        // Optionally show info UI again if player is still nearby
        float dist = Vector3.Distance(transform.position, player.transform.position);
        isPlayerNearby = dist < interactDistance;
        if (infoUI)
        {
            infoUI.GetComponent<TMPro.TMP_Text>().text = isPlayerNearby ? "Press F to push the cube" : "";
            infoUI.SetActive(isPlayerNearby);
        }
    }

    // Called by PlayerController when pushing
    public void Push(Vector3 direction, float speed)
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
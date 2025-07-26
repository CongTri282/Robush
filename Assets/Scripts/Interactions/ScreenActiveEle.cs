using UnityEngine;
using UnityEngine.InputSystem;


public class ScreenActiveEle : MonoBehaviour
{

    [SerializeField] float interactDistance = 2f;
    [SerializeField] private GameObject eleUp;
    [SerializeField] private InputActionAsset inputActionsAsset;
    [SerializeField] private GameObject infoUI;
    private InputAction interactAction;
    private PlayerController player;
    [SerializeField] bool isPlayerNearby = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (inputActionsAsset != null)
        {
            var gameplayMap = inputActionsAsset.FindActionMap("Gameplay", true);
            interactAction = gameplayMap.FindAction("Interact", true);
            interactAction.Enable();
        }
        infoUI = GameplayCanvasController.Instance?.infoUI;

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj) player = playerObj.GetComponent<PlayerController>();
        }

        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        isPlayerNearby = dist < interactDistance;

        if (infoUI)
        {
            infoUI.GetComponent<TMPro.TMP_Text>().text = isPlayerNearby ? "Press E to interact" : "";
            infoUI.SetActive(isPlayerNearby);
        }

        if (isPlayerNearby && interactAction != null && interactAction.triggered)
        {
            SFXManager.Instance?.PlaySFX(SoundType.Switch);
            interactAction.Disable();
            if (infoUI) infoUI.SetActive(false);
            if (eleUp != null)
            {
                var anim = eleUp.GetComponent<Animator>();
                if (anim != null)
                    anim.Play("EleUp");
            }

        }
    }
}

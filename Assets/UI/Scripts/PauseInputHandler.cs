using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionReference pauseActionReference;

    private InputAction pauseAction;

    private void Awake()
    {
        // If using Input Action Reference from an Input Action Asset
        if (pauseActionReference != null)
        {
            pauseAction = pauseActionReference.action;
        }
        else
        {
            // Create action programmatically
            pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");
        }
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        pauseAction.performed -= OnPausePerformed;
        pauseAction.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        // Only toggle pause when in Playing or Paused states
        if (GameManager.Instance != null)
        {
            var currentState = GameManager.Instance.CurrentState;
            if (currentState == GameManager.GameState.Playing ||
                currentState == GameManager.GameState.Paused)
            {
                GameManager.Instance.TogglePause();
            }
        }
    }
}
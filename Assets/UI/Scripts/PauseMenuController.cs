using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button resetLevelButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel; // Optional options panel
    [SerializeField] private Button backButton; // Add this for OptionsPanel "Back"

    private void Awake()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        SetupButtons();
    }

    private void OnEnable()
    {
        GameManager.Instance.onGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChanged -= OnGameStateChanged;
    }

    private void SetupButtons()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);

        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);

        if (resetLevelButton != null)
            resetLevelButton.onClick.AddListener(OnResetLevelClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(newState == GameManager.GameState.Paused);

            // Hide options panel when unpausing
            if (newState != GameManager.GameState.Paused && optionsPanel != null)
            {
                optionsPanel.SetActive(false);
            }
        }
    }

    private void OnContinueClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        GameManager.Instance.ResumeGame();
    }

    private void OnOptionsClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }


    private void OnResetLevelClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);

        GameManager.Instance.ResetLevel();
    }

    private void OnMainMenuClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);

        GameManager.Instance.LoadMainMenu();
    }

    private void OnQuitClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        // Could show a confirmation dialog here

        GameManager.Instance.QuitGame();
    }
    private void OnBackClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
    }

}
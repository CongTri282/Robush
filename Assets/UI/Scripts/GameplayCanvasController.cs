using UnityEngine;
using System.Collections;

public class GameplayCanvasController : MonoBehaviour
{
    public static GameplayCanvasController Instance { get; private set; }

    [SerializeField] private GameObject gameplayUI;
    public GameObject infoUI;
    public GameObject energyBarUI;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Listen for game state changes
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChanged += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameManager.GameState newState)
    {
        // Disable GameplayUI when entering MainMenu
        if (newState == GameManager.GameState.MainMenu && gameplayUI != null)
        {
            gameplayUI.SetActive(false);
        }

        // Enable GameplayUI when entering Playing state (if already present), or instantiate if missing
        if (newState == GameManager.GameState.Playing)
        {
            if (gameplayUI != null)
            {
                StartCoroutine(EnableGameplayUIDelayed());
            }

            if (energyBarUI != null)
            {
                var bar = energyBarUI.GetComponent<EnergyBarUI>();
                if (bar != null)
                    bar.SetEnergy(1f); // Set to full
            }
        }
    }

    private IEnumerator EnableGameplayUIDelayed()
    {
        yield return new WaitForSeconds(3f);
        gameplayUI.SetActive(true);
    }
}
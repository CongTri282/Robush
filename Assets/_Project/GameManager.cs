using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused }
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    // Event for state changes
    public System.Action<GameState> onGameStateChanged;

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
        }
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        var previousState = CurrentState;
        CurrentState = newState;

        // Notify listeners
        onGameStateChanged?.Invoke(CurrentState);

        switch (CurrentState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                if (previousState == GameState.Paused)
                {
                    if (BGMManager.Instance != null)
                    {
                        BGMManager.Instance.ResumeBGM();
                    }
                }
                else
                {
                    // Load the main gameplay scene if not already loaded
                    if (SceneManager.GetActiveScene().name != "DevScene" && previousState == GameState.MainMenu)
                    {
                        StartCoroutine(FadeAndLoadScene("DevScene", 1));
                    }
                }
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                if (BGMManager.Instance != null)
                {
                    BGMManager.Instance.PauseBGM();
                }
                break;
        }
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (CurrentState == GameState.Paused)
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeAndLoadScene(SceneManager.GetActiveScene().name));
        SetState(GameState.Playing);
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void LoadLevel(string levelName, int bgmTrack)
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeAndLoadScene(levelName, bgmTrack));
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeAndLoadScene("MainMenu"));
        SetState(GameState.MainMenu);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    private IEnumerator FadeAndLoadScene(string sceneName, int bgmTrack = -1)
    {
        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeOut();

        SceneManager.LoadScene(sceneName);
        yield return null;

        if (bgmTrack != -1 && BGMManager.Instance != null)
            BGMManager.Instance.PlayBGM(bgmTrack);

        if (ScreenFader.Instance != null)
            yield return ScreenFader.Instance.FadeIn();
    }
}
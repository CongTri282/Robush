using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused }
    public GameState CurrentState { get; private set; } = GameState.MainMenu;

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
        CurrentState = newState;
        // Add logic for entering new state here (UI, audio, etc.)
        switch (CurrentState)
        {
            case GameState.MainMenu:
                // Show main menu UI
                break;
            case GameState.Playing:
                // Hide menus, start/resume gameplay
                // Load the main gameplay scene if not already loaded
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "DevScene")
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("DevScene");
                }
                // Play BGM track 1
                if (BGMManager.Instance != null)
                {
                    BGMManager.Instance.PlayBGM(1);
                }
                break;
            case GameState.Paused:
                // Show pause UI, stop time
                Time.timeScale = 0f;
                break;
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            Time.timeScale = 1f;
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }
}

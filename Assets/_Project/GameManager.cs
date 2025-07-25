using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, Paused, ResumeGame }
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
        switch (CurrentState)
        {
            case GameState.MainMenu:
                // Show main menu UI
                break;
            case GameState.Playing:
                // Hide menus, start/resume gameplay
                // Load the main gameplay scene if not already loaded
                if (SceneManager.GetActiveScene().name != "DevScene")
                {
                    StartCoroutine(FadeAndLoadScene("DevScene"));
                }
                // Play BGM track 1
                if (BGMManager.Instance != null)
                {
                    BGMManager.Instance.PlayBGM(1);
                }
                break;
            case GameState.ResumeGame:
                // Resume game logic
                break;
            case GameState.Paused:
                // Show pause UI, stop time
                Time.timeScale = 0f;
                break;
        }
    }

    public void ResumeGame()
    {
        // if (CurrentState == GameState.Paused)
        // {
        //     Time.timeScale = 1f;
        //     // Resume gameplay
        //     SetState(GameState.Playing, SceneManager.GetActiveScene().name, 1);
        // }
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
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Playing);
    }
    public void LoadLevel(string levelName, int bgmTrack)
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeAndLoadScene(levelName));
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.PlayBGM(bgmTrack);
        }
    }

    public void LoadMainMenu()
    {
        StartCoroutine(FadeAndLoadScene("MainMenu"));
        SetState(GameState.MainMenu);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeOut();
        }
        SceneManager.LoadScene(sceneName);
        yield return null; // Wait one frame for scene load
        if (ScreenFader.Instance != null)
        {
            yield return ScreenFader.Instance.FadeIn();
        }
    }
}

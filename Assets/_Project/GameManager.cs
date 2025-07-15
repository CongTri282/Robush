using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public RobotController robot;
    public Box box;
    public EnergyBattery battery;

    [Header("UI")]
    public Slider energySlider;
    public Text energyText;
    public Text levelText;
    public Text statusText;

    private PlayerInputActions inputActions;
    private int currentLevel = 1;
    private bool levelComplete = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            inputActions = new PlayerInputActions();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Restart.performed += OnRestart;
        inputActions.Player.NextLevel.performed += OnNextLevel;
    }

    void OnDisable()
    {
        inputActions.Player.Restart.performed -= OnRestart;
        inputActions.Player.NextLevel.performed -= OnNextLevel;
        inputActions.Player.Disable();
    }

    void Start()
    {
        StartLevel();
    }

    void Update()
    {
        UpdateUI();
        CheckGameOver();
    }

    void UpdateUI()
    {
        if (energySlider != null)
        {
            energySlider.value = robot.currentEnergy / robot.maxEnergy;
        }

        if (energyText != null)
        {
            energyText.text = $"Energy: {robot.currentEnergy:F0}/{robot.maxEnergy:F0}";
        }

        if (levelText != null)
        {
            levelText.text = $"Level: {currentLevel}";
        }
    }

    void CheckGameOver()
    {
        if (robot.currentEnergy <= 0 && !levelComplete)
        {
            statusText.text = "Game Over! Press R to Restart";
        }
    }

    void OnRestart(InputAction.CallbackContext ctx)
    {
        if (!levelComplete)
        {
            RestartLevel();
        }
    }

    void OnNextLevel(InputAction.CallbackContext ctx)
    {
        if (levelComplete)
        {
            NextLevel();
        }
    }

    public void CompleteLevel()
    {
        levelComplete = true;
        statusText.text = "Level Complete! Press N for Next Level";
    }

    void StartLevel()
    {
        levelComplete = false;
        robot.currentEnergy = robot.maxEnergy;
        box.ResetPosition();
        battery.ResetBattery();
        statusText.text = "Push the box to the goal!";
    }

    void RestartLevel()
    {
        StartLevel();
    }

    void NextLevel()
    {
        currentLevel++;
        StartLevel();
        // Add level loading logic here (e.g., load from scene list or prefab setups)
    }
}

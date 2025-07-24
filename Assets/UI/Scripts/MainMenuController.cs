using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button bgmToggleButton;

    [Header("BGM Sprites")]
    [SerializeField] private Sprite bgmOnSprite;
    [SerializeField] private Sprite bgmOffSprite;

    private void Start()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
        if (optionsButton != null)
            optionsButton.onClick.AddListener(OnOptionsClicked);
        if (aboutButton != null)
            aboutButton.onClick.AddListener(OnAboutClicked);
        if (bgmToggleButton != null)
            bgmToggleButton.onClick.AddListener(OnBGMToggleClicked);

        // Ensure BGM state matches saved preference
        bool isBGMOn = PlayerPrefs.GetInt("BGMEnabled", 1) != 0;
        SetBGMEnabled(isBGMOn);
        UpdateBGMToggleSprite();
    }

    public void OnPlayClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
        else
        {
            // Fallback: load scene directly if GameManager is missing
            SceneManager.LoadScene("DevScene");
        }
    }

    public void OnQuitClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        Application.Quit();
    }

    public void OnOptionsClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        // Show options UI or logic here
    }
    public void OnAboutClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        // Show about UI or logic here
    }

    private void OnBGMToggleClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        bool isBGMOn = PlayerPrefs.GetInt("BGMEnabled", 1) != 0;
        isBGMOn = !isBGMOn;
        PlayerPrefs.SetInt("BGMEnabled", isBGMOn ? 1 : 0);
        SetBGMEnabled(isBGMOn);
        UpdateBGMToggleSprite();
    }

    private void UpdateBGMToggleSprite()
    {
        if (bgmToggleButton != null)
        {
            bool isBGMOn = PlayerPrefs.GetInt("BGMEnabled", 1) != 0;
            var image = bgmToggleButton.GetComponent<Image>();
            if (image != null)
                image.sprite = isBGMOn ? bgmOnSprite : bgmOffSprite;
        }
    }

    public void SetBGMEnabled(bool enabled)
    {
        if (BGMManager.Instance != null && BGMManager.Instance.AudioSource != null)
        {
            BGMManager.Instance.AudioSource.mute = !enabled;
            if (enabled)
            {
                // Always play track 0 when enabling BGM
                BGMManager.Instance.PlayBGM(0);
            }
            else
            {
                BGMManager.Instance.StopBGM();
            }
        }
    }
}

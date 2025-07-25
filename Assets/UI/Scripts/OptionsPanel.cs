using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : MonoBehaviour
{
    [Header("Audio Buttons")]
    [SerializeField] private Button bgmButton;
    [SerializeField] private Button sfxButton;

    [Header("BGM Sprites")]
    [SerializeField] private Sprite bgmOnSprite;
    [SerializeField] private Sprite bgmOffSprite;

    [Header("SFX Sprites")]
    [SerializeField] private Sprite sfxOnSprite;
    [SerializeField] private Sprite sfxOffSprite;

    private bool bgmOn;
    private bool sfxOn;

    private void Start()
    {
        // Load saved preferences (default to true if not set)
        bgmOn = PlayerPrefs.GetInt("BGM", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;

        if (bgmButton != null)
        {
            bgmButton.onClick.AddListener(OnBGMButtonClicked);
            UpdateBGMSprite();
        }

        if (sfxButton != null)
        {
            sfxButton.onClick.AddListener(OnSFXButtonClicked);
            UpdateSFXSprite();
        }

        ApplyAudioSettings();
    }

    private void OnBGMButtonClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        bgmOn = !bgmOn;
        PlayerPrefs.SetInt("BGM", bgmOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateBGMSprite();
        ApplyAudioSettings();
    }

    private void OnSFXButtonClicked()
    {
        SFXManager.Instance?.PlaySFX(SoundType.ButtonClick);
        sfxOn = !sfxOn;
        PlayerPrefs.SetInt("SFX", sfxOn ? 1 : 0);
        PlayerPrefs.Save();
        UpdateSFXSprite();
        ApplyAudioSettings();
    }

    private void ApplyAudioSettings()
    {
        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.SetMute(!bgmOn);
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.SetMute(!sfxOn);
        }
    }

    private void UpdateBGMSprite()
    {
        if (bgmButton != null)
        {
            var image = bgmButton.GetComponent<Image>();
            if (image != null)
                image.sprite = bgmOn ? bgmOnSprite : bgmOffSprite;
        }
    }

    private void UpdateSFXSprite()
    {
        if (sfxButton != null)
        {
            var image = sfxButton.GetComponent<Image>();
            if (image != null)
                image.sprite = sfxOn ? sfxOnSprite : sfxOffSprite;
        }
    }
}

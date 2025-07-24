using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 3f;

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
        fadeImage.gameObject.SetActive(true);
    }
    public IEnumerator FadeOut()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            SetAlpha(t / fadeDuration);
            yield return null;
        }
        SetAlpha(1f);
    }

    public IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0f)
        {
            t -= Time.deltaTime;
            SetAlpha(t / fadeDuration);
            yield return null;
        }
        SetAlpha(0f);
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = Mathf.Clamp01(alpha);
        fadeImage.color = c;
    }
}

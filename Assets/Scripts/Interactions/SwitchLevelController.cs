using UnityEngine;

public class SwitchLevelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject elevator;
    [SerializeField] private string closeElevatorAnim = "CloseElevator";
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private string nextSceneName = "NextLevel";

    private bool isSwitching = false;

    public void TriggerSwitch()
    {
        if (isSwitching) return;
        isSwitching = true;
        // Close elevator
        if (elevator != null)
        {
            var anim = elevator.GetComponent<Animator>();
            if (anim != null)
                anim.Play(closeElevatorAnim);
        }
        // Start fade and switch
        StartCoroutine(FadeAndSwitch());
    }

    private System.Collections.IEnumerator FadeAndSwitch()
    {
        if (fadeCanvas != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
                yield return null;
            }
            fadeCanvas.alpha = 1f;
        }
        // Switch to next scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}

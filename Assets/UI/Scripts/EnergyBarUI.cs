using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    [SerializeField] Image fillImage; // Assign the fill image in the inspector

    public void SetEnergy(float value)
    {
        fillImage.fillAmount = Mathf.Clamp01(value);
    }
}
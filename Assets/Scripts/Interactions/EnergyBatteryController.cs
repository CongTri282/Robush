using UnityEngine;

public class EnergyBatteryController : MonoBehaviour
{
    [SerializeField] float energyAmount = 1f; // Amount to fill (1 = full bar)

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            SFXManager.Instance.PlaySFX(SoundType.EnergyPickup);
            player.AddEnergy(energyAmount);
            Destroy(gameObject);
        }
    }
}

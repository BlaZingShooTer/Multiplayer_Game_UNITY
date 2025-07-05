using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplayScript : NetworkBehaviour
{

    [Header("Health Display Settings")]

    [SerializeField] private Health health;

    [SerializeField] private Image healthBarImage;

    public override void OnNetworkSpawn()
    {
        if(!IsClient) { return; }
        health.CurrentHealth.OnValueChanged += HandleHealthChanged;
        HandleHealthChanged(0, health.CurrentHealth.Value); // Initialize the health bar on spawn
    }


    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }

        health.CurrentHealth.OnValueChanged -= HandleHealthChanged;
        
    }

    private void HandleHealthChanged(int oldHealthValue, int newHealthValue)
    {
        healthBarImage.fillAmount = (float)newHealthValue / health.MaxHealth;
    }
}

using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [field:SerializeField] public int MaxHealth { get; private set; } = 100;

    public NetworkVariable<int> CurrentHealth  = new NetworkVariable<int>();

    private bool isDead;

    public Action<Health> onDie;
    public override void OnNetworkSpawn()
    {
        if(!IsServer)
        {
            // Only the server should initialize health
            return;
        }

        CurrentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damageValue) 
    {
        ModifyHealth(damageValue * -1);
    }

    public void RestoreHealth(int healValue) 
    {
        ModifyHealth(healValue);
    }

    private void ModifyHealth(int value) 
    {
        if (!isDead) 
        {
            CurrentHealth.Value = Mathf.Clamp(CurrentHealth.Value + value, 0, MaxHealth);
            if (CurrentHealth.Value == 0) 
            {
                onDie?.Invoke(this);
                isDead = true;
            }
        }
    }
}

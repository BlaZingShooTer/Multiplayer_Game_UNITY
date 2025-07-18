using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealingZone : NetworkBehaviour
{
    [SerializeField] private Image healPowerBar;


    [SerializeField] private int maxHealPower = 30;

    [SerializeField] private float healCooldown = 60f;

    [SerializeField] private int coinsPerTick = 10;

    [SerializeField] private float healTickRate = 1f;

    [SerializeField] private int healthPerTick = 10;


    private List<TankPlayer> playerInZone = new List<TankPlayer>();



    private NetworkVariable<int> HealPower = new NetworkVariable<int>();


    private float remainingCoolDown;

    private float tickTimer;


    public override void OnNetworkSpawn()
    {
        if (IsClient) 
        {
            HealPower.OnValueChanged += HandleHealPowerChanged;

            HandleHealPowerChanged(0, HealPower.Value);
        }

        if (IsServer) 
        {
            HealPower.Value = maxHealPower;
        }
    }


    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            HealPower.OnValueChanged -= HandleHealPowerChanged;
        }
    }


    private void HandleHealPowerChanged(int oldHealPower, int newHealPower) 
    {
        healPowerBar.fillAmount = (float) newHealPower / maxHealPower;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsServer) { return; }

        if(!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }

        playerInZone.Add(player);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!IsServer) { return; }

        if (!collision.attachedRigidbody.TryGetComponent<TankPlayer>(out TankPlayer player)) { return; }


        playerInZone.Remove(player);
    }


    private void Update()
    {
        if(!IsServer) { return;}

        if(remainingCoolDown > 0f) 
        {
            remainingCoolDown -= Time.deltaTime;

            if (remainingCoolDown < 0f) 
            {
                HealPower.Value = maxHealPower;
            }

            else 
            {
                return;
            }
        }

        tickTimer += Time.deltaTime;

        if(tickTimer >=1 / healTickRate) 
        {
            foreach (TankPlayer player in playerInZone) 
            {
                if(HealPower.Value == 0) 
                {
                    break;
                }

                if(player.Health.CurrentHealth.Value == player.Health.MaxHealth) 
                {
                    continue;
                }

                if(player.Wallet.TotalCoinValue.Value < coinsPerTick) 
                {
                    continue ;
                }

                player.Wallet.SpendCoins(coinsPerTick);
                player.Health.RestoreHealth(healthPerTick);

                HealPower.Value -= 1;

                if(HealPower.Value == 0) 
                {
                    remainingCoolDown = healCooldown;
                }
            }

            tickTimer = tickTimer % (1 / healTickRate);
        }
    }
}

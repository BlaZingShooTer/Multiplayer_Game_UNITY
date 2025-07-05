using System;
using UnityEngine;

public class RespawningCoin : Coin
{

    public event Action<RespawningCoin> onCollected;

    private Vector3 previousSpawnPosition;
    private void Update()
    {
        if(previousSpawnPosition != transform.position)
        {
            Show(true);
            
        }

        previousSpawnPosition = transform.position;
    }

    public override int Collect()
    {
        if (!IsServer) 
        {
            Show(false);
            return 0;
        }

        if (alreadyCollected)
        {
            return 0;
        }

        alreadyCollected = true;

        onCollected?.Invoke(this);

        return coinValue;
    }

    internal void Reset()
    {
       alreadyCollected = false;
    }
}

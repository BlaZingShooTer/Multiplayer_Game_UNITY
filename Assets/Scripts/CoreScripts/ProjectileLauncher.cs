using System;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]

    [SerializeField] private InputReader inputReader;

    [SerializeField] private CoinWallet wallet;

    [SerializeField] private Transform projectileSpawnPoint;

    [SerializeField] private GameObject serverProjectilePrefab;

    [SerializeField] private GameObject clientProjectilePrefab;

    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] private Collider2D playerCollider;

    [Header("Settings")]

    [SerializeField] private float projectileSpeed = 10f;

    [SerializeField] private float fireRate = 0.5f;

    [SerializeField] private float muzzleFlashDuration = 0.1f;

    [SerializeField] private int costToFire;


    private bool shouldFire;

    private float timer;

    private float muzzleFlashTimer;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // Disable this script for non-owner clients
            return;
        }
        inputReader.OnPrimaryFireEvent += HandlePrimaryFireInput;
    }

    private void HandlePrimaryFireInput(bool shouldFire)
    {
        this.shouldFire = shouldFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        inputReader.OnPrimaryFireEvent -= HandlePrimaryFireInput;
    }

    private void Update()
    {
        if(muzzleFlashTimer > 0f)
        {
            muzzleFlashTimer -= Time.deltaTime;
            if(muzzleFlashTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner)
        {
            return;
        }

        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (!shouldFire)
        {
            return;
        }

        if(timer > 0)
        {
            return;
        }

        if(wallet.TotalCoinValue.Value < costToFire)
        {
            Debug.Log("Not enough coins to fire.");
            return;
        }

        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.up);

        SpawnDummyProjectile(projectileSpawnPoint.position, projectileSpawnPoint.up);

        timer = 1 / fireRate;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {

        muzzleFlash.SetActive(true);

        muzzleFlashTimer = muzzleFlashDuration;

        GameObject projectileInstance = Instantiate(clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Rigidbody2D component.");
        }
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        if (wallet.TotalCoinValue.Value < costToFire)
        {
            Debug.Log("Not enough coins to fire.");
            return;
        }

        wallet.SpendCoins(costToFire);

        GameObject projectileInstance = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);

        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(playerCollider, projectileInstance.GetComponent<Collider2D>());   

        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact damageScript))
        {
            damageScript.SetOwner(OwnerClientId);
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a DealDamageOnContact component.");
        }


        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.linearVelocity = rb.transform.up * projectileSpeed;
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Rigidbody2D component.");
        }


        SpawnDummyProjectileClientRpc(spawnPos, direction);

    }


    [ClientRpc]

    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction) 
    {
        if (IsOwner) 
        {
            return;
        }

        SpawnDummyProjectile(spawnPos, direction);  

    }

}

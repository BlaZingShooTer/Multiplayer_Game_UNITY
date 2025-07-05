using Unity.Netcode;
using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int damageValue = 10;

    private ulong ownerClientId;
    public void SetOwner(ulong ownerClientId) 
    {
        this.ownerClientId = ownerClientId;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.attachedRigidbody == null) return;

       if(collision.attachedRigidbody.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
       {
           // Check if the object is owned by the local player
           if (networkObject.OwnerClientId == ownerClientId)
           {
               return; // Ignore objects not owned by the local player
           }
       }
        // Check if the collided object has a Health component and apply damage

        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
       {
           health.TakeDamage(damageValue);
        }
    }
}

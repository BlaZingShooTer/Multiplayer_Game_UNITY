using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
   private static List<SpawnPoint> spawnPoints = new List<SpawnPoint>();


    private void OnEnable()
    {
        spawnPoints.Add(this);
    }

    private void OnDisable()
    {
        spawnPoints.Remove(this);
    }

    public static Vector3 GetRandomSpawnPoint() 
    {

        if (spawnPoints.Count == 0)
        {
           
            return Vector3.zero; 
        }


        return spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawSphere(transform.position, 1f);
    }
}

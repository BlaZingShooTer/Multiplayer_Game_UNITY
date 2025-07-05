using UnityEngine;

public class Lifetime : MonoBehaviour
{
    [SerializeField] private float lifetime = 2f; // Lifetime in seconds

    // Update is called once per frame
    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
}

using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static Vector3 CurrentSpawnPosition;
    public static bool HasSpawnPoint;

    private void Start()
    {
        if (!HasSpawnPoint)
        {
            CurrentSpawnPosition = transform.position;
            HasSpawnPoint = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CurrentSpawnPosition = transform.position;
        HasSpawnPoint = true;
    }
}
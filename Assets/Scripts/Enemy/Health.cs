using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("Info")]
    public string displayName = "Enemy";

    [Header("Health")]
    public int maxHealth = 3;

    [Header("Death")]
    public bool destroyOnDeath = true;
    public bool respawnOnDeath = false;
    public float respawnDelay = 1.5f;

    private int currentHealth;
    private bool isDead;

    private Rigidbody2D rb;
    private Collider2D[] colliders2D;
    private SpriteRenderer[] renderers;
    private Vector3 startPosition;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        colliders2D = GetComponentsInChildren<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void TakeDamage(DamageRequest damage)
    {
        if (isDead) return;

        currentHealth -= Mathf.Max(1, damage.amount);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    private void Die()
    {
        if (respawnOnDeath)
        {
            StartCoroutine(RespawnRoutine());
            return;
        }

        isDead = true;

        if (destroyOnDeath)
            Destroy(gameObject);
    }

    private IEnumerator RespawnRoutine()
    {
        isDead = true;

        DisableBody();

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = SpawnPoint.HasSpawnPoint
            ? SpawnPoint.CurrentSpawnPosition
            : startPosition;

        transform.position = respawnPosition;
        currentHealth = maxHealth;

        EnableBody();

        isDead = false;
    }

    private void DisableBody()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        for (int i = 0; i < colliders2D.Length; i++)
            colliders2D[i].enabled = false;

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = false;
    }

    private void EnableBody()
    {
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        for (int i = 0; i < colliders2D.Length; i++)
            colliders2D[i].enabled = true;

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].enabled = true;
    }
}
using UnityEngine;

public class PlayerRangedAttack : MonoBehaviour
{
    public Projectile projectilePrefab;
    public Transform firePoint;

    public KeyCode shootKey = KeyCode.LeftControl;
    public float shootCooldown = 0.2f;

    public Animator animator;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip shootClip;

    private float shootTimer;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        if (Input.GetKeyDown(shootKey) && shootTimer <= 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        shootTimer = shootCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (sfxSource != null && shootClip != null)
            sfxSource.PlayOneShot(shootClip);

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float dirX = transform.localScale.x >= 0f ? 1f : -1f;
        Vector2 shootDirection = new Vector2(dirX, 0f);

        projectile.Launch(shootDirection, gameObject);
    }
}
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerMeleeAttack : MonoBehaviour
{
    [Header("Attack")]
    public KeyCode attackKey = KeyCode.Space;
    public int damage = 1;
    public float attackCooldown = 0.3f;

    [Header("Hitbox")]
    public LayerMask enemyMask;
    public Vector2 attackBoxSize = new Vector2(1.2f, 0.8f);
    public float attackOffset = 0.8f;

    public Animator animator;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip meleeClip;

    private Collider2D col;
    private float attackTimer;

    private void Awake()
    {
        col = GetComponent<Collider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(attackKey) && attackTimer <= 0f)
        {
            Attack();
        }
    }

    private void Attack()
    {
        attackTimer = attackCooldown;

        if (animator != null)
            animator.SetTrigger("Attack");

        if (sfxSource != null && meleeClip != null)
            sfxSource.PlayOneShot(meleeClip);

        float dirX = transform.localScale.x >= 0f ? 1f : -1f;

        Bounds b = col.bounds;
        Vector2 center = new Vector2(
            b.center.x + dirX * attackOffset,
            b.center.y
        );

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, attackBoxSize, 0f, enemyMask);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.root == transform.root)
                continue;

            IDamageable damageable = hits[i].GetComponentInParent<IDamageable>();
            if (damageable == null) continue;

            Vector2 hitPoint = hits[i].ClosestPoint(transform.position);
            Vector2 direction = new Vector2(dirX, 0f);

            damageable.TakeDamage(new DamageRequest(
                damage,
                gameObject,
                hitPoint,
                direction
            ));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c == null) return;

        float dirX = transform.localScale.x >= 0f ? 1f : -1f;
        Bounds b = c.bounds;

        Vector2 center = new Vector2(
            b.center.x + dirX * attackOffset,
            b.center.y
        );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(center, attackBoxSize);
    }
}
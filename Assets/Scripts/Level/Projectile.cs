using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    public float speed = 12f;
    public float lifeTime = 3f;
    public int damage = 1;

    public LayerMask wallMask;
    public LayerMask damageMask;

    private Rigidbody2D rb;
    private GameObject sender;
    private Vector2 direction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 dir, GameObject owner)
    {
        direction = dir.normalized;
        sender = owner;
        rb.linearVelocity = direction * speed;

        if (direction.x != 0f)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (sender != null && other.transform.root.gameObject == sender)
            return;

        int otherLayer = 1 << other.gameObject.layer;

        if ((wallMask.value & otherLayer) != 0)
        {
            Destroy(gameObject);
            return;
        }

        if ((damageMask.value & otherLayer) != 0)
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(new DamageRequest(
                    damage,
                    sender,
                    transform.position,
                    direction
                ));

                Destroy(gameObject);
            }
        }
    }
}
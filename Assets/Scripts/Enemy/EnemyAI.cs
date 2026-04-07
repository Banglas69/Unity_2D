using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType
    {
        Ranged,
        Melee
    }

    [Header("Type")]
    public EnemyType enemyType = EnemyType.Ranged;

    [Header("Target")]
    public Transform player;

    [Header("Ranges")]
    public float detectionRange = 8f;
    public float meleeRange = 1.2f;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Line of Sight")]
    public LayerMask obstacleMask;

    [Header("Ranged Attack")]
    public Projectile projectilePrefab;
    public Transform firePoint;
    public float shootCooldown = 1f;

    [Header("Melee Attack")]
    public int meleeDamage = 1;
    public float meleeCooldown = 0.8f;

    [Header("Debug")]
    public string currentStateName;

    private Rigidbody2D rb;
    private IEnemyState currentState;

    private EnemyIdleState idleState;
    private EnemyChaseState chaseState;
    private EnemyRangedAttackState rangedAttackState;
    private EnemyMeleeAttackState meleeAttackState;

    private float shootTimer;
    private float meleeTimer;

    public float DistanceToPlayer
    {
        get
        {
            if (player == null) return Mathf.Infinity;
            return Vector2.Distance(transform.position, player.position);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        idleState = new EnemyIdleState(this);
        chaseState = new EnemyChaseState(this);
        rangedAttackState = new EnemyRangedAttackState(this);
        meleeAttackState = new EnemyMeleeAttackState(this);
    }

    private void OnValidate()
    {
        if (meleeRange >= detectionRange)
            meleeRange = detectionRange - 0.1f;

        if (meleeRange < 0.1f)
            meleeRange = 0.1f;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        ChangeState(idleState);
    }

    private void Update()
    {
        if (player == null)
        {
            currentStateName = "No Player";
            StopMoving();
            return;
        }

        shootTimer -= Time.deltaTime;
        meleeTimer -= Time.deltaTime;

        UpdateStateLogic();

        if (currentState != null)
            currentState.Tick();

        FacePlayer();
    }

    private void UpdateStateLogic()
    {
        float distance = DistanceToPlayer;

        if (distance > detectionRange)
        {
            ChangeState(idleState);
            return;
        }

        if (enemyType == EnemyType.Ranged)
        {
            ChangeState(rangedAttackState);
            return;
        }

        if (distance > meleeRange)
            ChangeState(chaseState);
        else
            ChangeState(meleeAttackState);
    }

    public void ChangeState(IEnemyState newState)
    {
        if (newState == null || currentState == newState) return;

        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();

        currentStateName = newState.GetType().Name;
    }

    public void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void MoveTowardsPlayer()
    {
        if (player == null) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);

        if (Mathf.Abs(player.position.x - transform.position.x) < 0.05f)
            dir = 0f;

        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    public bool HasLineOfSight()
    {
        if (player == null) return false;

        Vector2 start = transform.position;
        Vector2 end = player.position;

        RaycastHit2D hit = Physics2D.Linecast(start, end, obstacleMask);
        return hit.collider == null || hit.transform == player;
    }

    public void TryShoot()
    {
        if (shootTimer > 0f) return;
        if (projectilePrefab == null || firePoint == null || player == null) return;
        if (!HasLineOfSight()) return;

        shootTimer = shootCooldown;

        Projectile projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;
        projectile.Launch(dir, gameObject);
    }

    public void TryMeleeAttack()
    {
        if (meleeTimer > 0f) return;
        if (player == null) return;

        IDamageable target = player.GetComponent<IDamageable>();
        if (target == null) return;

        Vector2 dir = (player.position - transform.position).normalized;

        target.TakeDamage(new DamageRequest(
            meleeDamage,
            gameObject,
            player.position,
            dir
        ));

        meleeTimer = meleeCooldown;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        float dir = player.position.x - transform.position.x;
        if (Mathf.Abs(dir) < 0.01f) return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (dir > 0f ? 1f : -1f);
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        if (player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}
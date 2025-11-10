using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 3f;

    [Header("Platform Safety")]
    public LayerMask whatIsGround;
    public Transform groundCheck;

    private bool movingRight = true;
    private float lastAttackTime;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // When respawning/starting, ensure the enemy is still
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Attack State
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Chase State
            ChasePlayer();
        }
        else
        {
            // Patrol State
            Patrol();
        }
    }

    void Patrol()
    {
        // Check for ground ahead. If none, flip.
        if (!IsGroundAhead())
        {
            Flip();
        }
        
        rb.linearVelocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.linearVelocity.y);
    }

    void ChasePlayer()
    {
        // Check for ground ahead. If none, stop.
        if (!IsGroundAhead())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Turn to face the player
        if ((player.position.x > transform.position.x && !movingRight) || (player.position.x < transform.position.x && movingRight))
        {
            Flip();
        }
        // Move towards the player
        rb.linearVelocity = new Vector2(moveSpeed * (movingRight ? 1 : -1), rb.linearVelocity.y);
    }

    bool IsGroundAhead()
    {
        // Use a short raycast from the groundCheck position to see if there's ground in front
        Vector2 raycastOrigin = groundCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, 0.5f, whatIsGround);
        return hit.collider != null;
    }

    void Attack()
    {
        if (Mathf.Abs(player.position.y - transform.position.y) < 0.5f)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null)
                    health.TakeDamage(1);
            }
        }
    }

    void Flip()
    {
        movingRight = !movingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw ground check gizmo
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.5f);
        }
    }
}

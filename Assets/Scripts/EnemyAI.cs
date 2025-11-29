using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;            // İstersen Inspector’dan ver
    public string playerTag = "Player"; // Boşsa bu tag ile otomatik bulur

    [Header("Ranges")]
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 3f;

    [Header("Platform Safety")]
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;

    private bool movingRight = true;
    private float lastAttackTime;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Eğer Inspector’dan player atanmamışsa, tag ile bulmayı dene
        if (player == null && !string.IsNullOrEmpty(playerTag))
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null)
            {
                player = p.transform;
            }
        }

        // Hâlâ yoksa, kendini kapat ki hata yağdırmasın
        if (player == null)
        {
            Debug.LogWarning($"{nameof(EnemyAI)} on {name}: Player reference is missing. Disabling EnemyAI.", this);
            enabled = false;
            return;
        }

        if (groundCheck == null)
        {
            Debug.LogWarning($"{nameof(EnemyAI)} on {name}: groundCheck is not assigned.", this);
        }
    }

    private void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (player == null) return; // ekstra güvenlik

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            // Attack state
            StopHorizontal();
            TryAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            // Chase state
            ChasePlayer();
        }
        else
        {
            // Patrol state
            Patrol();
        }
    }

    private void StopHorizontal()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void Patrol()
    {
        // Önünde zemin yoksa dön
        if (groundCheck != null && !IsGroundAhead())
        {
            Flip();
        }

        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    private void ChasePlayer()
    {
        // Önünde zemin yoksa koşmayı kes
        if (groundCheck != null && !IsGroundAhead())
        {
            StopHorizontal();
            return;
        }

        // Player hangi tarafta ise o yöne bak
        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if ((directionToPlayer > 0f && !movingRight) ||
            (directionToPlayer < 0f && movingRight))
        {
            Flip();
        }

        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    private bool IsGroundAhead()
    {
        if (groundCheck == null) return true; // güvenlik: atanmadıysa düşmesin

        Vector2 origin = groundCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, whatIsGround);
        return hit.collider != null;
    }

    private void TryAttack()
    {
        // Cooldown kontrolü
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // Çok farklı yükseklikteyse vurmasın
        if (Mathf.Abs(player.position.y - transform.position.y) > 0.5f)
            return;

        lastAttackTime = Time.time;

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(1);
        }
    }

    private void Flip()
    {
        movingRight = !movingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position,
                            groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}

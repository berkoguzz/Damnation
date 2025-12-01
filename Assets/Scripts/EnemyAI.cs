using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public string playerTag = "Player";

    [Header("Ranges")]
    public float detectionRange = 5f;
    public float attackRange = 1.2f;
    public float meleeRange = 1.5f;
    public float ranged1Range = 3f;
    public float ranged2Range = 6f;
    public float moveSpeed = 2f;
    public float chasingSpeed;
    public float attackCooldown = 3f;

    [Header("Detection Angle")]
    public float minDetectionAngle = 0f;
    public float maxDetectionAngle = 60f;
    public float blindSpotMinAngle = 120f;
    public float blindSpotMaxAngle = 180f;

    [Header("Platform Safety")]
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public float groundCheckDistance = 0.5f;

    private bool movingRight = true;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator animator;
    private Animator armAnimator;
    private Animator laserAnimator;
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Player'ı bul
        if (player == null && !string.IsNullOrEmpty(playerTag))
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null)
            {
                player = p.transform;
            }
        }

        if (player == null)
        {
            Debug.LogWarning($"{nameof(EnemyAI)}: Player bulunamadı!", this);
            enabled = false;
            return;
        }

        if (groundCheck == null)
        {
            Debug.LogWarning($"{nameof(EnemyAI)}: groundCheck atanmamış!", this);
        }

        // Child animator'ları bul
        armAnimator = transform.Find("GolemArm")?.GetComponent<Animator>();
        laserAnimator = transform.Find("LaserBeam")?.GetComponent<Animator>();

        if (armAnimator == null)
            Debug.LogWarning($"{nameof(EnemyAI)}: GolemArm Animator bulunamadı!", this);
        
        if (laserAnimator == null)
            Debug.LogWarning($"{nameof(EnemyAI)}: LaserBeam Animator bulunamadı!", this);

        chasingSpeed = moveSpeed * 2f;
    }

    private void OnEnable()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Açı kontrolü
        if (!IsPlayerInDetectionAngle(distanceToPlayer))
        {
            isChasing = false;
            Patrol();
            return;
        }

        // Distance kontrolü
        if (distanceToPlayer <= attackRange)
        {
            isChasing = false;
            TryAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            Patrol();
        }
    }

    private bool IsPlayerInDetectionAngle(float distanceToPlayer)
    {
        if (distanceToPlayer > detectionRange)
            return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        Vector2 mobDirection = movingRight ? Vector2.right : Vector2.left;
        float signedAngle = Vector2.SignedAngle(mobDirection, directionToPlayer);
        float absoluteAngle = Mathf.Abs(signedAngle);

        if (absoluteAngle >= minDetectionAngle && absoluteAngle <= maxDetectionAngle)
        {
            return true;
        }
        else if (absoluteAngle >= blindSpotMinAngle && absoluteAngle <= blindSpotMaxAngle)
        {
            return false;
        }

        return false;
    }

    private void Patrol()
    {
        if (groundCheck != null && !IsGroundAhead())
        {
            Flip();
        }

        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * moveSpeed, rb.linearVelocity.y);
    }

    private void ChasePlayer()
    {
        if (groundCheck != null && !IsGroundAhead())
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if ((directionToPlayer > 0f && !movingRight) ||
            (directionToPlayer < 0f && movingRight))
        {
            Flip();
        }

        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * chasingSpeed, rb.linearVelocity.y);
    }

    private bool IsGroundAhead()
    {
        if (groundCheck == null) return true;

        Vector2 origin = groundCheck.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundCheckDistance, whatIsGround);
        return hit.collider != null;
    }

    private void TryAttack()
    {
        // Cooldown kontrol
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // Yükseklik kontrol
        if (Mathf.Abs(player.position.y - transform.position.y) > 0.5f)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Hareketi durdur
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        
        lastAttackTime = Time.time;

        // Mesafeye göre saldırı tipi seç
        if (distanceToPlayer <= meleeRange)
        {
            // MELEE ATTACK - Yakın saldırı
            Debug.Log("⚔️ MELEE ATTACK!");
            PlayBodyAnimation("Melee");
            DealDamageToPlayer(2);
        }
        else if (distanceToPlayer <= ranged1Range)
        {
            // RANGED ATTACK 1 - Silah
            Debug.Log("🗡️ RANGED ATTACK 1 - WEAPON!");
            PlayBodyAnimation("RangedAttack");
            PlayArmAnimation("Weapon");
            DealDamageToPlayer(1);
        }
        else if (distanceToPlayer <= ranged2Range)
        {
            // RANGED ATTACK 2 - Lazer
            Debug.Log("⚡ RANGED ATTACK 2 - LASER!");
            PlayBodyAnimation("RangedAttack");
            PlayLaserAnimation("Fire");
            DealDamageToPlayer(1);
        }
    }

    private void PlayBodyAnimation(string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }

    private void PlayArmAnimation(string triggerName)
    {
        if (armAnimator != null)
        {
            armAnimator.SetTrigger(triggerName);
        }
    }

    private void PlayLaserAnimation(string triggerName)
    {
        if (laserAnimator != null)
        {
            laserAnimator.SetTrigger(triggerName);
        }
    }

    private void DealDamageToPlayer(int damage)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log($"💥 Damage dealt: {damage}");
        }
        else
        {
            Debug.LogWarning("PlayerHealth component bulunamadı!");
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

        DrawDetectionAngleGizmo();
    }

    private void DrawDetectionAngleGizmo()
    {
        if (detectionRange <= 0) return;

        Vector2 mobDir = movingRight ? Vector2.right : Vector2.left;

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        DrawAngleArc(mobDir, minDetectionAngle, maxDetectionAngle, detectionRange);

        Gizmos.color = new Color(0, 1, 0, 0.3f);
        DrawAngleArc(mobDir, blindSpotMinAngle, blindSpotMaxAngle, detectionRange);

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        DrawAngleArc(mobDir, maxDetectionAngle, blindSpotMinAngle, detectionRange);
    }

    private void DrawAngleArc(Vector2 centerDir, float startAngle, float endAngle, float radius)
    {
        int segments = 20;
        Vector3 lastPoint = transform.position + (Vector3)GetDirectionAtAngle(centerDir, startAngle) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, i / (float)segments);
            Vector3 newPoint = transform.position + (Vector3)GetDirectionAtAngle(centerDir, angle) * radius;
            Gizmos.DrawLine(lastPoint, newPoint);
            lastPoint = newPoint;
        }

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)GetDirectionAtAngle(centerDir, startAngle) * radius);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)GetDirectionAtAngle(centerDir, endAngle) * radius);
    }

    private Vector2 GetDirectionAtAngle(Vector2 centerDir, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            centerDir.x * cos - centerDir.y * sin,
            centerDir.x * sin + centerDir.y * cos
        );
    }
}
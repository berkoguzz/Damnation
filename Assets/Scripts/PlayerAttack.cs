using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1f;
    public int damage = 1;
    public LayerMask enemyLayer;
    public float attackCooldown = 2f;
    public float attackManaCost = 300f; // Siyah kutuya vurunca 300 mana azalır
    
    private PlayerMana mana;
    private float lastAttackTime;

    void Start()
    {
        mana = GetComponent<PlayerMana>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            TryAttack();
        }
    }

    void TryAttack()
    {
        if (mana.UseMana(attackManaCost))
        {
            Attack();
            lastAttackTime = Time.time;
            Debug.Log("💥 Attack! Mana kullanıldı: " + attackManaCost);
        }
        else
        {
            Debug.Log("❌ Yeterli mana yok! (300 mana gerekli)");
        }
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
            Debug.Log("Hit " + enemy.name);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
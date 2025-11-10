using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1f;
    public int damage = 1;
    public LayerMask enemyLayer;
    public float attackCooldown = 2f; // 2 saniyede bir
    private float lastAttackTime;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
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

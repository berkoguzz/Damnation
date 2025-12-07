using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 2;
    public int currentHealth;
    public Image healthBar;
    public float respawnTime = 10f;

    private SpriteRenderer spriteRenderer;
    private Collider2D enemyCollider;
    private EnemyAI enemyAI;
    private Vector3 initialPosition;
    private bool isDead = false;

    private Color originalColor; // New variable

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        enemyAI = GetComponent<EnemyAI>();
        
        initialPosition = transform.position;
        currentHealth = maxHealth;
        UpdateBar();
        originalColor = spriteRenderer.color; // Store original color
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // Don't take damage if already dead

        currentHealth -= amount;
        UpdateBar();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageFlash()); // Re-add call
        }
    }

    IEnumerator DamageFlash() // Re-add coroutine
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor; // Restore original color
    }

    void UpdateBar()
    {
        if (healthBar != null)
            healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        if (isDead) return;
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        isDead = true;
        
        // --- DEATH: Disable components ---
        Debug.Log("Enemy died! Starting respawn timer...");
        enemyAI.enabled = false;
        spriteRenderer.enabled = false;
        enemyCollider.enabled = false;
        if (healthBar != null && healthBar.transform.parent != null)
        {
            healthBar.transform.parent.gameObject.SetActive(false);
        }

        // --- WAIT for respawn time ---
        yield return new WaitForSeconds(respawnTime);

        // --- RESPAWN: Reset state and re-enable components ---
        Debug.Log("Respawning enemy.");
        transform.position = initialPosition;
        
        currentHealth = maxHealth;
        UpdateBar();
        
        spriteRenderer.enabled = true;
        enemyCollider.enabled = true;
        enemyAI.enabled = true;
        if (healthBar != null && healthBar.transform.parent != null)
        {
            healthBar.transform.parent.gameObject.SetActive(true);
        }

        isDead = false;
    }
}
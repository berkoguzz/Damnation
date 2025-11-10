using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public float respawnDelay = 2f;
    public Transform respawnPoint;

    private SpriteRenderer spriteRenderer;
    private bool isDead = false; // 🔥 eklendi

    private Color originalColor; // New variable

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHearts();
        originalColor = spriteRenderer.color; // Store original color
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return; // 🧱 öldüyse tekrar hasar almaz

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHearts();
        StartCoroutine(DamageFlash()); // Re-add call

        if (currentHealth <= 0)
        {
            isDead = true; // ⚠️ artık ölü
            Debug.Log("Player died!");
            StartCoroutine(Respawn());
        }
    }

    IEnumerator DamageFlash() // Re-add coroutine
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = originalColor; // Restore original color
    }

    IEnumerator Respawn()
    {
        spriteRenderer.enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        yield return new WaitForSeconds(respawnDelay);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = respawnPoint.position;

        currentHealth = maxHealth;
        UpdateHearts();

        GetComponent<Rigidbody2D>().simulated = true;
        spriteRenderer.enabled = true;

        isDead = false; // 🟢 yeniden doğduktan sonra resetle
        Debug.Log("Player respawned!");
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = i < currentHealth ? fullHeart : emptyHeart;
        }
    }
}

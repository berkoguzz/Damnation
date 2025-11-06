using UnityEngine;

public class DeathZone : MonoBehaviour
{
    public Transform respawnPoint; // yeniden do�ma noktas�

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncunun Rigidbody'sini s�f�rl�yoruz ki fizik hatas� olmas�n
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            rb.linearVelocity = Vector2.zero;

            // Oyuncuyu respawn noktas�na ���nla
            other.transform.position = respawnPoint.position;
        }
    }
}

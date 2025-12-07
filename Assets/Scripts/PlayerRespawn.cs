using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (respawnPoint == null)
            return;

        rb.linearVelocity = Vector2.zero;
        transform.position = respawnPoint.position;
    }
}

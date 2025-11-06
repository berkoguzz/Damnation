using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint; // respawn noktası

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            transform.position = respawnPoint.position;
        }
    }
}

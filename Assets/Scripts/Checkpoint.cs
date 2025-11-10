using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncunun PlayerRespawn scriptini bul
            PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
            if (respawn != null)
            {
                // Checkpoint pozisyonunu yeni respawn noktası yap
                respawn.respawnPoint = transform;
                Debug.Log("Yeni checkpoint ayarlandı: " + transform.position);
            }
        }
    }
}

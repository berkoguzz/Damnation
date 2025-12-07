using UnityEngine;

// Script'in adýný MovingPlatform2D yapabilirsin
public class MovingPlatform : MonoBehaviour
{
    // 1. Gideceði noktalarý Unity editöründen atamak için
    public Transform waypoint_A;
    public Transform waypoint_B;

    // 2. Platformun hýzý
    public float speed = 3.0f;

    // 3. Platformun o an gitmekte olduðu hedef
    private Transform currentTarget;

    void Start()
    {
        // Oyuna baþlarken ilk hedef B noktasý olsun
        currentTarget = waypoint_B;
    }

    void Update()
    {
        // Platformu mevcut hedefe doðru hareket ettir
        // Vector3.MoveTowards 2D pozisyonlar için de sorunsuz çalýþýr (z eksenini 0 kabul eder)
        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Platform hedefe çok yaklaþtýysa (veya ulaþtýysa)
        if (Vector3.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            // Hedefi deðiþtir
            if (currentTarget == waypoint_B)
            {
                currentTarget = waypoint_A;
            }
            else
            {
                currentTarget = waypoint_B;
            }
        }
    }

    // OYUNCUNUN PLATFORM ÝLE HAREKET ETMESÝ ÝÇÝN (2D)

    // Platforma bir nesne temas ettiðinde (2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Platforma temas eden nesnenin etiketi "Player" ise
        if (collision.gameObject.CompareTag("Player"))
        {
            // Oyuncuyu platformun çocuðu (child) yap.
            // Böylece platform hareket edince oyuncu da onunla hareket eder.
            collision.transform.SetParent(this.transform);
        }
    }

    // Oyuncu platformdan ayrýldýðýnda (2D)
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Oyuncu platformdan ayrýldýðýnda
        if (collision.gameObject.CompareTag("Player"))
        {
            // Oyuncunun "child" iliþkisini bitir (parent'ýný null yap).
            collision.transform.SetParent(null);
        }
    }
}
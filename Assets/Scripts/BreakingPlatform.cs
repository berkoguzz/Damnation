using System.Collections;
using UnityEngine;

public class BreakingPlatform : MonoBehaviour
{
    // 1. K�r�lmadan �nce bekleme s�resi
    public float breakDelay = 2.0f;

    // 2. K�r�ld�ktan sonra geri gelmesi i�in bekleme s�resi
    public float respawnTime = 5.0f;

    private Rigidbody2D rb2D;
    private SpriteRenderer sr; // G�rseli a��p kapamak i�in
    private Collider2D col;    // Fizi�ini a��p kapamak i�in

    private Vector3 initialPosition; // Ba�lang�� pozisyonunu kaydetmek i�in
    private Quaternion initialRotation; // Ba�lang�� rotasyonunu kaydetmek i�in

    private Coroutine breakCoroutine = null; // K�r�lma s�recini tutan referans

    void Start()
    {
        // Gerekli component'leri al
        rb2D = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        // Platformun ilk pozisyonunu ve rotasyonunu kaydet
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Ba�lang��ta platformun sabit durdu�undan emin ol
        rb2D.bodyType = RigidbodyType2D.Kinematic;
    }

    // Oyuncu platformun �ZER�NE �ND���NDE (2D)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Temas eden "Player" ise ve platform zaten k�r�lma s�recinde de�ilse
        if (collision.gameObject.CompareTag("Player") && breakCoroutine == null)
        {
            // Oyuncunun �stten bast���ndan emin ol (yandan �arpmas�n)
            if (collision.contacts[0].normal.y < -0.5f)
            {
                // K�r�lma s�recini ba�lat
                breakCoroutine = StartCoroutine(BreakSequence());
            }
        }
    }

    // Oyuncu platformdan AYRILDI�INDA (2D)
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ayr�lan "Player" ise
        if (collision.gameObject.CompareTag("Player"))
        {
            // E�er oyuncu, platform tam KIRILMADAN �NCE ayr�l�rsa...
            // (breakCoroutine != null && rb2D.bodyType == RigidbodyType2D.Kinematic)
            // Bu kontrol, platform d��meye ba�lad�ktan sonra coroutine'i iptal etmeyi engeller.
            if (breakCoroutine != null && rb2D.bodyType == RigidbodyType2D.Kinematic)
            {
                // ...k�r�lma geri say�m�n� iptal et.
                StopCoroutine(breakCoroutine);
                breakCoroutine = null; // Referans� temizle, tekrar bas�labilir olsun
            }
        }
    }

    // Ad�m 1: K�r�lma Geri Say�m� ve D��me
    private IEnumerator BreakSequence()
    {
        // (Opsiyonel) Titreme efekti
        // ...

        // K�r�lma s�resi kadar bekle
        yield return new WaitForSeconds(breakDelay);

        // Platformu D���r
        rb2D.bodyType = RigidbodyType2D.Dynamic;
        rb2D.constraints = RigidbodyConstraints2D.FreezeRotation; // D�nerken takla atmas�n

        // Platform art�k k�r�ld���na g�re, Respawn (Geri Gelme) s�recini ba�lat
        StartCoroutine(RespawnSequence());
    }

    // Ad�m 2: Geri Gelme (Respawn) S�reci
    private IEnumerator RespawnSequence()
    {
        // Platformun d��mesi ve g�zden kaybolmas� i�in 'respawnTime' kadar bekle
        yield return new WaitForSeconds(respawnTime);

        // --- PLATFORMU SIFIRLA ---

        // 1. �NCE hareketi durdur (hala Dynamic iken)
        rb2D.linearVelocity = Vector2.zero;
        rb2D.angularVelocity = 0f;

        // 2. SONRA fizi�i durdur ve Kinematic yap
        rb2D.bodyType = RigidbodyType2D.Kinematic;

        // 3. G�rseli ve Collider'� kapat (Platform "yok" gibi davrans�n)
        sr.enabled = false;
        col.enabled = false;

        // 4. Orijinal pozisyonuna ve rotasyonuna geri d�nd�r
        transform.position = initialPosition;
        transform.rotation = initialRotation;

        // (Opsiyonel) Geri gelmeden �nce k�sa bir bekleme veya belirme efekti eklenebilir
        // yield return new WaitForSeconds(0.5f); 

        // 5. G�rseli ve Collider'� tekrar a� (Platform "geri geldi")
        sr.enabled = true;
        col.enabled = true;

        // 6. Coroutine referans�n� temizle ki oyuncu tekrar bast���nda s�re� yeniden ba�las�n
        breakCoroutine = null;
    }
}
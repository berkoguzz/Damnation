using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Dash Settings")]
    public float dashForce = 15f;      // Dash g�c�
    public float dashDuration = 0.2f;  // Dash s�resi (ne kadar h�zl� gidecek)
    public float dashCooldown = 5f;    // Dash tekrar kullan�lmadan �nceki bekleme s�resi

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private float dashCooldownTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Dash s�resi dolmad�ysa sayac� azalt
        if (dashCooldownTimer > 0)
            dashCooldownTimer -= Time.deltaTime;

        if (!isDashing)
        {
            Move();
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0)
        {
            StartCoroutine(Dash());
        }
    }

    void Move()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0; // havada s�z�lmesin

        Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        if (dashDirection == Vector2.zero)
            dashDirection = new Vector2(transform.localScale.x, 0); // duruyorsa son y�ne g�re dash atar

        rb.linearVelocity = dashDirection.normalized * dashForce;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}

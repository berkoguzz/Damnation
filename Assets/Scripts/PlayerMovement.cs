using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMana))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Dash Settings")]
    public float dashForce = 15f;
    public float dashDuration = 0.1f;
    public float dashCooldown = 5f;
    public float dashManaCost = 500f;

    private PlayerMana mana;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isGrounded;
    private bool isDashing;
    private float dashCooldownTimer = 0f;

    // Son baktığı yön (1 = sağ, -1 = sol)
    private float lastMoveDirection = 1f;

    private static readonly int SpeedParam = Animator.StringToHash("Speed");
    private static readonly int DashTrigger = Animator.StringToHash("Dash");

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mana = GetComponent<PlayerMana>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Dash cooldown sayacı
        if (dashCooldownTimer > 0f)
            dashCooldownTimer -= Time.deltaTime;

        // Dash yapmıyorken normal hareket + zıplama
        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        // Dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownTimer <= 0f)
        {
            if (mana != null && mana.UseMana(dashManaCost))
            {
                StartCoroutine(Dash());
            }
            else
            {
                Debug.Log("Not enough mana for Dash!");
            }
        }

        // Animator'a hız bilgisini gönder (idle / forward için)
        float horizontalSpeed = Mathf.Abs(rb.linearVelocity.x);
        animator.SetFloat(SpeedParam, horizontalSpeed);
    }

    private void HandleMovement()
    {
        float moveInput = 0f;

        if (Input.GetKey(KeyCode.A))
            moveInput = -1f;
        else if (Input.GetKey(KeyCode.D))
            moveInput = 1f;

        // Rigidbody'ye hız ver
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Yönü kaydet (dash'te kullanacağız)
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            lastMoveDirection = Mathf.Sign(moveInput);

            // Sprite'ı sağ/sol çevir
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * lastMoveDirection;
            transform.localScale = scale;
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Dash yönü: o anki input varsa onu kullan, yoksa son baktığı yön
        float inputX = Input.GetAxisRaw("Horizontal");
        float dashDir = Mathf.Abs(inputX) > 0.01f ? Mathf.Sign(inputX) : lastMoveDirection;

        Vector2 dashVelocity = new Vector2(dashDir * dashForce, 0f);
        rb.linearVelocity = dashVelocity;

        // Dash animasyonu tetikle
        animator.SetTrigger(DashTrigger);

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}

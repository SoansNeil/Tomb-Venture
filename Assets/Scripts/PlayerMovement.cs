using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashDistance = 5f;
    public float dashSpeed = 40f;
    public float slamForce = 10f;
    public float dashCooldown = 1f;
    public LayerMask wallMask;
    private Rigidbody2D rb;
    private int jumpRemain;
    private int maxJump = 2;
    public float wallSlideSpeed = 1f;
    public float attackRange = 1f;
    public float attackCooldown = 0.5f;
    public int attackDamage = 10;
    public LayerMask enemyMask;
    private bool isGrounded, isSlamming, isDashing, isWallSliding = false;
    private bool isTouchingWall = false;
    private bool facingRight = true;
    private float dashTimer;
    private float attackTimer;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpRemain = maxJump;
        if (!IsOwner)
            rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;

        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        if (rb.velocity.x > 0.1f) facingRight = true;
        else if (rb.velocity.x < -0.1f) facingRight = false;
        if(Input.GetButtonDown("Jump") && jumpRemain > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            AudioManager.Instance.PlaySoundEffect(AudioManager.Instance.jumpSound);
            jumpRemain--;
        }
        bool wasWallSliding = isWallSliding;
        isWallSliding = isTouchingWall && !isGrounded && rb.velocity.y < 0;

        if (isWallSliding)
        {
            rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            if (!wasWallSliding) jumpRemain++;
        }

        if (dashTimer > 0) dashTimer -= Time.deltaTime;
        if (attackTimer > 0) attackTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTimer <= 0 && !isDashing)
            StartCoroutine(Dash());
        if (Input.GetKeyDown(KeyCode.W) && attackTimer <= 0)
            StartCoroutine(Attack());
        if(Input.GetKeyDown(KeyCode.S) && !isGrounded && !isSlamming)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slamForce);
            isSlamming = true;
        }
    }
    private IEnumerator Attack()
    {
        attackTimer = attackCooldown;

        float direction = facingRight ? 1f : -1f;
        Vector2 hitPoint = (Vector2)transform.position + Vector2.right * (direction * attackRange);

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint, attackRange, enemyMask);
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(attackDamage);
        }

        yield return null;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        dashTimer = dashCooldown;

        float direction = facingRight ? 1f : -1f;
        Vector2 target = (Vector2)transform.position + Vector2.right * direction * dashDistance;
        rb.MovePosition(target);

        yield return null;
        isDashing = false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpRemain = maxJump;
            isGrounded = true;
            isSlamming = false;
        }
        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = false;
    }
}

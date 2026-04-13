using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float dashForce = 5f;
    public float slamForce = 10f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    private Rigidbody2D rb;
    private int jumpRemain,dashRemain;
    private int maxJump = 2;
    private int maxDash = 1;
    private bool isGrounded,isSlamming,isDashing = false;
    private bool facingRight = true;
    private float dashTimer;

    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpRemain = maxJump;
        dashRemain = maxDash;
    }

    // Update is called once per frame
    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        if (rb.velocity.x > 0.1f) facingRight = true;
        else if (rb.velocity.x < -0.1f) facingRight = false;
        if(Input.GetButtonDown("Jump") && jumpRemain > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpRemain--;
        }
        if (dashTimer > 0) dashTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashTimer <= 0 && !isDashing)
            StartCoroutine(Dash());
        if(Input.GetKeyDown(KeyCode.S) && !isGrounded && !isSlamming)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slamForce);
            isSlamming = true;
        }
    }
    private IEnumerator Dash()
    {
        isDashing = true;
        dashTimer = dashCooldown;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float direction = facingRight ? 1f : -1f;
        rb.velocity = new Vector2(dashForce * direction, 0f);

        //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);

        isDashing = false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpRemain = maxJump;
            dashRemain = maxDash;
            isGrounded = true;
            isSlamming = false;
        } 
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}

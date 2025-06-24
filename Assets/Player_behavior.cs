using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float jumpPower = 5f;

    // 이동 관련
    public float maxMoveSpeed = 7f;
    public float moveAccelTime = 0.1f;

    // 대시 관련
    public float dashSpeed = 7.5f;
    public float dashDuration = 0.5f;
    public float dashAccelTime = 0.2f;

    Rigidbody2D rigid;

    // 이동 상태
    float moveInput = 0f;
    float moveAccelTimer = 0f;
    float currentMoveSpeed = 0f;
    int facingDirection = 1;

    // 점프 상태
    bool isJumping = false;
    bool isGrounded = false;

    // 대시 상태
    bool isDashing = false;
    bool dashKeyHeld = false;
    float dashTimer = 0f;
    float currentDashSpeed = 0f;
    int dashDirection = 0;

    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

        // 대시 입력
        dashKeyHeld = Input.GetKey(KeyCode.LeftShift);

        if (!isDashing && dashKeyHeld && Input.GetAxisRaw("Horizontal") != 0)
        {
            isDashing = true;
            dashTimer = 0f;
            dashDirection = Input.GetAxisRaw("Horizontal") > 0 ? 1 : -1;
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
            Dash();
        else
            Move();

        Jump();
    }

    void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            // 방향 전환
            facingDirection = moveInput > 0 ? 1 : -1;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * facingDirection;
            transform.localScale = scale;

            // 가속
            moveAccelTimer += Time.fixedDeltaTime;
            float t = Mathf.Clamp01(moveAccelTimer / moveAccelTime);
            currentMoveSpeed = Mathf.Lerp(0f, maxMoveSpeed, t);
        }
        else
        {
            // 멈춤
            moveAccelTimer = 0f;
            currentMoveSpeed = 0f;
        }

        // 이동
        transform.position += Vector3.right * moveInput * currentMoveSpeed * Time.fixedDeltaTime;
    }

    void Jump()
    {
        if (!isJumping)
            return;

        isJumping = false;
        isGrounded = false;

        rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, 0f);
        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
    }

    void Dash()
    {
        dashTimer += Time.fixedDeltaTime;

        if (dashTimer < dashAccelTime)
        {
            float t = dashTimer / dashAccelTime;
            currentDashSpeed = Mathf.Lerp(0f, dashSpeed, t);
        }
        else if (dashTimer < dashDuration)
        {
            currentDashSpeed = dashSpeed;

            if (!dashKeyHeld)
            {
                isDashing = false;
                currentDashSpeed = 0f;
                return;
            }
        }
        else
        {
            isDashing = false;
            currentDashSpeed = 0f;
            return;
        }

        transform.position += Vector3.right * dashDirection * currentDashSpeed * Time.fixedDeltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

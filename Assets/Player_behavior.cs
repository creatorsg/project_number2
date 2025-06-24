using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float movePower = 1f;
    public float jumpPower = 5f;

    public float dashSpeed = 7.5f;
    public float dashDuration = 0.5f;     
    public float dashAccelTime = 0.2f;    

    Rigidbody2D rigid;

    bool isJumping = false;
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
        if (Input.GetButtonDown("Jump"))
        {
            isJumping = true;
        }

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
        Vector3 moveVelocity = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            moveVelocity = Vector3.left;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            moveVelocity = Vector3.right;
        }

        transform.position += moveVelocity * movePower * Time.deltaTime;
    }

    void Jump()
    {
        if (!isJumping)
            return;

        rigid.linearVelocity = Vector2.zero;
        Vector2 jumpVelocity = new Vector2(0, jumpPower);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

        isJumping = false;
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
}

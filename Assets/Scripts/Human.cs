using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Human : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float jumpVelocity = 7f;

    [SerializeField] float damageTime = 0.3f;
    float damage = 0f;
    [SerializeField] float iFramesTime = 1f;
    float iFrames = 0f;

    [SerializeField] float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    [SerializeField] float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [SerializeField] Gun gun;

    bool moving = false;
    bool isAlive = true;
    bool possessed = false;

    float direction = 1f;

    float facing = 1f;

    bool jumpButton = false;
    bool onGround = true;

    [SerializeField] Animator myAnimator;
    [SerializeField] Transform gunArm;
    [SerializeField] Transform shoulder;
    Rigidbody2D myRigidbody;
    Body body;
    Vector2 moveInput;

    // Start is called before the first frame update
    void Start() {
        myRigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<Body>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!possessed) {
            return;
        }
        if (!isAlive) {
            StopAllMotion();
            return;
        }
        onGround = body.IsGrounded();
        DirectionFacing();
        Animations();
        MovePlayer();
        Jump();
        Invincible();
        damage = Mathf.Max(0f, damage - Time.deltaTime);
    }

    private void MovePlayer() {
        Vector3 gunArmTarget = moveInput.magnitude < Mathf.Epsilon ? (Vector2)shoulder.position + new Vector2(direction, 0f) * 3f : (Vector2)shoulder.position + moveInput.normalized * 3f;
        gunArm.position = Vector3.Lerp(gunArm.position, gunArmTarget, 0.1f);

        moving = Mathf.Abs(moveInput.x) > (Mathf.Abs(moveInput.y) * 0.5);
        float velocityX = 0f;
        if (moving) {
            direction = Mathf.Sign(moveInput.x);
            velocityX = 1f;
            //myRigidbody.velocity = new Vector2(direction * moveSpeed, myRigidbody.velocity.y);
        }
        if (damage <= 0f) {
            myRigidbody.velocity = new Vector2(direction * moveSpeed * velocityX, myRigidbody.velocity.y);
        }


        //myRigidbody.velocity = new Vector2(Mathf.Clamp(myRigidbody.velocity.x, -maxSpeed * sprinting, maxSpeed * sprinting), Mathf.Clamp(myRigidbody.velocity.y, -terminalVelocity, terminalVelocity));
    }

    private void DirectionFacing() {
        facing = Mathf.Abs(moveInput.x) > Mathf.Epsilon ? Mathf.Sign(moveInput.x) : transform.localScale.x;
        transform.localScale = new Vector3(facing, 1f);
    }
    
    public void Animations() {
        myAnimator.SetBool("isRunning", moving);
        myAnimator.SetBool("onGround", onGround);
    }
    
    private void Jump() {
        coyoteTimeCounter = onGround ? coyoteTime : coyoteTimeCounter - Time.deltaTime;
        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0f) {
            body.Jumping();
            coyoteTimeCounter = 0;
            float jumpPower = jumpButton ? jumpVelocity : jumpVelocity * 0.4f;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpPower);
        }
    }

    public void Shoot(bool isPressed) {
        /*
        Vector2 aimDirection = moveInput - Vector2.zero;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        Vector2 shootDir = moveInput == Vector2.zero ? new Vector2(direction, 0f) : moveInput;
        newBullet.GetComponent<PlayerBullet>().Direction(shootDir);
        */
        gun.Fire(isPressed);
    }

    private void StopAllMotion() {
        myRigidbody.velocity = Vector2.zero;
    }

    private void Invincible() {
        if (iFrames > 0) {
            iFrames = Mathf.Max(0f, iFrames - Time.deltaTime);
            float alpha = Mathf.Sin(Time.realtimeSinceStartup * 100) * 0.5f;
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for(int i = 0; i < sprites.Length; i++) {
                if(sprites[i] != null) {
                    sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, iFrames > 0 ? 1 - alpha : 1f);
                }
            }
        }
    }

    void OnMove(InputValue value) {

        if (!isAlive) { return; }

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {

        if (!isAlive) { return; }

        if (value.isPressed) {
            jumpButton = true;
            jumpBufferCounter = jumpBufferTime;
        } else {
            jumpButton = false;
            if (coyoteTimeCounter < coyoteTime && myRigidbody.velocity.y > 0) {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, myRigidbody.velocity.y * 0.4f);
            }
        }
    }

    void OnFire(InputValue value) {
        Shoot(value.isPressed);
    }

    public void Possess(bool possess) {
        possessed = possess;
        if (possess) {
            FrameSwitcher frameSwitcher = FindObjectOfType<FrameSwitcher>();
            frameSwitcher.SetFrame(1);
            iFrames = iFramesTime;
        } else {
            myAnimator.SetBool("isRunning", false);
        }
    }

    private void Damaged() {
        iFrames = iFramesTime;
        damage = damageTime;
    }
}
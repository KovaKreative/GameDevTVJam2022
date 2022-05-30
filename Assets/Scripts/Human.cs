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

    float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    bool moving = false;
    bool isAlive = true;
    bool possessed = false;

    float direction = 1f;

    float facing = 1f;

    bool onGround = true;

    bool holdPosition = false;

    Vector2 originalScale = Vector2.one;

    [SerializeField] Gun gun;
    [SerializeField] Animator myAnimator;
    [SerializeField] Transform gunArm;
    [SerializeField] Transform shoulder;
    Rigidbody2D myRigidbody;
    Body body;
    Vector2 moveInput;

    // Start is called before the first frame update
    void Start() {
        originalScale = transform.localScale;
        myRigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<Body>();
        body.AssignTypeName("Biped");
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

        if (holdPosition) {
            myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);
        }
    }

    private void DirectionFacing() {
        facing = Mathf.Abs(moveInput.x) > Mathf.Epsilon ? Mathf.Sign(moveInput.x) : transform.localScale.x;
        transform.localScale = new Vector3(facing, 1f) * originalScale;
    }
    
    public void Animations() {
        myAnimator.SetBool("isRunning", Mathf.Abs(myRigidbody.velocity.x) > 0.1f);
        myAnimator.SetBool("onGround", onGround);
    }
    
    private void Jump() {
        if (onGround) {
            body.Jumping();
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpVelocity);
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
        if (value.isPressed) { Jump(); }
    }

    void OnFire(InputValue value) {
        Shoot(value.isPressed);
    }

    void OnStopMoving(InputValue value) {
        if (value.isPressed) {
            holdPosition = true;
        } else {
            holdPosition = false;
        }
    }

    public void Possess(bool possess) {
        possessed = possess;
        if (possess) {
            CameraOperations frameSwitcher = FindObjectOfType<CameraOperations>();
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
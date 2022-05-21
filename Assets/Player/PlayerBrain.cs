using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBrain : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float jumpVelocity = 6f;
    [SerializeField] float terminalVelocity = 10f;
    [SerializeField] float coyoteTime = 0.1f;
    private float coyoteTimeCounter;
    [SerializeField] float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;

    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask bodyLayerMask;

    bool onGround = true;
    bool moving = false;
    bool isAlive = true;
    bool isPossessing = false;

    float direction = 1f;

    float facing = 1f;

    bool jumpButton = false;

    RaycastHit2D feet;

    Animator myAnimator;
    Rigidbody2D myRigidbody;
    BoxCollider2D myCollider;
    Vector2 moveInput;

    GameObject player;
    GameObject body = null;

    // Start is called before the first frame update
    void Start() {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();
        player = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update() {
        if (!isAlive) {
            StopAllMotion();
            return;
        }

        isPossessing = body != null;
        myRigidbody.simulated = !isPossessing;

        if (isPossessing) {
            return;
        }

        DirectionFacing();
        //Animations();
        MovePlayer();
        onGround = IsGrounded();
        Jump();
    }

    private void MovePlayer() {

        moving = Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y);

        if (moving) {
            direction = Mathf.Sign(moveInput.x);
            myRigidbody.velocity = new Vector2(direction * moveSpeed, myRigidbody.velocity.y);
        } else {
            myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);
        }

        //myRigidbody.velocity = new Vector2(Mathf.Clamp(myRigidbody.velocity.x, -maxSpeed * sprinting, maxSpeed * sprinting), Mathf.Clamp(myRigidbody.velocity.y, -terminalVelocity, terminalVelocity));
    }

    private void DirectionFacing() {
        facing = Mathf.Abs(moveInput.x) > Mathf.Epsilon ? Mathf.Sign(moveInput.x) : transform.localScale.x;
        transform.localScale = new Vector3(facing, 1f);
    }

    /*
    private void Animations() {
        myAnimator.SetBool("isRunning", moving);
        myAnimator.SetBool("onGround", onGround);
    }
    */

    private void Jump() {
        coyoteTimeCounter = onGround ? coyoteTime : coyoteTimeCounter - Time.deltaTime;
        jumpBufferCounter = Mathf.Max(0, jumpBufferCounter - Time.deltaTime);
        if (coyoteTimeCounter > 0 && jumpBufferCounter > 0f) {
            coyoteTimeCounter = 0;
            float jumpPower = jumpButton ? jumpVelocity : jumpVelocity * 0.4f;
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpPower);
        }

    }

    public void OnCollisionEnter2D(Collision2D collision) {
        /*
        EnemyStats enemy = collision.gameObject.GetComponent<EnemyStats>();

        if (enemy != null) {
            if (stats.PlayerDamaged(enemy.GetDamage())) {
                Die();
            }
            return;
        }
        */
    }

    private void Die() {
        /*
        if (!isAlive) { return; }
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpVelocity * 0.8f);
        onGround = false;
        isAlive = false;
        myAnimator.SetTrigger("isDead");
        stats.ProcessPlayerDeath();
        */
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        /*
        PickUp pickup = collision.gameObject.GetComponent<PickUp>();

        if (pickup != null) {
            pickup.PickedUp();
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Hazard")) {
            stats.PlayerDamaged(-1);
            Die();
        }
        */
    }

    private bool IsGrounded() {
        float vSpeed = Mathf.Abs(myRigidbody.velocity.y);
        feet = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, 1f, platformLayerMask);
        Debug.DrawRay(myCollider.bounds.center, Vector2.down, Color.red);
        return feet.collider != null;
    }

    private void StopAllMotion() {
        myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x) < Mathf.Epsilon ? 0f : myRigidbody.velocity.x * 0.9f, onGround ? 0f : myRigidbody.velocity.y);
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

    void OnPossess() {
        if (isPossessing) {
            player.transform.DetachChildren();
            transform.parent = player.transform;
            body.GetComponent<Body>().DynamicBody(false);
            body = null;
        } else {
            RaycastHit2D newBody = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, 1f, bodyLayerMask);
            Debug.Log(newBody);
            if (newBody.collider.gameObject.GetComponent<Body>() != null) {
                transform.position = new Vector2(newBody.collider.transform.position.x, transform.position.y);
                direction = newBody.transform.localScale.x;
                body = newBody.collider.gameObject;
                body.transform.parent = player.transform;
                transform.parent = body.transform;
                body.GetComponent<Body>().DynamicBody(true);

            }
        }
    }

    void OnFire() {

    }


}

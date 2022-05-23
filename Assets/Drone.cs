using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drone : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float acceleration = 2f;

    [SerializeField] GameObject bullet;

    bool moving = false;
    bool isAlive = true;
    bool possessed = false;

    float direction = 1f;

    float facing = 1f;

    bool jumpButton = false;

    Animator myAnimator;
    Rigidbody2D myRigidbody;
    Body body;
    Vector2 moveInput;

    // Start is called before the first frame update
    void Start() {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<Body>();
    }

    // Update is called once per frame
    void Update() {

        if (!possessed) {
            return;
        }
        if (!isAlive) {
            StopAllMotion();
            return;
        }
        DirectionFacing();
        //Animations();
        MovePlayer();
    }

    private void MovePlayer() {

        moving = moveInput != Vector2.zero;

        if (moving) {
            if (myRigidbody.velocity.magnitude <= moveSpeed) {
                myRigidbody.AddForce(moveInput * acceleration);
            }
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

    public void Shoot() {
        Vector2 aimDirection = moveInput - Vector2.zero;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        Vector2 shootDir = moveInput == Vector2.zero ? new Vector2(direction, 0f) : moveInput;
        newBullet.GetComponent<PlayerBullet>().Direction(shootDir);
    }

    private void StopAllMotion() {
        myRigidbody.velocity = Vector2.zero;
    }

    void OnMove(InputValue value) {

        if (!isAlive) { return; }

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {

        if (!isAlive) { return; }

        if (value.isPressed) {

        } else {

        }
    }

    void OnFire() {
        Shoot();
    }

    public void Possess(bool possess) {
        possessed = possess;
    }
}
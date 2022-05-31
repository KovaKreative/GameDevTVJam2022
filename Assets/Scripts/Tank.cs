using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tank : MonoBehaviour
{
    [SerializeField] float acceleration = 3f;
    [SerializeField] float jumpVelocity = 7f;
    [SerializeField] float terminalVelocity = 10f;
    [SerializeField] float cannonRotateSpeed = 5f;

    [SerializeField] Gun gun;

    bool moving = false;
    bool isAlive = true;
    bool possessed = false;

    float direction = 1f;

    float facing = 1f;

    Animator myAnimator;
    Rigidbody2D myRigidbody;
    Body body;
    GameObject head;
    Vector2 moveInput;

    // Start is called before the first frame update
    void Start() {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody2D>();
        body = GetComponent<Body>();
        body.AssignTypeName("Tank");
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
        DirectionFacing();
        //Animations();
        MovePlayer();
        MoveCannon();
    }

    private void MovePlayer() {

        moving = Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y);

        if (moving && body.IsGrounded()) {
            direction = Mathf.Sign(moveInput.x);
            myRigidbody.AddForce(Vector2.right * direction * acceleration);
            //myRigidbody.velocity = new Vector2(direction * moveSpeed, myRigidbody.velocity.y);
        }
    }

    private void MoveCannon() {
        bool pivot = Mathf.Abs(moveInput.x) < Mathf.Abs(moveInput.y);
        
        if (pivot) {
            gun.Rotate(moveInput.y * cannonRotateSpeed);
        }
    }

    private void DirectionFacing() {
        facing = Mathf.Abs(moveInput.x) > Mathf.Epsilon ? Mathf.Sign(moveInput.x) : transform.localScale.x;
        transform.localScale = new Vector3(facing, 1f);
        //head.transform.localScale = new Vector3(facing, 1f);
    }

    /*
    private void Animations() {
        myAnimator.SetBool("isRunning", moving);
        myAnimator.SetBool("onGround", onGround);
    }
    */

    private void Jump() {
        if (body.IsGrounded()) {
            body.Jumping();
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpVelocity);
        }
    }

    public void Shoot(bool isPressed) {
        Vector2 aimDirection = moveInput - Vector2.zero;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gun.Fire(isPressed);
        //GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        //Vector2 shootDir = moveInput == Vector2.zero ? new Vector2(direction, 0f) : moveInput;
        //newBullet.GetComponent<PlayerBullet>().Direction(shootDir);
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
        if (value.isPressed) { Jump(); }
    }

    void OnFire(InputValue value) {
        Shoot(value.isPressed);
    }

    public void Possess(bool possess) {
        if (possess) {
            head = GetComponentInChildren<PlayerHead>().gameObject;
            CameraOperations frameSwitcher = FindObjectOfType<CameraOperations>();
            frameSwitcher.SetFrame(2);
        } else {
            head = null;
        }
        possessed = possess;
    }

}

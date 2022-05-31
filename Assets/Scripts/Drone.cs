using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drone : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float acceleration = 2f;

    [SerializeField] Gun gun;
    [SerializeField] float damageTime = 0.3f;
    float damage = 0f;
    [SerializeField] float iFramesTime = 1f;
    float iFrames = 0f;

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
        body.AssignTypeName("Drone");
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
        Invincible();

    }

    private void MovePlayer() {

        moving = moveInput != Vector2.zero;
        if (moving) {
            if (myRigidbody.velocity.magnitude <= moveSpeed) {
                myRigidbody.AddForce(moveInput * acceleration);
                //myRigidbody.AddTorque(Mathf.Abs(myRigidbody.rotation) < 30f ? 0.1f * -facing : 0f);
            }
        }
        myRigidbody.SetRotation(-myRigidbody.velocity.x);
    }

    private void MoveCannon() {
        gun.Rotate(90 * myRigidbody.velocity.x/moveSpeed);
    }

    private void DirectionFacing() {
        facing = Mathf.Abs(moveInput.x) > Mathf.Epsilon ? Mathf.Sign(moveInput.x) : facing;
        head.transform.localScale = new Vector3(facing, 1f);
    }

    /*
    private void Animations() {
        myAnimator.SetBool("isRunning", moving);
        myAnimator.SetBool("onGround", onGround);
    }
    */

    public void Shoot(bool isPressed) {
        gun.Fire(isPressed);
        /*
        Vector2 aimDirection = moveInput - Vector2.zero;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
        Vector2 shootDir = moveInput == Vector2.zero ? new Vector2(direction, 0f) : moveInput;
        newBullet.GetComponent<PlayerBullet>().Direction(shootDir);
        */
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

    void OnFire(InputValue value) {
        Shoot(value.isPressed);
    }

    private void Invincible() {
        if (iFrames > 0) {
            iFrames = Mathf.Max(0f, iFrames - Time.deltaTime);
            float alpha = Mathf.Sin(Time.realtimeSinceStartup * 100) * 0.5f;
            SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < sprites.Length; i++) {
                if (sprites[i] != null) {
                    sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, iFrames > 0 ? 1 - alpha : 1f);
                }
            }
        }
    }

    public void Possess(bool possess) {
        if (possess) {
            head = GetComponentInChildren<PlayerHead>().gameObject;
            CameraOperations frameSwitcher = FindObjectOfType<CameraOperations>();
            frameSwitcher.SetFrame(3);
        } else {
            head = null;
        }
        possessed = possess;
    }

    private void Damaged() {
        iFrames = iFramesTime;
    }
}
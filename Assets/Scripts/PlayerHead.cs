using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHead : MonoBehaviour
{
    [SerializeField] float acceleration = 0.1f;
    [SerializeField] float groundFriction = 0.96f;
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float jumpVelocity = 6f;

    [SerializeField] GameObject laserBullet;
    [SerializeField] Transform laserOrigin;
    [SerializeField] AudioClip laserSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] GameObject explosionParticle;
    [SerializeField] ParticleSystem attractParticle;

    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask bodyLayerMask;

    [SerializeField] float iFramesTime = 1f;
    float iFrames = 3f;

    bool onGround = true;
    bool moving = false;
    bool isAlive = true;
    bool isPossessing = false;

    float direction = 1f;

    float facing = 1f;

    bool possess = false;

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

    bool isActive = true;

    // Update is called once per frame
    void FixedUpdate() {
        if (!isAlive || !isActive) {
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
        Invincible();
        AttemptPossession();
    }

    private void MovePlayer() {
        //Debug.DrawRay(myCollider.bounds.center, moveInput.normalized, Color.red);
        moving = Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y);
        myRigidbody.rotation = myRigidbody.rotation * 0.8f;
        if (moving) {
            direction = Mathf.Sign(moveInput.x);
            if (onGround) {
                
                myRigidbody.AddForce(Vector2.right * direction * acceleration);
            } else {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x + (Mathf.Sign(direction) * 0.1f), myRigidbody.velocity.y);
            }
        }
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
        if (onGround) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpVelocity);
        }
        //Debug.DrawRay(myCollider.bounds.center, Vector2.down * myCollider.bounds.size.y * 0.6f, Color.red);
    }

    private void Die() {
        isAlive = false;
        gameObject.SetActive(false);
        AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        GameObject explosion = Instantiate(explosionParticle, explosionParticle.transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        Destroy(explosion, 1f);
        GetComponentInParent<Player>().PlayerDead();
    }

    private bool IsGrounded() {
        feet = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.size.y * 0.6f, platformLayerMask);
        return feet.collider != null;
    }

    private void StopAllMotion() {
        myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x) < Mathf.Epsilon ? 0f : myRigidbody.velocity.x * 0.9f, onGround ? 0f : myRigidbody.velocity.y);
    }

    private void AttemptPossession() {
        if (possess && !isPossessing) {
            if (!attractParticle.isPlaying) {
                attractParticle.Play();
            }
            //Body closestBody = null;

            Collider2D checkBody = Physics2D.OverlapCircle(attractParticle.transform.position, 1.5f, bodyLayerMask);

            if(checkBody != null) {
                Possess(checkBody.GetComponent<Body>());
                attractParticle.Stop();
            }
            /*
            Body[] bodies = FindObjectsOfType<Body>();
            foreach (Body body in bodies) {
                float dist = Vector2.Distance(body.transform.position, transform.position);
                if (closestBody == null || dist < Vector2.Distance(closestBody.transform.position, transform.position)) {
                    closestBody = body;
                }
            }

            if (closestBody != null && ) {
                Possess(closestBody);
                attractParticle.Stop();
            }
            */
        } else {
            attractParticle.Stop();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Bullet enemyBullet = collision.gameObject.GetComponent<Bullet>();
        if(enemyBullet != null) {
            Die();
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Hazard")) {
            GetComponentInParent<Player>().PlayerDead();
        }
    }

    void OnMove(InputValue value) {

        if (!isAlive) { return; }

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value) {

        if (!isAlive || isPossessing) { return; }
        if (value.isPressed) { Jump(); }

    }

    void OnPossess(InputValue value) {
        if (isPossessing && value.isPressed) {
            Dispossess(false);
        } else {
            possess = value.isPressed;
        }
    }

    void OnFire(InputValue value) {
        if (!isPossessing && value.isPressed) {
            AudioSource.PlayClipAtPoint(laserSound, transform.position);
            Instantiate(laserBullet, laserOrigin.position, transform.rotation * Quaternion.Euler(0f, 0f, 90f * direction));
        }
    }

    public void Dispossess(bool giveIFrames) {
        player.transform.DetachChildren();
        transform.parent = player.transform;
        Body bodyComponent = body.GetComponent<Body>();
        //bodyComponent.DynamicBody(false);
        bodyComponent.Possessed(false);
        bodyComponent.RevertLayer();
        body = null;
        GetComponentInParent<Player>().HaveBody(null, "none");
        myRigidbody.velocity = Vector2.up * jumpVelocity;
        CameraOperations frameSwitcher = FindObjectOfType<CameraOperations>();
        frameSwitcher.SetFrame(0);
        if (giveIFrames) {
            iFrames = iFramesTime * 2f;
        }
    }

    public void Possess(Body newBody) {
        possess = false;
        transform.position = newBody.transform.position;
        transform.localScale = newBody.transform.localScale;
        transform.localRotation = Quaternion.identity;
        body = newBody.gameObject;
        body.transform.parent = player.transform;
        transform.parent = body.transform;
        body.layer = gameObject.layer;
        GetComponentInParent<Player>().HaveBody(body.GetComponent<Body>(), newBody.GetTypeName());
        body.GetComponent<Body>().Possessed(true);
    }

    private void Invincible() {
        if (iFrames > 0) {
            iFrames = Mathf.Max(0f, iFrames - Time.deltaTime);
            float alpha = Mathf.Sin(Time.realtimeSinceStartup * 100) * 0.5f;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            if (sprite != null) {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, iFrames > 0 ? 1 - alpha : 1f);
            }
        }
    }

    public void LevelExit() {
        isActive = false;
    }

}

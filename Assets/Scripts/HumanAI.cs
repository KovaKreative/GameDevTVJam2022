using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HumanAI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float chaseSpeed = 3.5f;

    [SerializeField] float damageTime = 0.3f;
    float damage = 0f;
    [SerializeField] float iFramesTime = 1f;
    float iFrames = 0f;

    [SerializeField] float idleTime = 2f;
    float idleCountdown;

    [SerializeField] float lineOfSightDistance = 10f;
    [SerializeField] float aggroLimitDistance = 10f;

    [SerializeField] Light2D seekLight;

    [SerializeField] LayerMask platformLayerMask;

    Rigidbody2D myRigidbody;
    CapsuleCollider2D myCollider;
    [SerializeField] Animator myAnimator;
    [SerializeField] Transform gunArm;
    [SerializeField] Transform shoulder;
    [SerializeField] Gun gun;


    bool moving = false;
    bool onGround = true;
    bool shooting = false;

    float directionFacing = 1f; 
    
    Transform player;

    Enemy enemy;

    Body body;
    bool aiActive = false;

    private enum STATE
    {
        IDLE, PATROL, AGGRO, DYING
    }

    STATE state = STATE.PATROL;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();
        body = GetComponent<Body>();
        
        player = FindObjectOfType<PlayerHead>().transform;
        if (transform.parent != null) {
            enemy = transform.parent.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                gameObject.layer = enemy.gameObject.layer;
                aiActive = true;
            }
        }
        if(seekLight != null) {
            seekLight.pointLightOuterRadius = lineOfSightDistance;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!aiActive) { return; }
        switch (state) {
            case STATE.IDLE:
                Behaviour();
                PlayerCheck();
                Idle();
                break;
            case STATE.PATROL:
                Behaviour();
                PlayerCheck();
                Patrol();
                break;
            case STATE.AGGRO:
                Aggro();
                break;
            case STATE.DYING:
                Die();
                break;
        }

        onGround = body.IsGrounded();
        moving = myRigidbody.velocity.magnitude > Mathf.Epsilon;
        Animations();
        Invincible();
        damage = Mathf.Max(0f, damage - Time.deltaTime);
    }

    public void Animations() {
        transform.localScale = new Vector2(directionFacing, 1f);
        myAnimator.SetBool("isRunning", moving);
        myAnimator.SetBool("onGround", onGround);
    }

    private void Behaviour() {
        idleCountdown -= Time.deltaTime;
        if (idleCountdown <= 0f) {
            state = state == STATE.IDLE ? STATE.PATROL : STATE.IDLE;
            directionFacing = Random.value < 0.5f ? 1 : -1;
            idleCountdown = Random.Range(idleTime * 0.5f, idleTime);
        }
    }

    void Idle() {
        if (damage <= 0f) {
            myRigidbody.velocity = new Vector2(0f, 0f);
        }
    }

    void Patrol() {
        gunArm.position = (Vector2)shoulder.position + new Vector2(0f, -1f);
        Vector2 colliderCenter = myCollider.bounds.center;
        RaycastHit2D arms = Physics2D.BoxCast(colliderCenter, myCollider.size * 0.5f, 0f, Vector2.right * directionFacing, 0.3f, platformLayerMask);
        RaycastHit2D feet = Physics2D.Raycast(colliderCenter, new Vector2(directionFacing, -1f), myCollider.size.y, platformLayerMask);

        //Debug.DrawRay(colliderCenter, new Vector2(directionFacing, -1f));
        if (damage <= 0f) {
            myRigidbody.velocity = new Vector2(directionFacing * moveSpeed, myRigidbody.velocity.y);
        }

        if (arms.collider != null || feet.collider == null) {
            directionFacing = -directionFacing;
        }

    }

    void Aggro() {
        myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);

        directionFacing = Mathf.Sign(player.position.x - transform.position.x);

        gunArm.position = player.position;
        if (shooting) {
            gun.EnemyFire(shooting);
            shooting = false;
        }

        if (Vector3.Distance(transform.position, player.position) < aggroLimitDistance) {
            idleCountdown -= Time.deltaTime;
            if (idleCountdown <= 0f) {

                shooting = true;
                idleCountdown = Random.Range(idleTime * 0.5f, idleTime);
            }
        } else {
            state = STATE.IDLE;
        }

    }

    void PlayerCheck() {
        Transform head = GetComponentInChildren<Transform>();
        bool facingPlayer = Mathf.Sign(player.position.x - transform.position.x) == Mathf.Sign(directionFacing);
        bool obstacle = Physics2D.Linecast(head.position, player.position, platformLayerMask);
        if(Vector3.Distance(transform.position, player.position) < lineOfSightDistance && facingPlayer && !obstacle) {
            state = STATE.AGGRO;
            gunArm.position = player.position;
        }
    }

    public void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lineOfSightDistance);
    }

    void Die() {
        if (aiActive) {
            aiActive = false;


            foreach (Transform child in transform) {
                if (child.gameObject.tag == "Enemy Head") {
                    Destroy(child.gameObject);
                }
            }

            enemy.FinishDieSequence();
        }
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
    private void Damaged() {
        iFrames = iFramesTime;
        damage = damageTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class DroneAI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float chaseSpeed = 3.5f;
    [SerializeField] float acceleration = 2f;
    [SerializeField] float lineOfSightDistance = 10f;

    [SerializeField] float patrolRadius = 15f;
    Vector2 startPosition;
    Vector2 targetPosition;

    [SerializeField] float iFramesTime = 1f;
    float iFrames = 0f;

    [SerializeField] float idleTime = 2f;
    float idleCountdown;

    [SerializeField] Light2D seekLight;

    [SerializeField] Gun gun;

    [SerializeField] LayerMask platformLayerMask;


    Rigidbody2D myRigidbody;
    BoxCollider2D myCollider;
    Animator myAnimator;

    float directionFacing = 1f;

    Transform player;

    Enemy enemy;
    bool aiActive = false;

    private enum STATE
    {
        IDLE, PATROL, AGGRO, DYING
    }

    STATE state = STATE.PATROL;

    // Start is called before the first frame update
    void Start() {
        startPosition = transform.position;
        targetPosition = startPosition + (Random.insideUnitCircle * patrolRadius);

        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();

        if (transform.parent != null) {
            enemy = transform.parent.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                gameObject.layer = enemy.gameObject.layer;
                aiActive = true;
            }
        }

        if (seekLight != null) {
            seekLight.pointLightOuterRadius = lineOfSightDistance;
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (player == null) {
            player = FindObjectOfType<PlayerHead>().transform;
        }
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
        Invincible();

    }

    private void MoveCannon() {
        gun.Rotate(90 * myRigidbody.velocity.x / moveSpeed);
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
        myRigidbody.velocity = myRigidbody.velocity * 0.9f;
    }

    void Patrol() {
        MoveCannon();
        if (Vector3.Distance(transform.position, targetPosition) > 1f) {
            Vector3 dir = (targetPosition - (Vector2)transform.position).normalized;
            if (myRigidbody.velocity.magnitude <= moveSpeed) {
                myRigidbody.AddForce(dir * acceleration);
            }
        } else {
            targetPosition = startPosition + (Random.insideUnitCircle * patrolRadius);
        }
        myRigidbody.SetRotation(-myRigidbody.velocity.x);

    }

    void Aggro() {
        if (Vector3.Distance(transform.position, player.position) < lineOfSightDistance) {
            Vector3 dir = player.transform.position - transform.position;
            dir = player.transform.InverseTransformDirection(dir);
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + 90f;
            gun.Rotate(Mathf.LerpAngle(gun.transform.rotation.eulerAngles.z, angle, 0.02f));
            idleCountdown -= Time.deltaTime;
            if (idleCountdown <= 0f) {
                gun.EnemyFire(true);
                idleCountdown = Random.Range(idleTime, idleTime * 1.5f);
            }
        }
    }

    void PlayerCheck() {
        if (Vector3.Distance(transform.position, player.position) < lineOfSightDistance) {
            state = STATE.AGGRO;
        }
    }

    void Die() {
        if (aiActive) {
            aiActive = false;

            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }

            enemy.FinishDieSequence();
        }
    }

    public void OnCollisionEnter2D(Collision2D collision) {
        targetPosition = startPosition + (Random.insideUnitCircle * patrolRadius);
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
    }
}

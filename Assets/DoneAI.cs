using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoneAI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float chaseSpeed = 3.5f;
    [SerializeField] float acceleration = 2f;

    [SerializeField] float patrolRadius;
    Vector2 startPosition;
    Vector2 targetPosition;

    [SerializeField] float idleTime = 2f;
    float idleCountdown;

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

        player = FindObjectOfType<PlayerBrain>().transform;

        if (transform.parent != null) {
            enemy = transform.parent.gameObject.GetComponent<Enemy>();
            if (enemy != null) {
                gameObject.layer = enemy.gameObject.layer;
                aiActive = true;
            }
        }
    }

    // Update is called once per frame
    void Update() {
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
    }

    private void Behaviour() {
        idleCountdown -= Time.deltaTime;
        if (Random.Range(0f, 100f) < 1f) {
            state = state == STATE.IDLE ? STATE.PATROL : STATE.IDLE;
            directionFacing = Random.value < 0.5f ? 1 : -1;
            idleCountdown = Random.Range(idleTime * 0.5f, idleTime);
        }
    }

    void Idle() {
        myRigidbody.velocity = new Vector2(0f, 0f);
    }

    void Patrol() {
        if (Vector3.Distance(transform.position, targetPosition) > 1f) {
            Vector3 dir = (targetPosition - (Vector2)transform.position).normalized;
            if (myRigidbody.velocity.magnitude <= moveSpeed) {
                myRigidbody.AddForce(dir * acceleration);
            }
        } else {
            targetPosition = startPosition + (Random.insideUnitCircle * patrolRadius);
        }
    }

    void Aggro() {
        if (Vector3.Distance(transform.position, player.position) < 3f) {
            Debug.Log("Player close to enemy");
        }
    }

    void PlayerCheck() {
        if (Vector3.Distance(transform.position, player.position) < 3f) {
            print("Close to Player");
            //state = STATE.AGGRO;
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
}

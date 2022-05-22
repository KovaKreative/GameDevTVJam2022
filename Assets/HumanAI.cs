using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAI : MonoBehaviour
{
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float chaseSpeed = 3.5f;

    [SerializeField] float idleTime = 2f;
    float idleCountdown;

    [SerializeField] LayerMask platformLayerMask;

    Rigidbody2D myRigidbody;
    BoxCollider2D myCollider;
    Animator myAnimator;

    bool moving = false;

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
    void Start()
    {
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
    void Update()
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
        Vector2 colliderCenter = myCollider.bounds.center;
        RaycastHit2D arms = Physics2D.BoxCast(colliderCenter, myCollider.size * 0.5f, 0f, Vector2.right * directionFacing, 0.3f, platformLayerMask);
        RaycastHit2D feet = Physics2D.Raycast(colliderCenter, new Vector2(directionFacing, -1f), myCollider.size.y, platformLayerMask);

        Debug.DrawRay(colliderCenter, new Vector2(directionFacing, -1f));
        myRigidbody.velocity = new Vector2(directionFacing * moveSpeed, 0f);
        if (arms.collider != null || feet.collider == null) {
            directionFacing = -directionFacing;
        }
    }

    void Aggro() {
        if(Vector3.Distance(transform.position, player.position) < 3f) {
            Debug.Log("Player close to enemy");
        }
    }

    void PlayerCheck() {
        if(Vector3.Distance(transform.position, player.position) < 3f) {
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

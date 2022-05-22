using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] int health = 1;

    bool onGround = true;

    [SerializeField] LayerMask platformLayerMask;
    RaycastHit2D feet;
    int layerIndex;

    Rigidbody2D myRigidbody;
    BoxCollider2D myCollider;

    // Start is called before the first frame update
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<BoxCollider2D>();

        layerIndex = gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {
        bool abandoned = GetComponentInParent<Player>() == null && GetComponentInParent<Enemy>() == null && onGround;
        DynamicBody(!abandoned);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Platform")) {
            float vSpeed = Mathf.Abs(myRigidbody.velocity.y);
            feet = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, 0.6f, platformLayerMask);
            Debug.DrawRay(myCollider.bounds.center, Vector2.down, Color.red);
            onGround = feet.collider != null;
        }
    }

    public bool IsGrounded() {
        return onGround;
    }

    public void Jumping() {
        onGround = false;
    }

    public void DynamicBody(bool dynamic) {
        myRigidbody.bodyType = dynamic ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }

    public void RevertLayer() {
        gameObject.layer = layerIndex;
    }

    public void Damage(int damage) {
        health = Mathf.Max(0, health - damage);
        if(health <= 0) {
            Die();
        }
    }

    private void Die() {
        if (health <= 0) {
            Destroy(gameObject);
        } else {
            GetComponent<Body>().RevertLayer();
        }
    }

    public int GetHealth() {
        return health;
    }

    public void Possessed() {
        SendMessage("Possess", true);
        DynamicBody(true);
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] int health = 1;

    bool onGround = true;

    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask bodyLayerMask;
    RaycastHit2D feet;
    int layerIndex;

    Rigidbody2D myRigidbody;
    Collider2D myCollider;

    // Start is called before the first frame update
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        layerIndex = gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {
        bool abandoned = GetComponentInParent<Player>() == null && GetComponentInParent<Enemy>() == null && onGround;
        DynamicBody(!abandoned);
        OnGround();
    }

    void OnCollisionEnter2D(Collision2D collision) {

    }

    private void OnGround() {
        feet = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.size.y * 0.6f, platformLayerMask);
        if (feet.collider == null) {
            feet = Physics2D.Raycast(myCollider.bounds.center, Vector2.down, myCollider.bounds.size.y * 0.6f, bodyLayerMask);
        }
        onGround = feet.collider != null;
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

    public void Possessed(bool possess) {
        SendMessage("Possess", possess);
        DynamicBody(possess);
    }
    
}

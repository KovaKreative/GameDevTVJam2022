using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{

    [SerializeField] int maxHealth = 10;
    int health = 0;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] GameObject explosionParticle;

    bool onGround = true;

    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] LayerMask bodyLayerMask;
    RaycastHit2D feet;
    int layerIndex;

    Rigidbody2D myRigidbody;
    Collider2D myCollider;

    string bodyType = "none";

    // Start is called before the first frame update
    void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();

        layerIndex = gameObject.layer;
        health = maxHealth;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool abandoned = GetComponentInParent<Player>() == null && GetComponentInParent<Enemy>() == null && onGround;
        DynamicBody(!abandoned);
        OnGround();
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Hazard")) {
            FindObjectOfType<GameSession>().PlayerDeath();
        }
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

    public void Damage(int damage, float force, Vector3 position) {
        myRigidbody.velocity = new Vector2(Mathf.Sign(transform.position.x - position.x) * Mathf.Max(0, force - myRigidbody.mass), myRigidbody.velocity.y);
        health = Mathf.Max(0, health - damage);
        if (health <= 0) {
            Die();
        } else {
            SendMessage("Damaged");
        }
    }

    private void Die() {
        if (health <= 0) {
            PlayerHead playerHead = GetComponentInChildren<PlayerHead>();
            if (playerHead != null) {
                playerHead.Dispossess();
            }
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
            GameObject explosion = Instantiate(explosionParticle, explosionParticle.transform.position, Quaternion.identity);
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 1f);
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
    public float GetHealthPercentage() {
        return ((float)health / (float)maxHealth) * 100f;
    }

    public string GetTypeName() {
        return bodyType;
    }

    public void AssignTypeName(string name) {
        bodyType = name;
    }
}
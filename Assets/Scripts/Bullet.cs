using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] int damage = 10;
    [SerializeField] float impactForce = 10f;
    [SerializeField] GameObject sparksParticle;
    [SerializeField] Transform impactPoint;
    [SerializeField] AudioClip damageSound;
    [SerializeField] AudioClip impactSound;

    bool projectile = false;
    Camera cam;

    //Vector2 direction = Vector2.right;

    Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        projectile = myRigidbody.bodyType == RigidbodyType2D.Dynamic;
        Fire();

        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (projectile) {
            float angle = Mathf.Atan2(myRigidbody.velocity.y, myRigidbody.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
        }

        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        if (viewPos.x > 1.1f || viewPos.x < -0.1f || viewPos.y > 1.2f || viewPos.y < -0.2f) {
            Destroy(gameObject);
        }
    }

    private void Fire() {
        float z = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float xSpeed = Mathf.Sin(z);
        float ySpeed = -Mathf.Cos(z);
        myRigidbody.velocity = new Vector2(xSpeed * speed, ySpeed * speed);
        
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Body body = collision.gameObject.GetComponent<Body>();
        AudioClip sound = impactSound;
        if(body != null) {
            body.Damage(damage, impactForce, impactPoint.position);
            sound = damageSound;
        } else {
            Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
            if (enemy != null) {
                sound = damageSound;
                enemy.Damage(damage);
            }
        }

        BossWeakPoint boss = collision.gameObject.GetComponent<BossWeakPoint>();
        if(boss != null) {
            float recoil = Mathf.Sign(collision.transform.position.x - transform.position.x);
            FindObjectOfType<BossAI>().Damage(damage, recoil);
        }

        AudioSource.PlayClipAtPoint(sound, transform.position);
        GameObject impact = Instantiate(sparksParticle, impactPoint.position, Quaternion.identity);
        impact.GetComponent<ParticleSystem>().Play();
        Destroy(impact, 1f);

        

        Destroy(gameObject);
    }
}

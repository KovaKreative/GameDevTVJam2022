using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] int damage = 10;

    Camera cam;

    //Vector2 direction = Vector2.right;

    Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        Fire();

        cam = FindObjectOfType<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        if (viewPos.x > 1f || viewPos.x < 0f || viewPos.y > 1f || viewPos.y < 0f) {
            Destroy(gameObject);
        }
    }

    public void Direction(Vector2 dir) {
        //direction = dir;
    }

    private void Fire() {
        float z = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        float xSpeed = Mathf.Sin(z);
        float ySpeed = -Mathf.Cos(z);
        myRigidbody.velocity = new Vector2(xSpeed * speed, ySpeed * speed);
        
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null) {
            enemy.Damage(damage);
            Destroy(gameObject);
        }

        Body body = collision.gameObject.GetComponent<Body>();
        if(body != null) {
            body.Damage(damage);
            Debug.Log("Bullet hit a body");
        }

        Destroy(gameObject);
    }
}

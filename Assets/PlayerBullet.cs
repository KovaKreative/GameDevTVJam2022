using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] float speed = 8f;

    Rigidbody2D myRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        Fire();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire() {
        myRigidbody.velocity = new Vector2(speed, 0f);
        transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Body body = collision.gameObject.GetComponent<Body>();
        if(body != null) {
            Debug.Log("Bullet hit a body");
        }

        Destroy(gameObject);
    }
}

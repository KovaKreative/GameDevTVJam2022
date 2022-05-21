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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(float dir) {
        myRigidbody.velocity = new Vector2(Mathf.Sign(dir) * speed, 0f);
        transform.localScale = new Vector2(transform.localScale.x * dir, transform.localScale.y);
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Body body = collision.gameObject.GetComponent<Body>();
        if(body != null) {
            Debug.Log("Bullet hit a body");
        }

        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [SerializeField] float speed = 8f;

    Camera cam;

    float direction = 1f;

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

    public void Direction(float dir) {
        direction = Mathf.Sign(dir);
    }

    private void Fire() {
        Debug.Log(transform.forward);
        myRigidbody.velocity = transform.rotation.eulerAngles * speed;
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Body body = collision.gameObject.GetComponent<Body>();
        if(body != null) {
            Debug.Log("Bullet hit a body");
        }

        Destroy(gameObject);
    }
}

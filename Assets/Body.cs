using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour
{
    [SerializeField] int health = 1;

    Rigidbody2D myRigidBody;

    int layerIndex;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        layerIndex = gameObject.layer;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DynamicBody(bool dynamic) {
        myRigidBody.bodyType = dynamic ? RigidbodyType2D.Dynamic : RigidbodyType2D.Static;
    }

    public void RevertLayer() {
        gameObject.layer = layerIndex;
    }
}

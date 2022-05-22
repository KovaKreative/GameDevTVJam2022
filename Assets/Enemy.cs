using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int health = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(int damage) {
        health = Mathf.Max(0, health - damage);
        if (health <= 0) {
            BroadcastMessage("Die");
        }
    }

    public void FinishDieSequence() {
        transform.DetachChildren();
        Destroy(gameObject);
    }
}

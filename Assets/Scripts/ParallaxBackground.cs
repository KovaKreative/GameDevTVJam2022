using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] Material parallaxTexture;
    [SerializeField] float scrollRate = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Parallax();
    }

    private void Parallax() {
        parallaxTexture.mainTextureOffset = new Vector2(transform.position.x * scrollRate, 0f);
    }
}

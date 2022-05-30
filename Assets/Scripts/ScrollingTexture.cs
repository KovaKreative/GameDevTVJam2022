using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    [SerializeField] Material parallaxTexture;
    [SerializeField] float scrollRate = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        parallaxTexture.mainTextureOffset += new Vector2(scrollRate, 0f);
    }
}

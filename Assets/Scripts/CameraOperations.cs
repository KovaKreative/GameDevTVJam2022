using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraOperations : MonoBehaviour
{
    private float[] orthoFrames = { 7, 8, 10, 9 };
    private int currentFrame = 0;

    private CinemachineVirtualCamera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
        camera.Follow = FindObjectOfType<PlayerHead>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        float currentOrtho = camera.m_Lens.OrthographicSize;
        camera.m_Lens.OrthographicSize = Mathf.Lerp(currentOrtho, orthoFrames[currentFrame], 0.02f);
    }

    public void SetFrame(int frame) {
        currentFrame = frame;
    }


}

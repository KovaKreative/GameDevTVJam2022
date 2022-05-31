using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraOperations : MonoBehaviour
{
    private float[] orthoFrames = { 7, 10, 12, 11 };
    private int currentFrame = 0;

    private CinemachineVirtualCamera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<CinemachineVirtualCamera>();
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

    public void AssignNewFollow() {
        PlayerHead player = FindObjectOfType<PlayerHead>();
        if (player != null) {
            camera.Follow = player.gameObject.transform;
        }
    }
}

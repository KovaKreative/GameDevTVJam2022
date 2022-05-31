using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleScreen : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void StartGame() {
        SceneManager.LoadSceneAsync(1);
    }

    public void Credits() {
        SceneManager.LoadSceneAsync(SceneManager.sceneCountInBuildSettings - 1);
    }

    public void EndGame() {
        SceneManager.LoadSceneAsync(0);
    }

    void OnQuit() {
        Application.Quit();
    }
}

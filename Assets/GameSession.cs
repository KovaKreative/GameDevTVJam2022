using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] float sceneResetDelay = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayerDeath() {
        StartCoroutine("ResetScene");
    }

    IEnumerator ResetScene() {
        yield return new WaitForSeconds(sceneResetDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

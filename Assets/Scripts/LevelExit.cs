using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] bool customLevel = false;
    [SerializeField] string customLevelName = "MainMenu";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision) {
        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null) {
            if (customLevel) {
                StartCoroutine("LoadCustomLevel");
            } else {
                StartCoroutine("LoadNextLevel");
            }
        }
    }

    IEnumerator LoadCustomLevel() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetSceneByName(customLevelName).buildIndex);
    }

    IEnumerator LoadNextLevel() {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] float sceneResetDelay = 2f;

    [SerializeField] ProgressBar healthBar;
    int bodyHealth = 0;
    int maxHealth = 1;
    Body body;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        bodyHealth = body == null ? 0 : (int)body.GetHealth();
        maxHealth = body == null ? 1 : (int)body.GetMaxHealth();
        healthBar.BarValue(bodyHealth, maxHealth);
    }

    public void HaveBody (Body getBody, string bodyType){
        body = getBody;
        healthBar.Title = bodyType;
    }

    IEnumerator ResetScene() {
        yield return new WaitForSeconds(sceneResetDelay);
        Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerDead() {
        StartCoroutine("ResetScene");
    }
}

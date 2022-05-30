using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] float sceneResetDelay = 2f;

    [SerializeField] TextMeshProUGUI currentBody;
    [SerializeField] ProgressBar healthBar;
    int bodyHealth = 0;
    Body body;

    public void Awake() {
        Player[] players = FindObjectsOfType<Player>();
        if (players.Length > 1) {
            foreach (Player player in players) {
                if (player != GetComponent<Player>()) {
                     {
                        player.GoToSpawnPoint(transform.position);
                        Destroy(gameObject);
                        break;
                    }
                }
            }
        } else {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start() {
        currentBody = FindObjectOfType<BodyTypeText>().GetComponent<TextMeshProUGUI>();
        healthBar = FindObjectOfType<HealthBar>().GetComponent<ProgressBar>();
    }

    // Update is called once per frame
    void Update() {
        bodyHealth = body == null ? 0 : (int)body.GetHealthPercentage();
        healthBar.BarValue = bodyHealth;
    }

    public void HaveBody (Body getBody, string bodyType){
        body = getBody;
        currentBody.text = bodyType;
    }

    IEnumerator ResetScene() {
        yield return new WaitForSeconds(sceneResetDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerDead() {
        StartCoroutine("ResetScene");
    }

    public void GoToSpawnPoint(Vector3 pos) {
        BroadcastMessage("ResetPosition", pos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentBody;
    [SerializeField] ProgressBar healthBar;
    int bodyHealth = 0;
    Body body;

    // Start is called before the first frame update
    void Start() {

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
}

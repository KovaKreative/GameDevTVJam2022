using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour

{
    [SerializeField] int maxHealth = 1000;
    int health;

    [SerializeField] Transform head;
    Vector2 headOrigin;
    Color originalHeadColour;
    [SerializeField] Transform leftHand;
    [SerializeField] Transform rightHand;

    [SerializeField] SpriteRenderer headSprite;

    [SerializeField] GameObject laser;
    [SerializeField] Transform laserBarrel;

    [SerializeField] GameObject shell;
    [SerializeField] Transform shellBarrel;

    [SerializeField] float telegraphTime = 2f;
    [SerializeField] float idleTime = 3f;
    [SerializeField] float iFrames = 0f;
    float iFrameTimer = 0f;
    bool nextActionLaser = true;

    [SerializeField] Transform player;

    [SerializeField] ProgressBar healthBar;

    [SerializeField] ParticleSystem deathExplosion;
    [SerializeField] AudioSource myAudio;
    [SerializeField] AudioClip shellLaunch;
    [SerializeField] AudioClip laserShoot;

    bool isAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        headOrigin = head.position;
        StartCoroutine("Idle");
        myAudio.GetComponent<AudioSource>();
        originalHeadColour = headSprite.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) {
            player = FindObjectOfType<PlayerHead>().transform;
        }
        head.position = Vector2.Lerp(head.position, headOrigin, 0.05f);
        Color newColor = new Color(Mathf.Min(originalHeadColour.r, headSprite.color.r + 0.1f), Mathf.Min(originalHeadColour.g, headSprite.color.g + 0.1f), Mathf.Min(originalHeadColour.b, headSprite.color.b + 0.1f));
        headSprite.color = newColor;
        iFrameTimer = Mathf.Max(0, iFrameTimer - Time.deltaTime);
        healthBar.BarValue(health, maxHealth);
        if (deathExplosion.isPlaying) {
            if (!myAudio.isPlaying) {
                myAudio.Play();
            }
        }
    }

    IEnumerator Idle() {
        nextActionLaser = Random.value < 0.5f ? true : false;
        yield return new WaitForSeconds(idleTime);
        if (nextActionLaser) {
            StartCoroutine("LaserAim");
        } else {
            StartCoroutine("ShellAim");
        }
    }

    IEnumerator ShellAim() {
        leftHand.position = player.position;
        yield return new WaitForSeconds(Random.Range(telegraphTime, telegraphTime * 1.5f));
        ShellShoot();
    }

    private void ShellShoot() {
        AudioSource.PlayClipAtPoint(shellLaunch, shellBarrel.position);
        Instantiate(shell, shellBarrel.position, shellBarrel.rotation * Quaternion.Euler(0f, 0f, 90f)); //transform.TransformPoint(bulletOffset.x, bulletOffset.y, 0f), transform.rotation * Quaternion.Euler(0f, 0f, rotationOffset));
        StartCoroutine("Idle");
    }

    IEnumerator LaserAim() {
        rightHand.position = player.position;
        yield return new WaitForSeconds(Random.Range(telegraphTime, telegraphTime * 1.5f));
        LaserShoot();
    }

    IEnumerator Victory() {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void LaserShoot() {
        AudioSource.PlayClipAtPoint(laserShoot, laserBarrel.position);
        Instantiate(laser, laserBarrel.position, laserBarrel.rotation * Quaternion.Euler(0f, 0f, 90f)); //.TransformPoint(bulletOffset.x, bulletOffset.y, 0f), transform.rotation * Quaternion.Euler(0f, 0f, rotationOffset));
        StartCoroutine("Idle");
    }


    public void Damage(int damage, float recoilDirection) {
        if(iFrameTimer > 0f) { return; }
        health -= damage;
        iFrameTimer = iFrames;
        head.position = new Vector2(head.position.x + 10f * recoilDirection, head.position.y);
        headSprite.color = Color.red;
        if(health <= 0) {
            StopAllCoroutines();
            deathExplosion.Play();
            isAlive = false;
            StartCoroutine("Victory");
        }
    }
}

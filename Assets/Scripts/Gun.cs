using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject enemyBullet;
    [SerializeField] bool autoFire = false;
    [SerializeField] float fireRate = 3f;
    [SerializeField] AudioClip shootSound;

    float fireCooldown = 0f;
    bool firing = false;

    enum GUN_TYPE { BIPED, TANK, DRONE }

    [SerializeField] GUN_TYPE gunType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Shoot();
    }

    public void Rotate(float rotateBy) {
        //float rotationAngle = transform.rotation.eulerAngles.z;

        switch (gunType) {
            case GUN_TYPE.BIPED:

                break;
            case GUN_TYPE.TANK:
                transform.Rotate(0, 0, rotateBy * 0.1f);
                transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Clamp(transform.localEulerAngles.z, 1, 60));
                break;
            case GUN_TYPE.DRONE:
                float z = transform.eulerAngles.z;
                transform.rotation = Quaternion.Euler(0f, 0f, rotateBy);
                
                //transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(z, rotateBy, 0.1f));
                //transform.Rotate(0, 0, rotateBy * 0.5f);
                //transform.eulerAngles = new Vector3(0f, 0f, Mathf.Clamp((z <= 180) ? z : -(360 - z), -60, 60));
                break;
        }
    }

    private void Shoot() {
        if (firing) {
            Vector3 bulletOffset;
            float rotationOffset = 0f;

            if (fireCooldown <= 0) {
                fireCooldown = fireRate;
                if (!autoFire) { firing = false; }

                switch (gunType) {
                    case GUN_TYPE.BIPED:
                        bulletOffset = new Vector3(1f, 0f, 0f);
                        rotationOffset = 90f * transform.lossyScale.x;
                        break;
                    case GUN_TYPE.TANK:
                        bulletOffset = new Vector3(1f, 0f, 0f);
                        rotationOffset = 90f * transform.lossyScale.x;
                        break;
                    case GUN_TYPE.DRONE:
                        bulletOffset = new Vector3(0f, -1f, 0f);
                        break;
                    default:
                        bulletOffset = Vector2.zero;
                        break;
                }
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
                Instantiate(bullet, transform.TransformPoint(bulletOffset.x, bulletOffset.y, 0f), transform.rotation * Quaternion.Euler(0f, 0f, rotationOffset));

                //GameObject newBullet = 
            }

        }
        fireCooldown -= Time.deltaTime;
    }
    public void EnemyFire(bool shooting) {
        if (shooting) {
            Vector3 bulletOffset;
            float rotationOffset = 0f;

                fireCooldown = fireRate;
                switch (gunType) {
                    case GUN_TYPE.BIPED:
                        bulletOffset = new Vector3(1f, 0f, 0f);
                        rotationOffset = 90f * transform.lossyScale.x;

                        Instantiate(enemyBullet, transform.TransformPoint(bulletOffset.x, bulletOffset.y, 0f), transform.rotation * Quaternion.Euler(0f, 0f, rotationOffset));
                        break;
                    case GUN_TYPE.TANK:
                        bulletOffset = new Vector3(1f, 0f, 0f);
                        rotationOffset = 90f * transform.lossyScale.x;
                        Instantiate(enemyBullet, transform.TransformPoint(bulletOffset.x, bulletOffset.y, 0f), transform.rotation * Quaternion.Euler(0f, 0f, rotationOffset));
                        break;
                    case GUN_TYPE.DRONE:
                        bulletOffset = new Vector3(0f, -0.5f, 0f);
                        Instantiate(enemyBullet, transform.TransformPoint(bulletOffset.x, bulletOffset.y, 0f), transform.rotation * Quaternion.Euler(0f, 0f, rotationOffset));
                        break;
                    default:
                        bulletOffset = Vector2.zero;
                        break;
                }
                //GameObject newBullet = 
        }
    }

    public void Fire(bool isPressed) {
        if (isPressed) {

            firing = true;

        } else {
            firing = false;
            
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public float shootInterval = 1f;
    float interval_t = 0;

    public float projectileSpeed = 380f;

    public Transform TurretBody;
    public Transform TurretHead;


    public float detectionRange = 60f;

    public Transform turretLook;
    public Transform turretFWD;

    bool dead = false;

    public GameObject projectilePrefab;

    public Transform shootPos;

    public Transform warning;
    public Renderer warningRend;

    void Start()
    {
        projectilePrefab = Resources.Load("Projectile") as GameObject;
    }

    void Update()
    {
        bool playerInRange = Vector3.Distance(Camera.main.transform.position, TurretHead.position) < detectionRange;
        warning.position = TurretHead.position + new Vector3(0, 5, -1);
        warning.LookAt(Camera.main.transform.position);

        if (playerInRange) {
            turretLook.LookAt(Camera.main.transform.position);

            if(interval_t > 0) {
                interval_t -= Time.deltaTime;
                if(interval_t <= 0) {
                    ShootAtPlayer();
                    interval_t = shootInterval;
                }
            }
        } else {
            interval_t = shootInterval;
        }

        TurretHead.rotation = Quaternion.Lerp(TurretHead.rotation, playerInRange ? turretLook.rotation : turretFWD.rotation, Time.deltaTime * 8f);

        warningRend.enabled = playerInRange;
        if (playerInRange && !dead) {
            //warningRend.material.SetFloat("_YellowRed", Mathf.Clamp01((1f-interval_t) * 1.3f));
            float yellowRed = interval_t / shootInterval;
            warningRend.material.SetFloat("_YellowRed", 1f - yellowRed);
            if (interval_t > 0.55f)
                warningRend.material.SetFloat("_Flash", Mathf.Clamp01((interval_t + 0.5f)));
            else
                warningRend.material.SetFloat("_Flash", 0f);
        }

    }

    public void ShootAtPlayer() {
        GameObject newProj = Instantiate(projectilePrefab, shootPos.position, Quaternion.identity);
        newProj.GetComponent<ProjectileScript>().playerPos = GameObject.Find("_Player").transform;
        newProj.GetComponent<ProjectileScript>().SetMoveDir();
        newProj.GetComponent<ProjectileScript>().speed = projectileSpeed;
    }

    public void Shot() {
        dead = true;
        gameObject.SetActive(false);
    }
}

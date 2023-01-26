using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public float speed = 10f;
    public float pauseDelay = 1f;
    public float detectRange = 40f;

    public float chargeUpSpeed = 1;

    public float shootCooldown = 1.5f;

    float t = 0;
    float charge_t = 0;
    float cooldown_t = 0;
    bool coolingDown = false;

    public Transform drone;

    public Transform A;
    public Transform B;

    bool moveToA = true;
    bool waiting = false;

    public Transform bladeL;
    public Transform bladeR;

    public Transform Halo_A;
    public Transform Halo_B;
    public LineRenderer LR;

    public Transform hover;

    bool playerDetected = false;
    public Transform droneFWD;
    public Transform droneLookAt;

    bool dead = false;

    public Transform warningLook;
    public Renderer warningRend;

    public GameObject projectilePrefab;
    public Transform projectilePos;

    public Renderer droneRend;

    bool spottedSound = false;

    void Start()
    {
        projectilePrefab = Resources.Load("Projectile") as GameObject;
    }

    void Update()
    {
        Vector3 targetPos = moveToA ? A.localPosition : B.localPosition;
        Vector3 hoverOffset = new Vector3(0, Mathf.Sin(Time.time * 58.5f) * 0.038f, 0);

        //hover.transform.localPosition = hoverOffset;

        playerDetected = Vector3.Distance(Camera.main.transform.position, drone.position) < detectRange;
        if (!spottedSound && playerDetected && !dead) {
            GameObject.Find("_Player").GetComponent<PlayerMovement>().PlayOneShot(6, 0.268f);
            spottedSound = true;
        }
        if (!playerDetected)
            spottedSound = false;
        droneLookAt.position = drone.position;
        warningLook.position = drone.position + new Vector3(0, 5, -1);
        droneRend.material.SetColor("_FresnelColour", Color.Lerp(droneRend.material.GetColor("_FresnelColour"), playerDetected ? Color.red : Color.clear, Time.deltaTime * 4f));

        if (playerDetected) {
            droneLookAt.LookAt(Camera.main.transform.position);
            warningLook.LookAt(Camera.main.transform.position);
        }
        drone.rotation = Quaternion.Lerp(drone.rotation, playerDetected ? droneLookAt.rotation : droneFWD.rotation, Time.deltaTime * 19f);

        if (!waiting && !dead && !playerDetected)
            drone.localPosition = Vector3.Lerp(drone.localPosition, targetPos, Time.deltaTime * speed * 0.15f);



        if (Vector3.Distance(drone.localPosition, targetPos) < 0.21f) {
            t = pauseDelay;
            waiting = true;
            moveToA = !moveToA;
        }

        if (waiting) {
            t -= Time.deltaTime;
            if (t <= 0)
                waiting = false;
        }

        bladeL.Rotate(new Vector3(0, 0, 50 * Time.deltaTime * 298f));
        bladeR.Rotate(new Vector3(0, 0, 50 * Time.deltaTime * 298f));

        if (playerDetected && !dead && !coolingDown) {
            warningRend.enabled = true;
            charge_t += Time.deltaTime * chargeUpSpeed;
            warningRend.material.SetFloat("_YellowRed", Mathf.Clamp01(charge_t * 1.3f));
            if(charge_t > 0.55f)
                warningRend.material.SetFloat("_Flash", Mathf.Clamp01((charge_t + 0.5f)));
            else
                warningRend.material.SetFloat("_Flash", 0f);

            if (charge_t >= 1) {
                ShootAtPlayer();
            }
        }

        if (!playerDetected) {
            charge_t = 0;
            warningRend.enabled = false;
        }

        if (coolingDown) {
            cooldown_t -= Time.deltaTime;
            if (cooldown_t <= 0) {
                charge_t = 0;
                coolingDown = false;
            }
        }
    }

    private void LateUpdate() {
        Halo_A.LookAt(Camera.main.transform.position);
        Halo_B.LookAt(Camera.main.transform.position);
        LR.SetPosition(0,A.position);
        LR.SetPosition(1,B.position);
    }
    public void ShootAtPlayer() {
        coolingDown = true;
        warningRend.enabled = false;
        cooldown_t = shootCooldown;

        GameObject newProj = Instantiate(projectilePrefab, projectilePos.position, Quaternion.identity);
        newProj.GetComponent<ProjectileScript>().playerPos = GameObject.Find("_Player").transform;
        newProj.GetComponent<ProjectileScript>().SetMoveDir();
    }

    public void Shot() {
        //Debug.LogError("GotShot");
        GameObject.Find("_Player").GetComponent<PlayerMovement>().PlayOneShot(7, 0.4f);
        dead = true;
        drone.gameObject.SetActive(false);
        Halo_A.gameObject.SetActive(false);
        Halo_B.gameObject.SetActive(false);
        warningRend.enabled = false;
        LR.enabled = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drone : MonoBehaviour
{
    public float speed = 10f;
    public float pauseDelay = 1f;
    float t = 0;

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

    public float detectRange = 40f;
    bool playerDetected = false;
    public Transform droneFWD;
    public Transform droneLookAt;

    bool dead = false;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 targetPos = moveToA ? A.localPosition : B.localPosition;
        Vector3 hoverOffset = new Vector3(0, Mathf.Sin(Time.time * 58.5f) * 0.038f, 0);

        //hover.transform.localPosition = hoverOffset;

        playerDetected = Vector3.Distance(Camera.main.transform.position, drone.position) < detectRange;
        droneLookAt.position = drone.position;
        if (playerDetected)
            droneLookAt.LookAt(Camera.main.transform.position);
        drone.rotation = Quaternion.Lerp(drone.rotation, playerDetected ? droneLookAt.rotation : droneFWD.rotation, Time.deltaTime * 19f);

        if (!waiting && !dead)
            drone.localPosition = Vector3.Lerp(drone.localPosition, targetPos, Time.deltaTime * speed * 0.15f);



        if (Vector3.Distance(drone.position, targetPos) < 0.1f) {
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


    }

    private void LateUpdate() {
        Halo_A.LookAt(Camera.main.transform.position);
        Halo_B.LookAt(Camera.main.transform.position);
        LR.SetPosition(0,A.position);
        LR.SetPosition(1,B.position);
    }

    public void Shot() {
        //Debug.LogError("GotShot");
        dead = true;
        drone.gameObject.SetActive(false);
        Halo_A.gameObject.SetActive(false);
        Halo_B.gameObject.SetActive(false);
        LR.enabled = false;
    }
}

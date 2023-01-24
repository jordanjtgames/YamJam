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

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 targetPos = moveToA ? A.position : B.position;
        Vector3 hoverOffset = new Vector3(0, Mathf.Sin(Time.time * 28.5f) * 0.08f, 0);

        hover.transform.localPosition = hoverOffset;

        if (!waiting)
            drone.position = Vector3.MoveTowards(drone.position, targetPos, Time.deltaTime * speed);



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
}

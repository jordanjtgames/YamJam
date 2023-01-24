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

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 targetPos = moveToA ? A.position : B.position;

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

        bladeL.Rotate(new Vector3(0, 50 * Time.deltaTime * 498f, 0));
        bladeR.Rotate(new Vector3(0, 50 * Time.deltaTime * 498f, 0));
    }
}

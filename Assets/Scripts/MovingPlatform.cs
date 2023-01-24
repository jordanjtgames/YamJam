using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 10f;
    public float pauseDelay = 1f;
    float t = 0;

    public Transform platform;
    public Transform A;
    public Transform B;

    bool moveToA = true;
    bool waiting = false;

    void Update()
    {
        Vector3 targetPos = moveToA ? A.position : B.position;

        if(!waiting)
            platform.position = Vector3.MoveTowards(platform.position, targetPos, Time.deltaTime * speed);

        if (Vector3.Distance(platform.position, targetPos) < 0.1f) {
            t = pauseDelay;
            waiting = true;
            moveToA = !moveToA;
        }

        if (waiting) {
            t -= Time.deltaTime;
            if(t <= 0)
                waiting = false;
        }
    }
}

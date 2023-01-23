using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public Transform player;
    public Transform hook;

    public Transform gunFWD;
    public Transform Rot_L;
    public Transform Rot_R;
    public Transform Rot_U;
    public Transform Rot_D;

    public Transform Rot_LR;
    public Transform Rot_UD;

    public RawImage crosshairRI;
    public RawImage topLeftRI;
    public RawImage bottomRightRI;

    public float hookRange = 45;

    Vector3 hookPos;

    public Vector2 tViz;

    public RawImage testRI;

    public Transform hook_A;
    public Transform hook_B;
    public Transform hook_C;
    public LineRenderer hookLine;

    public int arcResolution = 10;

    void Start()
    {
        hookLine.positionCount = arcResolution;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        crosshairRI.transform.position = mousePos;

        float Yt = mousePos.y / topLeftRI.transform.position.y;
        float Xt = mousePos.x / bottomRightRI.transform.position.x;

        Quaternion UD = Quaternion.Lerp(Rot_U.localRotation, Rot_D.localRotation, 1f-Yt);
        Quaternion LR = Quaternion.Lerp(Rot_L.localRotation, Rot_R.localRotation, Xt);

        Rot_LR.localRotation = LR;
        Rot_UD.localRotation = UD;

        tViz.x = Xt;
        tViz.y = Yt;



        Vector3 currentHookPos = GameObject.Find("_PoI").transform.position;
        Vector2 currentHookScreenPos = Camera.main.WorldToScreenPoint(currentHookPos);

        if (Vector2.Distance(mousePos, currentHookScreenPos) < hookRange) {
            testRI.enabled = false;
        } else {
            testRI.enabled = true;

        }

        testRI.transform.position = currentHookScreenPos;

        hook_A.position = player.position;
        hook_B.position = player.position + transform.TransformDirection(Vector3.forward * 7f);
        hook_C.position = currentHookPos;

        for (int i = 0; i < arcResolution; i++) {
            float at = 0;
            if (i == 0)
                at = 0;
            else
                at = (float)i / (float)(arcResolution);
            hookLine.SetPosition(i, QuadCurve(hook_A.position, hook_B.position, hook_C.position, at));
        }
    }

    Vector3 QuadCurve(Vector3 start, Vector3 mid, Vector3 end, float t) {
        Vector3 p0 = Vector3.Lerp(start, mid, t);
        Vector3 p1 = Vector3.Lerp(mid, end, t);
        return Vector3.Lerp(p0, p1, t);
    }
}

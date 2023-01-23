using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public Transform allGrabs;
    public List<Transform> grabPoints;

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

    Vector3 targetHookPos;

    public Vector2 tViz;

    public RawImage testRI;

    public Transform hook_A;
    public Transform hook_B;
    public Transform hook_C;
    public LineRenderer hookLine;

    public int arcResolution = 10;

    public Rigidbody rb;
    public Transform currentGrab;

    public AnimationCurve velPowerOverDiist;

    void Awake()
    {
        GenerateGrabsList();
        hookLine.positionCount = arcResolution;
    }

    public void GenerateGrabsList() {
        grabPoints = new List<Transform>();
        foreach (SphereCollider SC in allGrabs.GetComponentsInChildren<SphereCollider>()) {
            grabPoints.Add(SC.transform);
        }
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


        if(currentGrab != null) {
            //Debug.Log("Grabbed");
            rb.useGravity = false;

            float dist = Vector3.Distance(currentGrab.position, player.position);
            Vector3 holdPos = currentGrab.position + new Vector3(0, 0, -20);
            Vector3 vel = (holdPos - player.position).normalized;
            float pow = velPowerOverDiist.Evaluate(dist);
            float mod = Mathf.Clamp(dist * 0.1f,0,1);
            rb.velocity = vel * pow * mod;
        } else {
            rb.useGravity = true;
        }
        

        

        if(grabPoints != null) {
            for (int i = 0; i < grabPoints.Count; i++) {
                Vector3 currentHookPos = grabPoints[i].position;
                Vector2 currentHookScreenPos = Camera.main.WorldToScreenPoint(currentHookPos);

                bool inPlayerRange = Vector3.Distance(player.position, currentHookPos) < 60;

                if (Vector2.Distance(mousePos, currentHookScreenPos) < hookRange && inPlayerRange) {
                    testRI.enabled = false;
                    targetHookPos = currentHookPos;
                    if(Mouse.current.leftButton.ReadValue() == 1) {
                        currentGrab = grabPoints[i];
                    }
                } else {
                    testRI.enabled = true;

                }

                testRI.transform.position = currentHookScreenPos;

                hook_A.position = player.position;
                hook_B.position = player.position + transform.TransformDirection(Vector3.forward * 10f);//7
                hook_C.position = Vector3.Lerp(hook_C.position, targetHookPos, Time.deltaTime * 6f);
            }
        }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


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
    public RawImage crosshairGrabRI;
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

    int arcResolution = 15;

    public Rigidbody rb;
    public Transform currentGrab;

    public AnimationCurve velPowerOverDiist;

    public float gravity = 9f;
    float swingDelay = 0.1f;

    bool isSwinging = false;
    Vector2 swingStartPos;
    Vector3 swingOffset = Vector3.zero;

    float grabStep_t = 0;
    public AnimationCurve stepOverTime;

    bool slowMotion = false;
    public Volume postFX;
    float currentChromAb = 0;
    float currentContrast = 0;
    float currentSat = 0;

    public Transform lookUp;
    public Transform lookFWD;

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


        if(swingDelay > 0) {
            swingDelay -= Time.unscaledDeltaTime;
        }
        hookLine.widthMultiplier = isSwinging ? 0.345f : 0.1f;

        if(currentGrab != null) {
            //Debug.Log("Grabbed");
            grabStep_t += Time.deltaTime;
            Mathf.Clamp01(grabStep_t);

            float dist = Vector3.Distance(currentGrab.position, player.position);
            float holdDistance = 1.115f;
            Vector3 holdPos = currentGrab.position + new Vector3(0, -7 * holdDistance, -25 * holdDistance) + swingOffset;
            Vector3 vel = (holdPos - player.position);//norm
            float pow = velPowerOverDiist.Evaluate(dist);
            float mod = Mathf.Clamp(dist * 0.1f,0,1);
            Vector3 refVel = Vector3.zero;
            float step = Mathf.Lerp(1f, 0.05f, Mathf.Clamp01((dist - 1f) * 0.5f));
            //float finalStep = Mathf.Lerp(step, )
            //step = stepOverTime.Evaluate(grabStep_t);

            rb.velocity = Vector3.SmoothDamp(rb.velocity, vel * pow * mod * 0.25f, ref refVel, step);

            if (dist > 3) {
                //rb.velocity = vel * pow * mod;
            } else {
                //player.position = currentGrab.position;
                //rb.velocity = Vector3.zero;
            }

            crosshairGrabRI.enabled = true;
            crosshairGrabRI.transform.position = Camera.main.WorldToScreenPoint(currentGrab.transform.position);

            player.rotation = Quaternion.Lerp(player.rotation, lookUp.rotation, Time.deltaTime * 3f);

            rb.drag = Mathf.Lerp(25,0,Mathf.Clamp01(dist * 0.5f));
            rb.useGravity = false;
        } else {
            crosshairGrabRI.enabled = false;
            rb.drag = 0;
            rb.useGravity = true;

            player.rotation = Quaternion.Lerp(player.rotation, lookFWD.rotation, Time.deltaTime * 3f);

        }

        if (Keyboard.current.rKey.wasPressedThisFrame) {
            currentGrab = null;
        }
        

        if(grabPoints != null) {
            for (int i = 0; i < grabPoints.Count; i++) {
                Vector3 currentHookPos = grabPoints[i].position;
                Vector2 currentHookScreenPos = Camera.main.WorldToScreenPoint(currentHookPos);

                bool inPlayerRange = Vector3.Distance(player.position, currentHookPos) < 90;

                if (Vector2.Distance(mousePos, currentHookScreenPos) < hookRange && inPlayerRange && !isSwinging) {
                    targetHookPos = currentHookPos;
                    if(Mouse.current.leftButton.wasPressedThisFrame) {
                        if(currentGrab != null) {
                            if(grabPoints[i] != currentGrab) {
                                currentGrab = grabPoints[i];
                                swingDelay = 0.1f;
                                grabStep_t = 0;
                            } else {
                                if(swingDelay <= 0) {
                                    swingStartPos = mousePos;
                                    isSwinging = true;
                                }
                            }
                        } else {
                            currentGrab = grabPoints[i];
                            swingDelay = 0.1f;
                            grabStep_t = 0;
                        }
                    }
                } else {

                }

                testRI.transform.position = currentHookScreenPos;

                hook_A.position = player.position + transform.TransformDirection(Vector3.back);
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

        if (isSwinging) {
            //Debug.Log("SWINGING");
            Vector3 tmpSwingOffset = (mousePos - swingStartPos) * 0.05f;
            float delta = 21;
            float xClamp = Mathf.Clamp(tmpSwingOffset.x,-delta, delta);
            float yClamp = Mathf.Clamp(tmpSwingOffset.y, -delta, delta);
            swingOffset = new Vector3(xClamp, yClamp,0);


            if (Mouse.current.leftButton.ReadValue() == 0) {
                isSwinging = false;
                swingOffset = Vector3.zero;
            }
        }

        if (slowMotion) {

        }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) {
            slowMotion = !slowMotion;
        }

        currentChromAb = Mathf.Lerp(currentChromAb, slowMotion ? 1f : 0, Time.deltaTime * 5f);
        currentContrast = Mathf.Lerp(currentChromAb, slowMotion ? 22f : 0, Time.deltaTime * 5f);
        currentSat = Mathf.Lerp(currentChromAb, slowMotion ? 51f : 0, Time.deltaTime * 5f);

        Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotion ? 0.1f : 1f, Time.deltaTime * 4);
        //Time.fixedTime = Mathf.Lerp(Time.timeScale, slowMotion ? 1f : 0.1f, Time.deltaTime * 4);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        postFX.profile.TryGet(out ChromaticAberration chromAb);
        chromAb.intensity.value = currentChromAb;
        postFX.profile.TryGet(out ColorAdjustments colAdj);
        colAdj.contrast.value = currentContrast;
        colAdj.saturation.value = currentSat;
    }

    private void FixedUpdate() {
        if (currentGrab != null) {

        } else {
            rb.AddForce(Vector3.down * gravity * Time.fixedDeltaTime);
        }
    }

    Vector3 QuadCurve(Vector3 start, Vector3 mid, Vector3 end, float t) {
        Vector3 p0 = Vector3.Lerp(start, mid, t);
        Vector3 p1 = Vector3.Lerp(mid, end, t);
        return Vector3.Lerp(p0, p1, t);
    }
}

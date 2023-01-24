using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;


public class PlayerMovement : MonoBehaviour
{
    Vector2 mousePos;

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

    public float springRange = 4.5f;
    public float maxSpeed = 70;//50
    public float playerRange = 90;
    public float hookRange = 45;

    Vector3 targetHookPos;

    public Vector2 tViz;

    public RawImage testRI;

    public Transform hook_A;
    public Transform hook_B;
    public Transform hook_C;
    public LineRenderer hookLine;
    public LineRenderer hookLine_Aim;

    int arcResolution = 20;//15

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

    public Transform camLook;
    public Transform camLookFWD;
    public Transform camLookAt;

    public RawImage speedRI;
    float speedDist = 0f;
    float swayX = 0f;
    float swayY = 0f;
    public RawImage vignette;

    public Transform camTilt;
    public Transform camTilt_L;
    public Transform camTilt_R;
    public Transform camTilt_None;

    bool isShooting = false;
    public Transform shootAnim;
    public Transform idlePos;
    public Transform shootPos;
    public Transform reloadPos;
    bool needsToReleaseShoot = false;

    bool hasShot = false;
    float shootTime = 0;
    public Animator viewAnim;

    public Renderer laserSight;
    public Light aimLight;

    public Color normalTint;
    public Color shootingTint;
    Color currentTint;

    Vector3 hookAim_A;
    Vector3 hookAim_B;
    Vector3 hookAim_C;

    bool reloading = false;
    float reload_t = 0;

    public Transform currentHitPoI;

    public Renderer shotgunRend;
    public Color shotgunColour_hook;
    public Color shotgunColour_Shooting;

    float Yt;
    float Xt;

    public Transform lookAtAim;
    public Transform hookStartPos;

    public VisualEffect explosionVFX;

    void Awake()
    {
        GameObject canvas = GameObject.Find("Canvas");
        crosshairRI = canvas.transform.Find("CrosshairRI").GetComponent<RawImage>();
        crosshairGrabRI = canvas.transform.Find("CrosshairGrabRI").GetComponent<RawImage>();
        topLeftRI = canvas.transform.Find("TopLeft").GetComponent<RawImage>();
        bottomRightRI = canvas.transform.Find("BottomRight").GetComponent<RawImage>();
        testRI = canvas.transform.Find("TestRI").GetComponent<RawImage>();
        speedRI = canvas.transform.Find("SpeedRI").GetComponent<RawImage>();
        vignette = canvas.transform.Find("Vignette").GetComponent<RawImage>();
        postFX = GameObject.Find("PostFX").GetComponent<Volume>();

        currentTint = normalTint;
        GenerateGrabsList();
        hookLine.positionCount = arcResolution;
        hookLine_Aim.positionCount = arcResolution;
    }

    public void GenerateGrabsList() {
        grabPoints = new List<Transform>();
        /*
        foreach (SphereCollider SC in allGrabs.GetComponentsInChildren<SphereCollider>()) {
            grabPoints.Add(SC.transform);
        }
        */
        foreach (ClimbingPoI PoI in GameObject.FindObjectsOfTypeAll(typeof(ClimbingPoI))) {
            if (PoI.isActiveAndEnabled) {
                grabPoints.Add(PoI.transform);
                //Debug.Log(LC.gameObject.GetInstanceID());
            }
        }
    }

    void Update()
    {
        mousePos = Mouse.current.position.ReadValue();
        crosshairRI.transform.position = Vector3.Lerp(crosshairRI.transform.position, mousePos, Time.deltaTime * 100);

        Yt = Mathf.Lerp(Yt, isShooting ? 0.5f : (mousePos.y / topLeftRI.transform.position.y), Time.unscaledDeltaTime * 75f);
        Xt = Mathf.Lerp(Xt, isShooting ? 0.5f : (mousePos.x / bottomRightRI.transform.position.x), Time.unscaledDeltaTime * 75f);

        Quaternion UD = Quaternion.Lerp(Rot_U.localRotation, Rot_D.localRotation, 1f-Yt);
        Quaternion LR = Quaternion.Lerp(Rot_L.localRotation, Rot_R.localRotation, Xt);

        if (isShooting)
            Rot_LR.rotation = Quaternion.Lerp(Rot_LR.rotation, lookAtAim.rotation, Time.deltaTime * 8f);
        else
            Rot_LR.localRotation = Quaternion.Lerp(Rot_LR.localRotation, LR, Time.deltaTime * 8f);
        //Rot_LR.localRotation = LR;
        Rot_UD.localRotation = UD;

        tViz.x = Xt;
        tViz.y = Yt;


        if(swingDelay > 0) {
            swingDelay -= Time.unscaledDeltaTime;
        }

        //float hookWidth = isSwinging ? 0.345f : 0.1f;
        float hookWidth = isSwinging ? 0.45f : 0.31f;
        hookLine.widthMultiplier = currentGrab == null ? 0f : hookWidth;
        hookLine.material.SetFloat("_Intensity", isSwinging ? 5f : 1f);

        //hookLine.enabled = !isShooting;
        hookLine_Aim.enabled = !isShooting;
        laserSight.enabled = isShooting;
        aimLight.enabled = isShooting;
        shotgunRend.material.SetColor("_FresnelColour", isShooting ? shotgunColour_Shooting : shotgunColour_hook);


        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        bool hitPoI = false;
        if (Physics.Raycast(ray, out hit, 100)) {
            aimLight.transform.position = hit.point + (player.position - hit.point).normalized;
            if (Mouse.current.leftButton.ReadValue() == 0) {
                if (hit.transform.GetComponentsInChildren<ClimbingPoI>() != null) {
                    foreach (ClimbingPoI CPOI in hit.transform.GetComponentsInChildren<ClimbingPoI>()) {
                        if (Vector3.Distance(player.position, CPOI.transform.position) < playerRange) {
                            currentHitPoI = CPOI.transform;
                            hit.collider.SendMessage("Highlighted", SendMessageOptions.DontRequireReceiver);
                            hitPoI = true;
                        }
                    }
                } else {
                    currentHitPoI = null;
                }
            }
        }
        lookAtAim.LookAt(aimLight.transform.position);

        if (currentGrab != null) {
            currentGrab.SendMessage("HoldingPlayer", SendMessageOptions.DontRequireReceiver);
            hook.position = currentGrab.position + new Vector3(0, 0, -1.1f);

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

            rb.velocity = Vector3.SmoothDamp(rb.velocity, vel * pow * mod * 0.175f, ref refVel, step);//0.25f

            if (dist > 3) {
                //rb.velocity = vel * pow * mod;
            } else {
                //player.position = currentGrab.position;
                //rb.velocity = Vector3.zero;
            }

            crosshairGrabRI.enabled = true;
            crosshairGrabRI.transform.position = Camera.main.WorldToScreenPoint(currentGrab.transform.position);

            player.rotation = Quaternion.Lerp(player.rotation, lookUp.rotation, Time.deltaTime * 5f);

            rb.drag = Mathf.Lerp(25,0,Mathf.Clamp01(dist * 0.5f));
            rb.useGravity = false;
        } else {
            crosshairGrabRI.enabled = false;
            rb.drag = 0;
            rb.useGravity = true;

            player.rotation = Quaternion.Lerp(player.rotation, lookFWD.rotation, Time.deltaTime * 5f);

            if(rb.velocity.magnitude > maxSpeed) {
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            }
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && !reloading && !isSwinging) {
            Reload();
        }

        bool hitSomething = false;

        if (grabPoints != null) {
            if(currentHitPoI != null) {
                Vector3 currentHookPos = currentHitPoI.position;
                Vector2 currentHookScreenPos = Camera.main.WorldToScreenPoint(currentHookPos);
                bool inPlayerRange = Vector3.Distance(player.position, currentHookPos) < playerRange;

                if (Vector2.Distance(mousePos, currentHookScreenPos) < hookRange && inPlayerRange && !isSwinging) {
                    hitSomething = true;
                    targetHookPos = currentHookPos;
                    if (Mouse.current.leftButton.wasPressedThisFrame && !isShooting) {
                        if (currentGrab != null) {
                            if (currentHitPoI != currentGrab) {
                                currentGrab = currentHitPoI;
                                swingDelay = 0.1f;
                                grabStep_t = 0;
                                SetupHook();
                            } else {
                                if (swingDelay <= 0) {
                                    swingStartPos = mousePos;
                                    isSwinging = true;
                                }
                            }
                        } else {
                            currentGrab = currentHitPoI;
                            swingDelay = 0.1f;
                            grabStep_t = 0;
                            SetupHook();
                        }
                    }
                } else {

                }

                hookLine_Aim.widthMultiplier = hitSomething ? 0.1f : 0f;


                testRI.transform.position = currentHookScreenPos;
                hookAim_C = Vector3.Lerp(hookAim_C, currentHitPoI != null ? currentHitPoI.position : targetHookPos, Time.deltaTime * 6f);

                hook_A.position = player.position + transform.TransformDirection(Vector3.back * 2f);
                hook_B.position = player.position + transform.TransformDirection(Vector3.forward * 10f);//7

                Vector3 offsetC = new Vector3(0, 0.8f, 2);
                if (currentGrab == null)
                    hook_C.position = Vector3.Lerp(hook_C.position, targetHookPos + offsetC, Time.deltaTime * 6f);
                else
                    hook_C.position = Vector3.Lerp(hook_C.position, currentGrab.position + offsetC, Time.deltaTime * 6f);

                hookAim_A = hook_A.position;
                hookAim_B = hook_B.position;

            } else {
                for (int i = 0; i < grabPoints.Count; i++) {
                    Vector3 currentHookPos = grabPoints[i].position;
                    bool lookingAtGrab = false;
                    if (currentGrab != null && currentHitPoI != null) {
                        if (currentHitPoI == currentGrab)
                            lookingAtGrab = true;
                    }
                    //if (currentHitPoI != null && !lookingAtGrab)
                    //    currentHookPos = currentHitPoI.position;
                    Vector2 currentHookScreenPos = Camera.main.WorldToScreenPoint(currentHookPos);

                    bool inPlayerRange = Vector3.Distance(player.position, currentHookPos) < playerRange;

                    if (Vector2.Distance(mousePos, currentHookScreenPos) < hookRange && inPlayerRange && !isSwinging) {
                        hitSomething = true;
                        targetHookPos = currentHookPos;
                        if (Mouse.current.leftButton.wasPressedThisFrame && !isShooting) {
                            if (currentGrab != null) {
                                if (grabPoints[i] != currentGrab) {
                                    currentGrab = grabPoints[i];
                                    swingDelay = 0.1f;
                                    grabStep_t = 0;
                                    SetupHook();
                                } else {
                                    if (swingDelay <= 0) {
                                        swingStartPos = mousePos;
                                        isSwinging = true;
                                    }
                                }
                            } else {
                                currentGrab = grabPoints[i];
                                swingDelay = 0.1f;
                                grabStep_t = 0;
                                SetupHook();
                            }
                        }
                    } else {

                    }

                    hookLine_Aim.widthMultiplier = hitSomething ? 0.1f : 0f;


                    testRI.transform.position = currentHookScreenPos;
                    hookAim_C = Vector3.Lerp(hookAim_C, currentHitPoI != null ? currentHitPoI.position : targetHookPos, Time.deltaTime * 6f);

                    hook_A.position = player.position + transform.TransformDirection(Vector3.back * 2f);
                    hook_B.position = player.position + transform.TransformDirection(Vector3.forward * 10f);//7

                    Vector3 offsetC = new Vector3(0, 0.8f, 2);
                    if (currentGrab == null)
                        hook_C.position = Vector3.Lerp(hook_C.position, targetHookPos + offsetC, Time.deltaTime * 6f);
                    else
                        hook_C.position = Vector3.Lerp(hook_C.position, currentGrab.position + offsetC, Time.deltaTime * 6f);

                    hookAim_A = hook_A.position;
                    hookAim_B = hook_B.position;
                }
            }
            
        }

        if (!hitSomething && currentGrab != null && !needsToReleaseShoot && Mouse.current.leftButton.ReadValue() == 1) {
            if (!isSwinging && !isShooting) {
                if (swingDelay <= 0) {
                    swingStartPos = mousePos;
                    isSwinging = true;
                }
            }
            //Debug.Log("SWING");
        }


        for (int i = 0; i < arcResolution; i++) {
            float at = 0;
            if (i == 0)
                at = 0;
            else
                at = (float)i / (float)(arcResolution);
            hookLine.SetPosition(i, QuadCurve(hook_A.position, hook_B.position, hook_C.position, at));
            hookLine_Aim.SetPosition(i, QuadCurve(hookAim_A, hookAim_B, hookAim_C, at));
        }



        float swayRange = rb.velocity.magnitude < 10 ? 0f : 0.5f;//.85f
        swayX = Mathf.Lerp(swayX, (rb.velocity.x > 4.5f || rb.velocity.x < -4.5f) ? (swayRange * (rb.velocity.x > 4.5f ? 1 : -1)) : 0, Time.deltaTime * 4.5f);
        swayY = Mathf.Lerp(swayY, (rb.velocity.y > 4.5f || rb.velocity.y < -4.5f) ? (swayRange * (rb.velocity.y > 4.5f ? 1 : -1)) : 0, Time.deltaTime * 4.5f);

        gunFWD.localPosition = Vector3.Lerp(gunFWD.localPosition, isSwinging ? new Vector3(swayX, swayY, -3.54f) : new Vector3(swayX, swayY, -2.06f), Time.deltaTime * 4f);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, isSwinging ? 70 : 80, Time.deltaTime * 5f);
        //camLook.rotation = Quaternion.Lerp(camLook.rotation, isSwinging ? camLookAt.rotation : camLookFWD.rotation, Time.deltaTime * 3.5f);
        vignette.color = Color.Lerp(vignette.color, isSwinging ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0), Time.deltaTime * 3.4f);

        if(rb.velocity.magnitude < 23) {
            camTilt.localRotation = Quaternion.Lerp(camTilt.localRotation, camTilt_None.localRotation, Time.deltaTime * 2f);
        } else {
            if(rb.velocity.x < -4.5f)
                camTilt.localRotation = Quaternion.Lerp(camTilt.localRotation, camTilt_L.localRotation, Time.deltaTime * 2f);
            else
                camTilt.localRotation = Quaternion.Lerp(camTilt.localRotation, camTilt_R.localRotation, Time.deltaTime * 2f);

        }

        if (isSwinging) {
            //Debug.Log("SWINGING");
            Vector3 tmpSwingOffset = (mousePos - swingStartPos) * 0.05f;//0.05f
            //springRange = 4.5f;//21
            float xClamp = Mathf.Clamp(tmpSwingOffset.x,-springRange, springRange);
            float yClamp = Mathf.Clamp(tmpSwingOffset.y, -springRange, springRange);
            swingOffset = new Vector3(xClamp, yClamp,0);
            camLookAt.transform.LookAt(currentGrab.position);
            needsToReleaseShoot = true;

            if (Mouse.current.leftButton.ReadValue() == 0) {
                isSwinging = false;
                swingOffset = Vector3.zero;
                camLookAt.rotation = camLookFWD.rotation;
                if(speedDist > 25) {
                    hook.position = new Vector3(0, -100, 0);
                    currentGrab = null;
                }
            }

        } else {

        }

        if (slowMotion) {

        }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) {
            //slowMotion = !slowMotion;
        }
        slowMotion = isShooting;
        if (Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.digit1Key.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame) {
            if (isSwinging) {
                isSwinging = false;
                swingOffset = Vector3.zero;
                camLookAt.rotation = camLookFWD.rotation;
            }
            isShooting = !isShooting;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            needsToReleaseShoot = false;

        currentChromAb = Mathf.Lerp(currentChromAb, slowMotion ? 1f : 0, Time.deltaTime * 5f);
        currentContrast = Mathf.Lerp(currentChromAb, slowMotion ? 22f : 0, Time.deltaTime * 5f);
        currentSat = Mathf.Lerp(currentChromAb, slowMotion ? 51f : 0, Time.deltaTime * 5f);

        float slowMoTimescale = 0.5f;
        Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotion ? slowMoTimescale : 1f, Time.deltaTime * 4);
        //Time.fixedTime = Mathf.Lerp(Time.timeScale, slowMotion ? 1f : 0.1f, Time.deltaTime * 4);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        postFX.profile.TryGet(out ChromaticAberration chromAb);
        chromAb.intensity.value = currentChromAb;
        postFX.profile.TryGet(out ColorAdjustments colAdj);
        colAdj.contrast.value = currentContrast;
        colAdj.saturation.value = currentSat;
        currentTint = Color.Lerp(currentTint, isShooting ? shootingTint : normalTint, Time.deltaTime);
        colAdj.colorFilter.value = currentTint;

        if (Mouse.current.leftButton.wasPressedThisFrame && !hasShot && !needsToReleaseShoot && !reloading) {
            hasShot = true;
            shootTime = 0.5f;
            viewAnim.Play("ViewShot");
            
            if (isShooting) {
                Vector3 dir = (aimLight.transform.position - player.position).normalized;
                //shootAnim.TransformDirection(Vector3.back * 123);
                rb.velocity += dir * -123;
                explosionVFX.transform.position = aimLight.transform.position;
                explosionVFX.Play();
            }
        }

        if (hasShot) {
            shootTime -= Time.deltaTime;
            if(shootTime <= 0) {
                shootTime = 0;
                viewAnim.Play("ViewIdle");
                hasShot = false;
            }
            if (shootTime > 0.45f) {
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, shootPos.localPosition, Time.deltaTime * 50f);
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, shootPos.localRotation, Time.deltaTime * 50f);
            } else {
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, idlePos.localPosition, Time.deltaTime * 10f);
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, idlePos.localRotation, Time.deltaTime * 10f);
            }
        } else if(!reloading) {
            shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, idlePos.localPosition, Time.deltaTime * 10f);
            shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, idlePos.localRotation, Time.deltaTime * 10f);
        }

        if (reloading) {
            reload_t += Time.deltaTime;
            
            if (reload_t < 0.35f)
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, reloadPos.localPosition, Time.deltaTime * 7.55f);
            else
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, idlePos.localPosition, Time.deltaTime * 10f);
            

            if(reload_t > 0.13f && reload_t < 0.6f)
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, reloadPos.localRotation, Time.deltaTime * 15f);
            else
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, idlePos.localRotation, Time.deltaTime * 10f);


            if (reload_t > 1f) {
                reload_t = 0f;
                reloading = false;
                shootTime = 0;
                hasShot = false;
                viewAnim.Play("ViewIdle");
            }
        } else {

        }
    }

    private void LateUpdate() {
        speedRI.transform.position = Vector3.MoveTowards(speedRI.transform.position, mousePos, Time.deltaTime * 1730f);
        speedDist = Vector2.Distance(mousePos, speedRI.transform.position);
    }

    private void FixedUpdate() {
        if (currentGrab != null) {

        } else {
            rb.AddForce(Vector3.down * gravity * Time.fixedDeltaTime);
        }
    }

    public void Reload() {
        viewAnim.Play("ViewReload");
        reload_t = 0f;
        reloading = true;
        hasShot = true;
    }

    public void SetupHook() {
        hook.rotation = player.rotation;
        hook.localScale = Vector3.one * 1f;
    }

    public void KickPlayer(Vector3 velocity) {
        isSwinging = false;
        swingOffset = Vector3.zero;
        camLookAt.rotation = camLookFWD.rotation;
        currentGrab = null;
        if(Mouse.current.leftButton.ReadValue() == 1)
            needsToReleaseShoot = true;
        rb.velocity = velocity;
        hook.position = new Vector3(0, -100, 0);
        //Debug.Log("KickPlayer");

    }

    Vector3 QuadCurve(Vector3 start, Vector3 mid, Vector3 end, float t) {
        Vector3 p0 = Vector3.Lerp(start, mid, t);
        Vector3 p1 = Vector3.Lerp(mid, end, t);
        return Vector3.Lerp(p0, p1, t);
    }
}

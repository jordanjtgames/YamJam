using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Vector2 mousePos;

    public Transform allGrabs;
    public List<Transform> grabPoints;
    public List<Transform> allArt;
    public List<Spider> allSpiders;
    float spiderSplash = 12f;

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
    public RawImage crosshairSwingRI;
    public RawImage topLeftRI;
    public RawImage bottomRightRI;

    public int currentLevel = 1;
    public float springRange = 4.5f;
    public float maxSpeed = 70;//50
    public float playerRange = 90;
    public float hookRange = 45;
    public float recoil = 123;
    public float singleShotTime = 1f;
    public float springJumpHeight = 45f;
    public float musicVol = 0.15f;

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

    public Transform barrel;
    public Transform barrel_Shoot;
    public Transform barrel_Hook;
    public Renderer handel;

    float rotate_t = 0;

    int ammo = 3;
    float barrelDarken = 0f;
    float ammoRegen = 1f;

    public VisualEffect hookVFX;
    public LayerMask PoIPairingMask;

    float vignetteIntensity = 0;

    public LayerMask shootMask;
    public LayerMask worldMask;

    public RawImage fadeToWhite;
    public bool goalReached = false;

    public AudioSource music;
    public AudioSource sfx;

    public AudioClip music_A;
    public AudioClip music_B;
    public AudioMixer mixer;

    public List<AudioClip> allSFX;
    float playerHealth = 100f;
    bool dead = false;
    float damageCooldown = 0;
    float death_t = 0f;

    float listsTimer = 0.4f;
    bool setupLists = false;

    public Transform crosshairHookRot;
    public Transform crosshairShootRot;
    public RawImage shotgunCrosshairRI;

    public RawImage blackFade;
    public GameObject loadingUI;

    bool paused = false;
    public Transform pausedCamPos;
    public Transform unpausedCamPos;
    public GameObject pauseMenuGO;

    float levelTime = 0f;
    float goalTime = 0f;
    bool needsToLoadMenu = true;
    public Transform skybox;

    public bool envReady = false;
    public Transform loadingIcon;
    public Transform groundLight;

    public GameObject deathUI;
    float startZ = 0;

    void Awake()
    {
        startZ = player.position.z;
        Debug.Log("Awake");
        music.volume = 0;
        barrel.rotation = isShooting ? barrel_Shoot.rotation : barrel_Hook.rotation;

        GameObject canvas = GameObject.Find("Canvas");
        crosshairRI = canvas.transform.Find("CrosshairRI").GetComponent<RawImage>();
        crosshairGrabRI = canvas.transform.Find("CrosshairGrabRI").GetComponent<RawImage>();
        crosshairHookRot = canvas.transform.Find("HookRot");
        crosshairShootRot = canvas.transform.Find("ShootRot");
        crosshairSwingRI = crosshairGrabRI.transform.GetChild(0).GetComponent<RawImage>();
        shotgunCrosshairRI = canvas.transform.Find("CrosshairShotgunRI").GetComponent<RawImage>();
        topLeftRI = canvas.transform.Find("TopLeft").GetComponent<RawImage>();
        bottomRightRI = canvas.transform.Find("BottomRight").GetComponent<RawImage>();
        testRI = canvas.transform.Find("TestRI").GetComponent<RawImage>();
        speedRI = canvas.transform.Find("SpeedRI").GetComponent<RawImage>();
        vignette = canvas.transform.Find("Vignette").GetComponent<RawImage>();
        postFX = GameObject.Find("PostFX").GetComponent<Volume>();
        fadeToWhite = canvas.transform.Find("FadeToWhite").GetComponent<RawImage>();
        blackFade = canvas.transform.Find("BlackFade").GetComponent<RawImage>();
        loadingUI = canvas.transform.Find("LoadingUI").gameObject;
        loadingIcon = loadingUI.transform.Find("LoadingIcon");
        skybox = GameObject.Find("_Skybox").transform;
        groundLight = GameObject.Find("GroundLight").transform;
        deathUI = canvas.transform.Find("DeathUI").gameObject;

        currentTint = normalTint;
        hookLine.positionCount = arcResolution;
        hookLine_Aim.positionCount = arcResolution;
    }

    private void Start() {
        //GenerateGrabsList();
        //StartMusic();
    }

    void StartMusic() {
        music.clip = currentLevel > 1 ? music_B : music_A;
        music.Play();
        music.mute = StaticGameParams.muted;
    }

    public void PlayOneShot(int id, float vol) {
        sfx.PlayOneShot(allSFX[id], vol);
    }

    public void GenerateGrabsList() {
        grabPoints = new List<Transform>();
        allSpiders = new List<Spider>();
        /*
        foreach (SphereCollider SC in allGrabs.GetComponentsInChildren<SphereCollider>()) {
            grabPoints.Add(SC.transform);
        }
        */

        //SPIDER
        foreach (Spider Spd in GameObject.FindObjectsOfTypeAll(typeof(Spider))) {
            if (Spd.isActiveAndEnabled) {
                allSpiders.Add(Spd);

                RaycastHit spiderHit;
                if (Physics.Raycast(Spd.transform.position + new Vector3(0, 0, -10), Vector3.forward, out spiderHit, 90, PoIPairingMask)) {
                    Spd.transform.root.parent = spiderHit.collider.transform;
                    //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = PoI.transform.position;
                    //Debug.LogError("PAIRED");
                }
            }
        }
        //DRONE
        foreach (Drone Drn in GameObject.FindObjectsOfTypeAll(typeof(Drone))) {
            if (Drn.isActiveAndEnabled) {
                RaycastHit droneHit;
                if (Physics.Raycast(Drn.transform.Find("A").position + new Vector3(0, 0, -10), Vector3.forward, out droneHit, 90, PoIPairingMask)) {
                    Drn.transform.root.parent = droneHit.collider.transform;
                    //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = PoI.transform.position;
                    //Debug.LogError("PAIRED");
                }
            }
        }
        //Turret
        foreach (Turret Trt in GameObject.FindObjectsOfTypeAll(typeof(Turret))) {
            if (Trt.isActiveAndEnabled) {
                RaycastHit turretHit;
                if (Physics.Raycast(Trt.transform.position + new Vector3(0, 0, -10), Vector3.forward, out turretHit, 90, PoIPairingMask)) {
                    Trt.transform.root.parent = turretHit.collider.transform;
                    //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = PoI.transform.position;
                    //Debug.LogError("PAIRED");
                }
            }
        }


        foreach (ClimbingPoI PoI in GameObject.FindObjectsOfTypeAll(typeof(ClimbingPoI))) {
            if (PoI.isActiveAndEnabled) {
                grabPoints.Add(PoI.transform);

                RaycastHit PoIPairHit;
                if(Physics.Raycast(PoI.transform.position + new Vector3(0,0,-10), Vector3.forward, out PoIPairHit, 90, PoIPairingMask)) {
                    PoI.transform.root.parent = PoIPairHit.collider.transform;
                    Debug.Log(PoIPairHit.collider.transform);
                    //GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = PoI.transform.position;
                    //Debug.LogError("PAIRED");
                }
                //Debug.Log(LC.gameObject.GetInstanceID());
            }
        }

        foreach (Art artAsset in GameObject.FindObjectsOfTypeAll(typeof(Art))) {
            if (artAsset.isActiveAndEnabled) {
                allArt.Add(artAsset.transform);

                RaycastHit artHit;
                if (Physics.Raycast(artAsset.transform.position + new Vector3(0, 0, -10), Vector3.forward, out artHit, 90, PoIPairingMask)) {
                    artAsset.transform.root.parent = artHit.collider.transform;
                    Debug.Log(artHit.collider.transform);
                }
            }
        }

        foreach (EnvironmentScript EvS in GameObject.FindObjectsOfTypeAll(typeof(EnvironmentScript))) {
            if (EvS.isActiveAndEnabled) {
                EvS.SetupCells();
            }
        }

    }

    void Update() {
        if (currentLevel == 1 && blackFade.color.a <= 0.01f && levelTime > 7f)
            GameObject.Find("Tut_1").GetComponent<Tutorials>().canShow = true;

        if (!setupLists) {
            listsTimer -= Time.unscaledDeltaTime;
            loadingIcon.Rotate(Vector3.forward * Time.unscaledDeltaTime * -670f);

            if (listsTimer <= 0) {
                setupLists = true;
                GenerateGrabsList();
                StartMusic();
                /*
                foreach (EnvironmentScript EvS in GameObject.FindObjectsOfTypeAll(typeof(EnvironmentScript))) {
                    if (EvS.isActiveAndEnabled) {
                        EvS.MoveToFarPos();
                    }
                }
                */
                //envReady = true;
                foreach (EnvironmentScript EvS in GameObject.FindObjectsOfTypeAll(typeof(EnvironmentScript))) {
                    if (EvS.isActiveAndEnabled) {
                        EvS.MoveToFarPos();
                    }
                }

                envReady = true;
            }
        } else {
            if(blackFade.color.a > 0.01f) {
                loadingUI.SetActive(false);
                blackFade.color = Color.Lerp(blackFade.color, Color.clear, Time.unscaledDeltaTime * 3f);
            } else {
                blackFade.color = Color.clear;
            }
            HandleInput();
        }

        if (!goalReached) {
            levelTime += Time.unscaledDeltaTime;
        } else {
            goalTime += Time.unscaledDeltaTime;
            if(goalTime > 1f) {
                GameObject canvas = GameObject.Find("Canvas");
                canvas.transform.Find("LevelCompleteUI").gameObject.SetActive(true);
                int minutes = Mathf.FloorToInt(levelTime / 60F);
                int seconds = Mathf.FloorToInt(levelTime - minutes * 60);
                string timeText = string.Format("{0:0}:{1:00}", minutes, seconds);
                canvas.transform.Find("LevelCompleteUI").GetChild(2).GetComponent<TextMeshProUGUI>().text = timeText;

            }
            if(goalTime > 3f && needsToLoadMenu) {
                SceneManager.LoadScene(0);
                needsToLoadMenu = false;
            }
        }
    }

    void HandleInput()
    {
        if(Keyboard.current.jKey.wasPressedThisFrame && !envReady) {
            
            
        }

        Cursor.visible = paused;
        mousePos = Mouse.current.position.ReadValue();
        crosshairRI.enabled = !paused;
        crosshairRI.transform.position = Vector3.Lerp(crosshairRI.transform.position, mousePos, Time.deltaTime * 100);
        crosshairSwingRI.enabled = isSwinging && !paused;
        crosshairRI.transform.rotation = Quaternion.RotateTowards(crosshairRI.transform.rotation, isShooting ? crosshairShootRot.rotation : crosshairHookRot.rotation, Time.unscaledDeltaTime * 586f);
        shotgunCrosshairRI.enabled = isShooting && !paused;
        shotgunCrosshairRI.transform.position = crosshairRI.transform.position;

        bool centerGun = isShooting || paused;
        Yt = Mathf.Lerp(Yt, centerGun ? 0.5f : (mousePos.y / topLeftRI.transform.position.y), Time.unscaledDeltaTime * 75f);
        Xt = Mathf.Lerp(Xt, centerGun ? 0.5f : (mousePos.x / bottomRightRI.transform.position.x), Time.unscaledDeltaTime * 75f);

        Quaternion UD = Quaternion.Lerp(Rot_U.localRotation, Rot_D.localRotation, 1f-Yt);
        Quaternion LR = Quaternion.Lerp(Rot_L.localRotation, Rot_R.localRotation, Xt);

        if (isShooting)
            Rot_LR.rotation = Quaternion.Lerp(Rot_LR.rotation, lookAtAim.rotation, Time.unscaledDeltaTime * 8f);
        else
            Rot_LR.localRotation = Quaternion.Lerp(Rot_LR.localRotation, LR, Time.unscaledDeltaTime * 8f);
        //Rot_LR.localRotation = LR;
        Rot_UD.localRotation = UD;

        tViz.x = Xt;
        tViz.y = Yt;


        if(swingDelay > 0) {
            swingDelay -= Time.unscaledDeltaTime;
        }

        if (music.isPlaying)
            music.volume = Mathf.Lerp(music.volume, goalReached ? 0f : musicVol, Time.deltaTime);

        if (playerHealth < 100 && !dead)
            playerHealth += Time.deltaTime * 10;
        else
            playerHealth = 100;
        if (playerHealth <= 0 && !dead) {
            PlayOneShot(4, 0.7f);
            Debug.LogError("DEAD");
            KickPlayer(Vector3.back * 5f);
            dead = true;
        }
        if (damageCooldown > 0)
            damageCooldown -= Time.deltaTime;
        else
            damageCooldown = 0;
        if (dead) {
            ammo = 0;
            death_t += Time.unscaledDeltaTime;
            if(death_t > 2.5f && needsToLoadMenu) {
                SceneManager.LoadScene(currentLevel);
                needsToLoadMenu = false;
            }
            deathUI.SetActive(true);
        }

        float currentShift = 1f;
        mixer.GetFloat("PitchShift", out currentShift);
        mixer.SetFloat("PitchShift", Mathf.Lerp(currentShift, slowMotion ? 0.25f : 1f, Time.unscaledDeltaTime * 1.5f));

        if (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.digit2Key.wasPressedThisFrame ||
            Keyboard.current.digit1Key.wasPressedThisFrame) {
            paused = !paused;
            pauseMenuGO.SetActive(paused);
            if (paused) {
                isShooting = false;
            } else {

            }
        }

        //float hookWidth = isSwinging ? 0.345f : 0.1f;
        float hookWidth = isSwinging ? 0.45f : 0.31f;
        hookLine.widthMultiplier = currentGrab == null ? 0f : hookWidth;
        hookLine.material.SetFloat("_Intensity", isSwinging ? 5f : 1f);

        //hookLine.enabled = !isShooting;
        hookLine_Aim.enabled = !isShooting;
        laserSight.enabled = isShooting;
        aimLight.enabled = isShooting;
        //shotgunRend.material.SetColor("_FresnelColour", isShooting ? shotgunColour_Shooting : shotgunColour_hook);
        barrel.GetComponent<Renderer>().material.SetColor("_Tint", Color.Lerp(barrel.GetComponent<Renderer>().material.GetColor("_Tint"), 
            isShooting ? shotgunColour_Shooting : shotgunColour_hook, Time.unscaledDeltaTime * 10f));
        switch (ammo) {
            case 3:
                barrelDarken = 0f;
                break;
            case 2:
                barrelDarken = 0.33f;
                break;
            case 1:
                barrelDarken = 0.66f;
                break;
            case 0:
                barrelDarken = 1f;
                break;
        }
        barrel.GetComponent<Renderer>().material.SetFloat("_Darken", barrelDarken);

        if (ammoRegen < 1f) {
            ammoRegen += Time.unscaledDeltaTime * 0.7f;
            if (ammoRegen >= 1f) {
                if(ammo != 3)
                    ammo++;
                ammoRegen = 0;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit worldHit;
        RaycastHit shootHit;
        bool hitPoI = false;
        if (Physics.Raycast(ray, out worldHit, 100, worldMask)) {
            aimLight.transform.position = worldHit.point + (player.position - worldHit.point).normalized;
            if (Mouse.current.leftButton.ReadValue() == 0) {
                if (worldHit.transform.GetComponentsInChildren<ClimbingPoI>() != null) {
                    foreach (ClimbingPoI CPOI in worldHit.transform.GetComponentsInChildren<ClimbingPoI>()) {
                        if (Vector3.Distance(player.position, CPOI.transform.position) < playerRange) {
                            currentHitPoI = CPOI.transform;
                            worldHit.collider.SendMessage("Highlighted", SendMessageOptions.DontRequireReceiver);
                            hitPoI = true;
                        }
                    }
                } else {
                    currentHitPoI = null;
                }   
            }
            if(worldHit.collider.gameObject.name == "ResumeWUI" && Mouse.current.leftButton.wasPressedThisFrame) {
                Debug.Log("Resume");
                paused = false;
                pauseMenuGO.SetActive(false);
            }
            if (worldHit.collider.gameObject.name == "RestartWUI" && Mouse.current.leftButton.wasPressedThisFrame) {
                Debug.Log("Restart");
                Time.timeScale = 1f;
                SceneManager.LoadScene(currentLevel);
            }
            if (worldHit.collider.gameObject.name == "QuitWUI" && Mouse.current.leftButton.wasPressedThisFrame) {
                Debug.Log("Quit");
                Time.timeScale = 1f;
                SceneManager.LoadScene(0);
            }

        }
        if (Physics.Raycast(ray, out shootHit, 100, shootMask)) {
            if (isShooting && ammo > 0 && shootHit.collider.tag == "Enemy" && Mouse.current.leftButton.wasPressedThisFrame && shootTime == 0) {
                if (shootHit.collider.gameObject.name == "DroneCol")
                    shootHit.collider.transform.parent.parent.SendMessage("Shot", SendMessageOptions.DontRequireReceiver);
                if (shootHit.collider.gameObject.name == "TurretCol")
                    shootHit.collider.transform.parent.SendMessage("Shot", SendMessageOptions.DontRequireReceiver);
                if (shootHit.collider.gameObject.name == "SpiderCol") {
                    shootHit.collider.transform.parent.SendMessage("Shot", true, SendMessageOptions.DontRequireReceiver);

                    for (int i = 0; i < allSpiders.Count; i++) {
                        if (Vector3.Distance(allSpiders[i].transform.position, shootHit.point) < spiderSplash)
                            allSpiders[i].Shot(false);
                    }
                }
            }
        }
        lookAtAim.LookAt(aimLight.transform.position);

        if (currentGrab != null) {
            currentGrab.SendMessage("HoldingPlayer", SendMessageOptions.DontRequireReceiver);

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

            if (paused) {
                player.rotation = Quaternion.Lerp(player.rotation, lookFWD.rotation, Time.unscaledDeltaTime * 5f);
            } else {
                player.rotation = Quaternion.Lerp(player.rotation, lookUp.rotation, Time.smoothDeltaTime * 5f);
            }

            rb.drag = Mathf.Lerp(25,0,Mathf.Clamp01(dist * 0.5f));
            rb.useGravity = false;
            hook.position = currentGrab.position + new Vector3(0, 0, -1.1f);

        } else {
            crosshairGrabRI.enabled = false;
            rb.drag = 0;
            rb.useGravity = true;


            if (paused) {
                player.rotation = Quaternion.Lerp(player.rotation, lookFWD.rotation, Time.unscaledDeltaTime * 5f);
            } else {
                player.rotation = Quaternion.Lerp(player.rotation, lookFWD.rotation, Time.smoothDeltaTime * 5f);
            }

            if (rb.velocity.magnitude > maxSpeed) {
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
            }

            hook.position = hookStartPos.position;
            hook.rotation = hookStartPos.rotation;
            hook.localScale = Vector3.one * 0.45199f;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame && !reloading && !isSwinging) {
            Reload();
        }

        bool hitSomething = false;

        if (grabPoints != null && !paused) {
            if(currentHitPoI != null) {
                Vector3 currentHookPos = currentHitPoI.position;
                Vector2 currentHookScreenPos = Camera.main.WorldToScreenPoint(currentHookPos);
                bool inPlayerRange = Vector3.Distance(player.position, currentHookPos) < playerRange && !dead;

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

                    bool inPlayerRange = Vector3.Distance(player.position, currentHookPos) < playerRange && !dead;

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

        if (!hitSomething && currentGrab != null && !needsToReleaseShoot && Mouse.current.leftButton.ReadValue() == 1 && !paused) {
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



        float swayRange = (rb.velocity.magnitude < 10 ? 0f : 0.5f) * (paused ? 0 : 1f);//.85f
        swayX = Mathf.Lerp(swayX, (rb.velocity.x > 4.5f || rb.velocity.x < -4.5f) ? (swayRange * (rb.velocity.x > 4.5f ? 1 : -1)) : 0, Time.unscaledDeltaTime * 4.5f);
        swayY = Mathf.Lerp(swayY, (rb.velocity.y > 4.5f || rb.velocity.y < -4.5f) ? (swayRange * (rb.velocity.y > 4.5f ? 1 : -1)) : 0, Time.unscaledDeltaTime * 4.5f);

        gunFWD.localPosition = Vector3.Lerp(gunFWD.localPosition, isSwinging ? new Vector3(swayX, swayY, -3.54f) : new Vector3(swayX, swayY, -2.06f), Time.unscaledDeltaTime * 4f);
        if(paused)
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 40, Time.unscaledDeltaTime* 5f);
        else
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, isSwinging ? 70 : 80, Time.deltaTime * 5f);
        //camLook.rotation = Quaternion.Lerp(camLook.rotation, isSwinging ? camLookAt.rotation : camLookFWD.rotation, Time.deltaTime * 3.5f);
        vignette.color = Color.Lerp(vignette.color, isSwinging ? new Color(0, 0, 0, 1) : new Color(0, 0, 0, 0), Time.deltaTime * 3.4f);

        if(rb.velocity.magnitude < 23 || paused) {
            camTilt.localRotation = Quaternion.Lerp(camTilt.localRotation, camTilt_None.localRotation, (paused ? Time.unscaledDeltaTime : Time.deltaTime) * 2f);
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
                camLookAt.rotation = camLookFWD.rotation;
                if(speedDist > 25 || swingOffset.y < -5) {
                    hook.position = new Vector3(0, -100, 0);
                    PlayOneShot(15, 0.6f);
                    currentGrab = null;
                    if (swingOffset.y < -5) {
                        rb.velocity = new Vector3(swingOffset.x * -2.5f, springJumpHeight, 0);
                        //Debug.Log("ThrowPlayer");
                    }
                }
                swingOffset = Vector3.zero;
            }

        } else {

        }

        if (slowMotion) {

        }

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame) {
            //slowMotion = !slowMotion;
        }
        slowMotion = isShooting;
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            if (isSwinging) {
                isSwinging = false;
                swingOffset = Vector3.zero;
                camLookAt.rotation = camLookFWD.rotation;
            }
            PlayOneShot(14, 0.139f);
            isShooting = !isShooting;
            rotate_t = 0.7f;
        }

        if(rotate_t > 0) {
            rotate_t -= Time.unscaledDeltaTime;
            if(rotate_t <= 0) {
                barrel.rotation = isShooting ? barrel_Shoot.rotation : barrel_Hook.rotation;
            } else {
                barrel.rotation = Quaternion.RotateTowards(barrel.rotation, isShooting ? barrel_Shoot.rotation : barrel_Hook.rotation, Time.unscaledDeltaTime * 1469f);
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            needsToReleaseShoot = false;

        currentChromAb = Mathf.Lerp(currentChromAb, slowMotion ? 1f : 0, Time.deltaTime * 5f);
        currentContrast = Mathf.Lerp(currentChromAb, slowMotion ? 22f : 0, Time.deltaTime * 5f);
        currentSat = Mathf.Lerp(currentChromAb, slowMotion ? 51f : 0, Time.deltaTime * 5f);

        float slowMoTimescale = 0.5f;
        if (paused) {
            Time.timeScale = 0;
            //viewAnim.enabled = false;
        } else {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotion ? slowMoTimescale : 1f, Time.unscaledDeltaTime * 4);
        }
        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, paused ? pausedCamPos.localPosition : unpausedCamPos.localPosition, Time.unscaledDeltaTime * 4);
        Camera.main.transform.localRotation = Quaternion.Lerp(Camera.main.transform.localRotation, paused ? pausedCamPos.localRotation : unpausedCamPos.localRotation, Time.unscaledDeltaTime * 4);
        //Time.fixedTime = Mathf.Lerp(Time.timeScale, slowMotion ? 1f : 0.1f, Time.deltaTime * 4);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        postFX.profile.TryGet(out ChromaticAberration chromAb);
        chromAb.intensity.value = currentChromAb;
        postFX.profile.TryGet(out ColorAdjustments colAdj);
        colAdj.contrast.value = currentContrast;
        colAdj.saturation.value = currentSat;
        currentTint = Color.Lerp(currentTint, isShooting ? shootingTint : normalTint, Time.deltaTime);
        colAdj.colorFilter.value = currentTint;
        postFX.profile.TryGet(out Vignette vig);
        vig.intensity.value = vignetteIntensity;
        vignetteIntensity = Mathf.Lerp(vignetteIntensity, dead ? 1f : 0f, Time.unscaledDeltaTime * 2.5f);


        if (Mouse.current.leftButton.wasPressedThisFrame && !hasShot && !needsToReleaseShoot && !reloading && ammo > 0 && !paused) {
            hasShot = true;
            shootTime = 0.5f;
            viewAnim.Play("ViewShot");
            

            if (isShooting) {
                Vector3 dir = (aimLight.transform.position - player.position).normalized;
                //shootAnim.TransformDirection(Vector3.back * 123);
                rb.velocity += dir * -recoil;
                explosionVFX.transform.position = aimLight.transform.position;
                explosionVFX.Play();
                if (ammo == 0) {
                    //Reload();
                } else {

                }
                PlayOneShot(0, 1f);
                PlayOneShot(8, 0.3f);
                if (ammo > 0)
                    ammo--;
                
                ammoRegen = 0f;
            }
        }

        if (hasShot) {
            shootTime -= Time.deltaTime * singleShotTime;

            if (shootTime <= 0) {
                shootTime = 0;
                viewAnim.Play("ViewIdle");
                hasShot = false;
            }
            float deltaT = paused ? Time.unscaledDeltaTime : Time.deltaTime;
            if (shootTime > 0.45f) {
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, shootPos.localPosition, deltaT * 50f);
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, shootPos.localRotation, deltaT * 50f);
            } else {
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, idlePos.localPosition, deltaT * 10f);
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, idlePos.localRotation, deltaT * 10f);
            }
        } else if(!reloading) {
            shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, idlePos.localPosition, Time.deltaTime * 10f);
            shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, idlePos.localRotation, Time.deltaTime * 10f);
        }

        if (reloading) {
            reload_t += Time.unscaledDeltaTime;

            if (reload_t < 0.35f)
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, reloadPos.localPosition, Time.unscaledDeltaTime * 7.55f);
            else
                shootAnim.localPosition = Vector3.Lerp(shootAnim.localPosition, idlePos.localPosition, Time.unscaledDeltaTime * 10f);
            

            if(reload_t > 0.13f && reload_t < 0.6f)
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, reloadPos.localRotation, Time.unscaledDeltaTime * 15f);
            else
                shootAnim.localRotation = Quaternion.Lerp(shootAnim.localRotation, idlePos.localRotation, Time.unscaledDeltaTime * 10f);


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

        if (goalReached) {
            fadeToWhite.color = Color.Lerp(fadeToWhite.color, Color.white, Time.deltaTime * 5f);
            if(fadeToWhite.color.a > 0.99f) {
                //Finished Level
            }
        }

        skybox.position = player.position;
        groundLight.position = new Vector3(player.position.x, player.position.y - 25f, groundLight.position.z);
    }

    private void FixedUpdate() {
        if (currentGrab != null) {

        } else {
            rb.AddForce(Vector3.down * gravity * Time.fixedDeltaTime);
            if(player.position.z > startZ) {
                //Debug.LogError("TOO CLOSE");
                rb.AddForce(Vector3.back * 1120 * Time.fixedDeltaTime);
            }
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
        if (currentGrab != null) {
            hookVFX.transform.position = currentGrab.transform.position;
            hookVFX.Play();
            PlayOneShot(9, 0.24f);//0.33f
        }
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

    public void PlayerHit() {
        PlayOneShot(12, 0.49f);
        vignetteIntensity = 0.75f;
        if(damageCooldown <= 0)
            playerHealth -= 50f;
        damageCooldown = 0.61f;
    }

    Vector3 QuadCurve(Vector3 start, Vector3 mid, Vector3 end, float t) {
        Vector3 p0 = Vector3.Lerp(start, mid, t);
        Vector3 p1 = Vector3.Lerp(mid, end, t);
        return Vector3.Lerp(p0, p1, t);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScript : MonoBehaviour
{
    public List<Transform> allCells;
    public List<Vector3> targetPositions;
    public List<Vector3> farPositions;
    bool setup = false;
    bool check = false;
    public List<bool> playedBuildSound;

    PlayerMovement PM;

    void Awake()
    {
        PM = GameObject.Find("_Player").GetComponent<PlayerMovement>();
    }

    public void SetupCells() {
        allCells = new List<Transform>();
        targetPositions = new List<Vector3>();
        farPositions = new List<Vector3>();
        playedBuildSound = new List<bool>();

        foreach (CapsuleCollider MC in GetComponentsInChildren<CapsuleCollider>()) {
            allCells.Add(MC.transform);
            targetPositions.Add(MC.transform.position);
            float randX = Random.Range(90, 390);
            float LorR = Random.Range(0, 0.99f);
            float randZ = Random.Range(90, 390);
            Vector3 farPos = MC.transform.position + new Vector3(randX * (LorR > 0.5f ? 1 : -1), Random.Range(-20, 120), randZ);
            farPositions.Add(farPos);
            playedBuildSound.Add(false);

            //MC.transform.position = farPos;
        }

        setup = true;
    }

    public void MoveToFarPos() {
        for (int i = 0; i < allCells.Count; i++) {
            allCells[i].position = farPositions[i];
        }
    }

    void Update()
    {
        check = Vector3.Distance(transform.position, Camera.main.transform.position) < 150;
    }

    private void LateUpdate() {
        if (setup && check && PM.envReady) {
            for (int i = 0; i < allCells.Count; i++) {
                bool inRange = Vector3.Distance(targetPositions[i], Camera.main.transform.position) < 108;//78
                float speed = 1500;
                if(playedBuildSound[i] == false)
                    allCells[i].position = Vector3.MoveTowards(allCells[i].position, inRange ? targetPositions[i] : farPositions[i], Time.deltaTime * speed);
                //allCells[i].position = 
                bool glow = !inRange && playedBuildSound[i] == false;
                allCells[i].GetComponent<Renderer>().material.SetFloat("_FresnelIntensity",
                    Mathf.Lerp(allCells[i].GetComponent<Renderer>().material.GetFloat("_FresnelIntensity"), glow ? 55f : 0f, Time.deltaTime * 9f));//6

                if(Vector3.Distance(targetPositions[i], allCells[i].position) < 1 && playedBuildSound[i] == false) {
                    GameObject.Find("_Player").GetComponent<PlayerMovement>().PlayOneShot(Random.Range(1,4), 0.1575f);
                    playedBuildSound[i] = true;
                }
                /*
                if (Vector3.Distance(farPositions[i], allCells[i].position) < 1 && playedBuildSound[i] == true) {
                    playedBuildSound[i] = false;
                }
                */
            }
        }
    }
}

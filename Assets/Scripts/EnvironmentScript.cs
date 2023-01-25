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

    void Awake()
    {
        
    }

    public void SetupCells() {
        allCells = new List<Transform>();
        targetPositions = new List<Vector3>();
        farPositions = new List<Vector3>();
        playedBuildSound = new List<bool>();

        foreach (SkinnedMeshRenderer MC in GetComponentsInChildren<SkinnedMeshRenderer>()) {
            allCells.Add(MC.transform);
            targetPositions.Add(MC.transform.position);
            float randX = Random.Range(90, 390);
            float LorR = Random.Range(0, 0.99f);
            float randZ = Random.Range(90, 390);
            Vector3 farPos = new Vector3(randX * (LorR > 0.5f ? 1 : -1), Random.Range(-20, 120), randZ);
            farPositions.Add(farPos);
            playedBuildSound.Add(false);

            //MC.transform.position = farPos;
        }

        setup = true;
    }

    void Update()
    {
        check = Vector3.Distance(transform.position, Camera.main.transform.position) < 150;
    }

    private void LateUpdate() {
        if (setup && check) {
            for (int i = 0; i < allCells.Count; i++) {
                bool inRange = Vector3.Distance(targetPositions[i], Camera.main.transform.position) < 78;
                float speed = 1500;
                allCells[i].position = Vector3.MoveTowards(allCells[i].position, inRange ? targetPositions[i] : farPositions[i], Time.deltaTime * speed);
                //allCells[i].position = 
                allCells[i].GetComponent<Renderer>().material.SetFloat("_FresnelIntensity",
                    Mathf.Lerp(allCells[i].GetComponent<Renderer>().material.GetFloat("_FresnelIntensity"), inRange ? 0f : 55f, Time.deltaTime * 6f));

                if(Vector3.Distance(targetPositions[i], allCells[i].position) < 1 && playedBuildSound[i] == false) {
                    GameObject.Find("_Player").GetComponent<PlayerMovement>().PlayOneShot(Random.Range(1,4), 0.1575f);
                    playedBuildSound[i] = true;
                }
                if (Vector3.Distance(farPositions[i], allCells[i].position) < 1 && playedBuildSound[i] == true) {
                    playedBuildSound[i] = false;
                }
            }
        }
    }
}

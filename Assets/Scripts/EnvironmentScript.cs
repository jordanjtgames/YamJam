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

    void Awake()
    {
        allCells = new List<Transform>();
        targetPositions = new List<Vector3>();
        farPositions = new List<Vector3>();

        foreach (MeshCollider MC in GetComponentsInChildren<MeshCollider>()) {
            allCells.Add(MC.transform);
            targetPositions.Add(MC.transform.position);
            float randX = Random.Range(90, 390);
            float LorR = Random.Range(0, 0.99f);
            float randZ = Random.Range(90, 390);
            Vector3 farPos = new Vector3(randX * (LorR > 0.5f ? 1 : -1), Random.Range(-20, 20), randZ);
            farPositions.Add(farPos);

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
            }
        }
    }
}

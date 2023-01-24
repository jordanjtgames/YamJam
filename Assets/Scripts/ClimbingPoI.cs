using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingPoI : MonoBehaviour
{
    public int id = 1; //1 = Tri

    float hightlight = 0;
    bool highlighted = false;
    float checkDelay = 0.05f;

    public Renderer myRend;

    void Awake()
    {
        if(myRend == null)
            myRend = GetComponent<Renderer>();
    }

    void Update()
    {
        if (highlighted) {
            checkDelay -= Time.deltaTime;

            if(checkDelay <= 0) {
                highlighted = false;
            }
        } else {
            hightlight = Mathf.Lerp(hightlight, 0f, Time.deltaTime * 8f);
        }

        if (myRend != null)
            myRend.material.SetFloat("_Select", hightlight);
    }

    public void Highlighted() {
        hightlight = Mathf.Lerp(hightlight, 1f, Time.deltaTime * 8f);
        highlighted = true;
        checkDelay = 0.15f;
        Debug.Log("Highlih");
    }
}

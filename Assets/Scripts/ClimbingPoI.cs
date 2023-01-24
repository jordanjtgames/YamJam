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

    Vector3 startLocalScale;
    bool holdingPlayer = false;
    float holdingCheck = 0.1f;

    //float shrink_t = 0;

    void Awake()
    {
        if (id == 1) {
            if (myRend == null)
                myRend = GetComponent<Renderer>();
        } else if (id == 2) {
            startLocalScale = transform.localScale;
            if (myRend == null)
                myRend = transform.parent.GetChild(1).GetComponent<Renderer>();
        }

        
    }

    void Update()
    {
        if(id == 1) {

        } else if(id == 2) {

        }

        if (highlighted) {
            checkDelay -= Time.deltaTime;

            if(checkDelay <= 0) {
                highlighted = false;
            }
            
        } else {
            hightlight = Mathf.Lerp(hightlight, 0f, Time.deltaTime * 8f);
        }

        if (holdingPlayer) {
            holdingCheck -= Time.deltaTime;
            if(holdingCheck <= 0) {
                holdingPlayer = false;
            }
        }

        if (id == 2) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, startLocalScale * (holdingPlayer ? 0.01f : 1f), Time.deltaTime * 6f);
            if(holdingPlayer && transform.localScale.x < 0.5f) {
                Debug.LogError("KickPlayer");
            }
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

    public void HoldingPlayer() {
        holdingCheck = 0.15f;
        holdingPlayer = true;
    }
}

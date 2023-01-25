using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public Transform spider;
    public ClimbingPoI myPoI;

    void Start()
    {
        RaycastHit PoIHit;
        if(Physics.Raycast(transform.position + new Vector3(0,0,-10), Vector3.forward, out PoIHit, 90)) {
            if(PoIHit.collider.GetComponent<ClimbingPoI>()) {
                PoIHit.collider.GetComponent<ClimbingPoI>().blocked = true;
                myPoI = PoIHit.collider.GetComponent<ClimbingPoI>();
            }
        }
    }

    void Update()
    {
        
    }

    public void Shot() {
        myPoI.blocked = false;
        spider.gameObject.SetActive(false);
    }
}

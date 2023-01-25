using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public Transform playerPos;
    public float speed = 15f;

    float lifeTime = 10;

    Vector3 moveDir = Vector3.zero;

    void Start()
    {
        
    }

    public void SetMoveDir() {
        moveDir = (playerPos.position - transform.position).normalized;
    }

    void Update()
    {
        if(playerPos != null) {
            if(Vector3.Distance(playerPos.position, transform.position) < 3f) {
                playerPos.SendMessage("PlayerHit", SendMessageOptions.DontRequireReceiver);
            }

            transform.Translate(moveDir * Time.deltaTime * speed);
        }

        lifeTime -= Time.deltaTime;

        if(lifeTime <= 0) {
            Destroy(gameObject);
        }
    }
}

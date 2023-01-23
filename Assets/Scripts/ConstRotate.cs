using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstRotate : MonoBehaviour
{
    public Vector3 rot;
    public bool local = false;

    void Update()
    {
        transform.Rotate(rot * Time.deltaTime, local ? Space.Self : Space.World);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorials : MonoBehaviour
{
    public Transform playerPos;
    public Transform tutStartPos;
    public Transform tutEndPos;

    public Transform holder;
    public bool canShow = true;

    void Update()
    {
        bool inRange = playerPos.position.y > tutStartPos.position.y && playerPos.position.y < tutEndPos.position.y && canShow;
        holder.localPosition = Vector3.MoveTowards(holder.localPosition, new Vector3(inRange ? 0 : 1500,0,0), Time.unscaledDeltaTime * 8700f);
    }
}

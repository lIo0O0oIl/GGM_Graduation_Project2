using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineController : MonoBehaviour
{
    private Transform myDotPos;
    private LineRenderer myLine;

    private void Awake()
    {
        myDotPos = GetComponentInChildren<Transform>();     // 자식으로 된 점이 내 시작 점의 좌표임.
        myLine.SetPosition(0, myDotPos.position);
    }


}

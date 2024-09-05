using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReader_Relationship : MonoBehaviour
{
    public GameObject linkLineDotPrefabs;
    public GameObject[] humans;
    private int nowHumanIndex = 0;

    public void AddHuman()
    {
        humans[nowHumanIndex].gameObject.SetActive(true);
        nowHumanIndex++;
    }
}

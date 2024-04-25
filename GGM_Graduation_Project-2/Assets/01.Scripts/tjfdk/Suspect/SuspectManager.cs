using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuspectManager : MonoBehaviour
{
    [SerializeField] private GameObject suspectPrefab;

    private void AddSuspect()
    {
        GameObject suspect = Instantiate(suspectPrefab);
        suspect.transform.SetParent(this.transform);

        
    }
}

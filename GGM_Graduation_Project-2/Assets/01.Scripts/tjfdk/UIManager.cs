using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void Panle_OnOff(GameObject panle)
    {
        //foreach (GameObject panle in panles)
            panle.SetActive(!panle.activeSelf);
    }
}

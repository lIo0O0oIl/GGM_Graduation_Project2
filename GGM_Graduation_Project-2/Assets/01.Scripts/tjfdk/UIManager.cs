using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;

    public void Panle_OnOff(GameObject panel)       // 셋팅창에서 사용함.
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void Panel_Popup(GameObject panel)
    {
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false);       // 전부 꺼주기
            panel.SetActive(true);      // 내꺼는 켜주기
        }
    }
}

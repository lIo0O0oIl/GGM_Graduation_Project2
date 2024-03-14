using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] List<GameObject> panels;

    public void Panle_OnOff(GameObject panel)
    {
        //foreach (GameObject panle in panles)
        panel.SetActive(!panel.activeSelf);
    }

    public void Panel_Popup(GameObject panel)
    {
        //GameObject button = EventSystem.current.currentSelectedGameObject;
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false); 
        }
        panel.SetActive(!panel.activeSelf);
    }
}

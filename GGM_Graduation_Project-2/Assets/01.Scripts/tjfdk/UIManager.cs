using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class UIManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] List<GameObject> panels;
    [SerializeField] GameObject alarmIcon;

    [Header("Connection System")]
    [SerializeField] GameObject connectionPanel;
    [SerializeField] Transform connectionParent;

    public void test()
    {
        alarmIcon.SetActive(!alarmIcon.activeSelf);
    }

    public void Panle_OnOff(GameObject panel)       // 셋팅창에서 사용함.
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void Panel_Popup(GameObject panel)
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false);       // 전부 꺼주기
            panel.SetActive(true);      // 내꺼는 켜주기
        }
    }

    public void ChangeParent()
    {
        connectionPanel.transform.parent.gameObject.SetActive(false);
        connectionParent.gameObject.SetActive(true);

        Transform temp = connectionPanel.transform.parent;
        connectionPanel.transform.SetParent(connectionParent);
        connectionParent = temp;

        connectionPanel.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        connectionPanel.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        connectionPanel.transform.SetAsFirstSibling();
    }

    public void SceneChange(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}

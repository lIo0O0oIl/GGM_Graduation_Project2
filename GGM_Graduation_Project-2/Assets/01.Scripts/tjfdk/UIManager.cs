using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Panel")]
    public List<GameObject> panels;
    public GameObject alarmIcon;

    public Action<int> startChatEvent;
    public int chatIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    [Header("Connection System")]
    [SerializeField] GameObject connectionPanel;
    [SerializeField] Transform connectionParent;

    public void test()
    {
        alarmIcon.SetActive(!alarmIcon.activeSelf);
    }

    public void Panle_OnOff(GameObject panel)       // 해당 패널 꺼주기
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void Panel_Popup(GameObject panel)
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false);       // 모드 패널 꺼주기
            panel.SetActive(true);      // 키려고 하는 패널은 켜주기
            if (panel.gameObject.name == panels[0].gameObject.name && startChatEvent == null)         // 알람이 올 때 채팅에 가야 다시 
            {
                startChatEvent.Invoke(chatIndex);
                startChatEvent -= (index) => ChattingManager.Instance.StartChatting(index);
            }
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public List<GameObject> panels;
    public GameObject alarmIcon;

    public Action<int> startChatEvent;
    public int chatIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

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
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false);       // 전부 꺼주기
            panel.SetActive(true);      // 내꺼는 켜주기
            if (panel.gameObject.name == panels[0].gameObject.name && startChatEvent == null)         // 만약 대화창이고 액션이 있으면 액션 호출 및 구독 해지
            {
                startChatEvent.Invoke(chatIndex);
                startChatEvent -= (index) => ChattingManager.Instance.StartChatting(index);
            }
        }
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

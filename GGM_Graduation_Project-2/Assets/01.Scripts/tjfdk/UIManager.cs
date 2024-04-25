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

    public void Panle_OnOff(GameObject panel)       // ����â���� �����.
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void Panel_Popup(GameObject panel)
    {
        Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        if (panel.activeSelf == false)
        {
            foreach (GameObject obj in panels)
                obj.SetActive(false);       // ���� ���ֱ�
            panel.SetActive(true);      // ������ ���ֱ�
            if (panel.gameObject.name == panels[0].gameObject.name && startChatEvent == null)         // ���� ��ȭâ�̰� �׼��� ������ �׼� ȣ�� �� ���� ����
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

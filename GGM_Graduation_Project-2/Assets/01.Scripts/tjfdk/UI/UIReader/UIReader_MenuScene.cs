using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class UIReader_MenuScene : MonoBehaviour
{
    [Header("Menu")]
    private UIDocument menuUI;
    private VisualElement menuRoot;

    [SerializeField] private Button startBtn;
    [SerializeField] private Button settingBtn;
    [SerializeField] private Button exitBtn;
    private VisualElement settingPanel;

    public bool isSettingOpen;

    private void OnEnable()
    {
        menuUI = GetComponent<UIDocument>();
        menuRoot = menuUI.rootVisualElement;

        startBtn = menuRoot.Q<Button>("PlayBtn");
        settingBtn = menuRoot.Q<Button>("SettingBtn");
        exitBtn = menuRoot.Q<Button>("ExitBtn");
        settingPanel = menuRoot.Q<VisualElement>("Setting");

        // you have to change this scene name, no tutorial! game!!!
        startBtn.clicked += (() => { UIManager.Instance.SceneChange("Game"); });
        settingBtn.clicked += (() => { OpenSetting(); });
        settingPanel.Q<Button>("ExitBtn").clicked += () => { OpenSetting(); };
        exitBtn.clicked += (() => { UIManager.Instance.Exit(); });
    }

    public void OpenSetting()
    {
        if (isSettingOpen)
            settingPanel.style.display = DisplayStyle.None;
        else
            settingPanel.style.display = DisplayStyle.Flex;

        isSettingOpen = !isSettingOpen;
    }
}

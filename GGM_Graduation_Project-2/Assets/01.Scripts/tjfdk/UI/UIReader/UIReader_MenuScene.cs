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

    private void OnEnable()
    {
        menuUI = GetComponent<UIDocument>();
        menuRoot = menuUI.rootVisualElement;

        startBtn = menuRoot.Q<Button>("Play");
        settingBtn = menuRoot.Q<Button>("Setting");
        exitBtn = menuRoot.Q<Button>("Exit");

        startBtn.clicked += (() => { UIManager.Instance.SceneChange("Game"); });
        settingBtn.clicked += (() => { UIManager.Instance.OpenSetting(true); });
        exitBtn.clicked += (() => { UIManager.Instance.Exit(); });
    }
}

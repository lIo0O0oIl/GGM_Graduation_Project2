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

    private Button startBtn;
    private Button settingBtn;
    private Button exitBtn;
    private VisualElement settingPanel;
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    public bool isSettingOpen;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
        startBtn.RegisterCallback<MouseEnterEvent>(evt => BtnSoundPlay());

        settingBtn.clicked += (() => { OpenSetting(); });
        settingBtn.RegisterCallback<MouseEnterEvent>(evt => BtnSoundPlay());        // 마우스 입력 시 소리
        settingPanel.Q<Button>("ExitBtn").clicked += () => { OpenSetting(); };

        exitBtn.clicked += (() => { UIManager.Instance.Exit(); });
        exitBtn.RegisterCallback<MouseEnterEvent>(evt => BtnSoundPlay());
    }

    public void OpenSetting()
    {
        if (isSettingOpen)
            settingPanel.style.display = DisplayStyle.None;
        else
            settingPanel.style.display = DisplayStyle.Flex;

        isSettingOpen = !isSettingOpen;
    }

    private void BtnSoundPlay()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(buttonClickSound);
    }
}

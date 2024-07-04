using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIReader_MenuScene : MonoBehaviour
{
    [Header("Menu")]
    private UIDocument menuUI;
    private VisualElement menuRoot;

    public UnityEngine.UI.Button startBtn;
    public UnityEngine.UI.Button settingBtn;
    public UnityEngine.UI.Button exitBtn;
    private VisualElement settingPanel;
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    public bool isSettingOpen;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        menuUI.enabled = false;
    }

    private void OnEnable()
    {
        menuUI = GetComponent<UIDocument>();
        menuRoot = menuUI.rootVisualElement;

        settingPanel = menuRoot.Q<VisualElement>("Setting");

        // you have to change this scene name, no tutorial! game!!!
        startBtn.onClick.AddListener(() => { UIManager.Instance.SceneChange("Game"); });

        settingBtn.onClick.AddListener(() => { OpenSetting(); });
        settingPanel.Q<UnityEngine.UIElements.Button>("ExitBtn").clicked += () => { OpenSetting(); };

        exitBtn.onClick.AddListener(() => { UIManager.Instance.Exit(); });
    }

    

    public void OpenSetting()
    {
        if (isSettingOpen)
        {
            menuUI.enabled = false;
        }
        else
        {
            menuUI.enabled = true;
        }

        isSettingOpen = !isSettingOpen;
    }

    public void BtnSoundPlay()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(buttonClickSound);
    }
}

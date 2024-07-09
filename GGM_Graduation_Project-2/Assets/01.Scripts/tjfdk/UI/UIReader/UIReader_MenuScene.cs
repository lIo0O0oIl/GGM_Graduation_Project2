using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIReader_MenuScene : MonoBehaviour
{
    [Header("Menu")]
    public UIDocument menuUI;

    public UnityEngine.UI.Button startBtn;
    public UnityEngine.UI.Button settingBtn;
    public UnityEngine.UI.Button exitBtn;
    private VisualElement settingPanel;
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    private UIReader_SettingScene settingScene;

    public bool isSettingOpen;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        settingScene = GetComponent<UIReader_SettingScene>();

        // you have to change this scene name, no tutorial! game!!!
        startBtn.onClick.AddListener(() => { UIManager.Instance.SceneChange("Game"); });

        settingBtn.onClick.AddListener(() => { OpenSetting(); });


        exitBtn.onClick.AddListener(() => { UIManager.Instance.Exit(); });
    }

    public void OnStart()
    {
        Debug.Log("dk");
        UIManager.Instance.SceneChange("Game");
    }

    public void OnExit()
    {
        Debug.Log("나가");
        UIManager.Instance.Exit();
    }

    public void OpenSetting()
    {
        Debug.Log(isSettingOpen);
        if (isSettingOpen)
        {
            menuUI.enabled = false;
            settingScene.enabled = false;
        }
        else
        {
            menuUI.enabled = true;
            // 설정창 켰음

            settingScene.enabled = true;

            menuUI.rootVisualElement.Q<Button>("ExitBtn").RegisterCallback<ClickEvent>(evt =>
            {
            OpenSetting();
            });

        }

        isSettingOpen = !isSettingOpen;
    }

    public void BtnSoundPlay()
    {
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(buttonClickSound);
    }
}

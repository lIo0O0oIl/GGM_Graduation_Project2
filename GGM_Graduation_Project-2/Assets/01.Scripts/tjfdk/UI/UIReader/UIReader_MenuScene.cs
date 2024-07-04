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
    public UIDocument menuUI;
    private VisualElement menuRoot;

    public UnityEngine.UI.Button startBtn;
    public UnityEngine.UI.Button settingBtn;
    public UnityEngine.UI.Button exitBtn;
    private VisualElement settingPanel;
    [SerializeField] private AudioClip buttonClickSound;
    private AudioSource audioSource;

    public bool isSettingOpen;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        menuRoot = menuUI.rootVisualElement;

        // you have to change this scene name, no tutorial! game!!!
        startBtn.onClick.AddListener(() => { UIManager.Instance.SceneChange("Game"); });

        settingBtn.onClick.AddListener(() => { OpenSetting(); });
        menuRoot.Q<UnityEngine.UIElements.Button>("ExitBtn").clicked += () => { OpenSetting(); };

        exitBtn.onClick.AddListener(() => { UIManager.Instance.Exit(); });

        StartCoroutine(min());
    }

    private void OnEnable()
    {
        Debug.Log(menuRoot.Q<UnityEngine.UIElements.Button>("ExitBtn"));
        menuRoot.Q<UnityEngine.UIElements.Button>("ExitBtn").clicked += () => 
        {
            Debug.Log("들어옴"); 
            OpenSetting();
        };
    }

    private IEnumerator min()
    {
        yield return new WaitForEndOfFrame();
        menuUI.enabled = false;
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

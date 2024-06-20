using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIReader_SettingScene : MonoBehaviour
{
    [SerializeField] ChatHumanManager chatHumanManager;

    [Header("Setting")]
    private UIDocument settingUI;
    private VisualElement settingRoot;

    private Button settingExitBtn;
    private Slider master;
    private Slider bgm;
    private Slider sfx;
    //private Slider scroll;
    //private Slider wheel;


    private void OnEnable()
    {
        settingUI = GetComponent<UIDocument>();
        settingRoot = settingUI.rootVisualElement;

        settingExitBtn = settingRoot.Q<Button>("ExitBtn");
        master = settingRoot.Q<Slider>("SliderMaster");
        bgm = settingRoot.Q<Slider>("SliderBGM");
        sfx = settingRoot.Q<Slider>("SliderSFX");
        //scroll = settingRoot.Q<Slider>("SliderScroll");
        //wheel = settingRoot.Q<Slider>("SliderWheel");

        settingExitBtn.clicked += (() => { UIManager.Instance.OpenSetting(false); });
        master.RegisterValueChangedCallback(OnMasterChange);
        bgm.RegisterValueChangedCallback(OnBGMChange);
        sfx.RegisterValueChangedCallback(OnSFXChange);

        //ChangeDefaultValue();
        //scroll.RegisterValueChangedCallback(OnChatSpeedChange);
        //wheel.RegisterValueChangedCallback(OnWheelSpeedhange);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.Find("Game"))
            chatHumanManager = GameObject.Find("Game").GetComponent<ChatHumanManager>();
    }

    public void ChangeDefaultValue()
    {
        master.value = VolumeManager.Instance.masterValue;
        bgm.value = VolumeManager.Instance.bgmValue;
        sfx.value = VolumeManager.Instance.sfxValue;
        //scroll.value = VolumeManager.Instance.scrollValue;
        //wheel.value = VolumeManager.Instance.wheelValue;
    }

    private void OnMasterChange(ChangeEvent<float> evt)
    {
        VolumeManager.Instance.masterValue = master.value;
        VolumeManager.Instance.Master(master.value);
    }


    private void OnBGMChange(ChangeEvent<float> evt)
    {
        VolumeManager.Instance.bgmValue = bgm.value;
        VolumeManager.Instance.BGM(bgm.value);
    }

    private void OnSFXChange(ChangeEvent<float> evt)
    {
        VolumeManager.Instance.sfxValue = sfx.value;
        VolumeManager.Instance.SFX(sfx.value);
    }

    //private void OnChatSpeedChange(ChangeEvent<float> evt)
    //{
    //    VolumeManager.Instance.scrollValue = scroll.value;
    //    UIManager.Instance.SetScrollSpeed(scroll.value);

    //    if (chatHumanManager)
    //        chatHumanManager.SetChatSpeed(UIManager.Instance.ScrollSpeed);
    //}

    //private void OnWheelSpeedhange(ChangeEvent<float> evt)
    //{
    //    VolumeManager.Instance.wheelValue = wheel.value;
    //    UIManager.Instance.SetWheelSpeed(wheel.value);

    //    if (chatHumanManager)
    //        chatHumanManager.SetWheelSpeed(UIManager.Instance.WheelSpeed);
    //}
}

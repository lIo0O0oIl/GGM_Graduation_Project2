using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIReader_SettingScene : MonoBehaviour
{
    //[SerializeField] ChatHumanManager chatHumanManager;

    [Header("Setting")]
    private UIDocument settingUI;
    private VisualElement root;

    private Slider master;
    private Slider bgm;
    private Slider sfx;
    private Slider wheel;
    //private Slider scroll;


    private void OnEnable()
    {
        settingUI = GetComponent<UIDocument>();
        root = settingUI.rootVisualElement;

        master = root.Q<Slider>("SliderMaster");
        bgm = root.Q<Slider>("SliderBGM");
        sfx = root.Q<Slider>("SliderSFX");
        wheel = root.Q<Slider>("SliderTextSpeed");

        master.RegisterValueChangedCallback(OnMasterChange);
        bgm.RegisterValueChangedCallback(OnBGMChange);
        sfx.RegisterValueChangedCallback(OnSFXChange);
        wheel.RegisterValueChangedCallback(OnWheelSpeedhange);
    }

    void OnDisable()
    {
        //SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    if (GameObject.Find("Game"))
    //        chatHumanManager = GameObject.Find("Game").GetComponent<ChatHumanManager>();
    //}

    private void Start()
    {
        master.value = VolumeManager.Instance.masterValue;
        bgm.value = VolumeManager.Instance.bgmValue;
        sfx.value = VolumeManager.Instance.sfxValue;
        wheel.value = VolumeManager.Instance.wheelValue;
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

    private void OnWheelSpeedhange(ChangeEvent<float> evt)
    {
        VolumeManager.Instance.wheelValue = wheel.value;
        //UIManager.Instance.SetWheelSpeed(wheel.value);

        //if (chatHumanManager)
        //    chatHumanManager.SetWheelSpeed(UIManager.Instance.WheelSpeed);
    }
}

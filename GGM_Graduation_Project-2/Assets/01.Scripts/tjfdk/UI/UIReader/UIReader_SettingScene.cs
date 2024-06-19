using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIReader_SettingScene : MonoBehaviour
{
    [Header("Setting")]
    private UIDocument settingUI;
    private VisualElement settingRoot;

    private Button settingExitBtn;
    private Slider master;
    private Slider bgm;
    private Slider sfx;
    private Slider scroll;
    private Slider wheel;

    [SerializeField] private float masterValue;
    [SerializeField] private float bgmValue;
    [SerializeField] private float sfxValue;
    [SerializeField] private float scrollValue;
    [SerializeField] private float wheelValue;

    private void Start()
    {
        masterValue = -20;
        bgmValue = -20;
        sfxValue = -20;
        scrollValue = 2.75f;
        wheelValue = 105;
    }

    private void OnEnable()
    {
        settingUI = GetComponent<UIDocument>();
        settingRoot = settingUI.rootVisualElement;

        settingExitBtn = settingRoot.Q<Button>("ExitBtn");
        master = settingRoot.Q<Slider>("SliderMaster");
        bgm = settingRoot.Q<Slider>("SliderBGM");
        sfx = settingRoot.Q<Slider>("SliderSFX");
        scroll = settingRoot.Q<Slider>("SliderScroll");
        wheel = settingRoot.Q<Slider>("SliderWheel");

        settingExitBtn.clicked += (() => { UIManager.Instance.OpenSetting(false); });
        master.RegisterValueChangedCallback(OnSliderValueChanged);
        bgm.RegisterValueChangedCallback(OnSliderValueChanged);
        sfx.RegisterValueChangedCallback(OnSliderValueChanged);
        scroll.RegisterValueChangedCallback(OnSliderValueChanged);
        wheel.RegisterValueChangedCallback(OnSliderValueChanged);

        master.value = masterValue;
        bgm.value = bgmValue;
        sfx.value = sfxValue;
        scroll.value = scrollValue;
        wheel.value = wheelValue;
    }

    public void BringDefaultValue()
    {
        master.value = masterValue;
        bgm.value = bgmValue;
        sfx.value = sfxValue;
        scroll.value = scrollValue;
        wheel.value = wheelValue;
    }

    private void OnSliderValueChanged(ChangeEvent<float> evt)
    {
        masterValue = master.value;
        bgmValue = bgm.value;
        sfxValue = sfx.value;
        scrollValue = scroll.value;
        wheelValue = wheel.value;
        wheelValue = wheel.value;

        VolumeManager.Instance.Master(master.value);
        VolumeManager.Instance.BGM(bgm.value);
        VolumeManager.Instance.SFX(sfx.value);
    }
}

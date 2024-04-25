using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider speedSlider;

    private void Start()
    {
        float master, bgm, sfx;
        mixer.GetFloat("Master", out master);
        mixer.GetFloat("BGM", out bgm);
        mixer.GetFloat("SFX", out sfx);
        masterSlider.value = master;
        bgmSlider.value = bgm;
        sfxSlider.value = sfx;
    }

    public void Master()
    {
        mixer.SetFloat("Master", masterSlider.value);
        mixer.SetFloat("BGM", masterSlider.value);
        mixer.SetFloat("SFX", masterSlider.value);
        if (speedSlider)
            ChattingManager.Instance.ChangeDelaySpeed(speedSlider.value);
    }

    public void BGM()
    {
        mixer.SetFloat("BGM", bgmSlider.value);
    }

    public void SFX()
    {
        mixer.SetFloat("SFX", sfxSlider.value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : Singleton<VolumeManager>
{
    [SerializeField] private AudioMixer mixer;
    //[SerializeField] private Slider masterSlider;
    //[SerializeField] private Slider bgmSlider;
    //[SerializeField] private Slider sfxSlider;
    //[SerializeField] private Slider speedSlider;

    //private void Start()
    //{
    //    float master, bgm, sfx;
    //    mixer.GetFloat("Master", out master);
    //    mixer.GetFloat("BGM", out bgm);
    //    mixer.GetFloat("SFX", out sfx);
    //}

    public void Master(float master)
    {
        mixer.SetFloat("Master", master);
        mixer.SetFloat("BGM", master);
        mixer.SetFloat("SFX", master);
    }

    public void BGM(float bgm)
    {
        mixer.SetFloat("BGM", bgm);
    }

    public void SFX(float sfx)
    {
        mixer.SetFloat("SFX", sfx);
    }
}

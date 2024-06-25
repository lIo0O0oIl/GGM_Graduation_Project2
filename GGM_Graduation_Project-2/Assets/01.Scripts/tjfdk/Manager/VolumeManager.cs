using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeManager : Singleton<VolumeManager>
{
    public AudioMixer mixer;
    public float masterValue;
    public float bgmValue;
    public float sfxValue;
    public float wheelValue;

    private void Start()
    {
        //masterValue = -20;    
        //bgmValue = -20;
        //sfxValue = -20;
        //scrollValue = 2.75f;
        //wheelValue = 105;
    }

    public void Master(float master)
    {
        mixer.SetFloat("Master", master);
        //mixer.SetFloat("BGM", master);
        //mixer.SetFloat("SFX", master);
    }

    public void BGM(float bgm)
    {
        mixer.SetFloat("BGM", bgm);
    }

    public void SFX(float sfx)
    {
        mixer.SetFloat("SFX", sfx);
    }

    public void WheelSpeed()
    {
        Debug.Log("fdfsdfasff");
    }
}

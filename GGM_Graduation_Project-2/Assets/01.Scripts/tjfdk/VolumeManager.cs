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

    public void Master()
    {
        mixer.SetFloat("Master", masterSlider.value);
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

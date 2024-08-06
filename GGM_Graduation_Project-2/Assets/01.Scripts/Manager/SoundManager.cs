using UnityEngine;

[System.Serializable]
public class Sound
{
    public string soundName;
    public AudioClip clip;
}

public class SoundManager : Singleton<SoundManager>
{
    public Sound[] bgmSounds;           // BGM 사운드 저장
    public Sound[] effectSounds;        // SFX 사운드 저장

    public AudioSource audioSourceBgmPlayers;           // BGM을 출력할 오디오 소스
    public AudioSource audioSourceEffectsPlayers;     // SFX를 출력할 오디오 소스

    private void Start()
    {
        PlayBGM("intro");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
            PlaySFX("click");
    }

    public void PlayBGM(string name) // BGM 실행
    {
        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (name == bgmSounds[i].soundName)
            {
                audioSourceBgmPlayers.clip = bgmSounds[i].clip;
                audioSourceBgmPlayers.loop = true;
                audioSourceBgmPlayers.Play();
                return;
            }
        }
    }

    public void PlaySFX(string name)
    {
        for (int i = 0; i < effectSounds.Length; i++)
        {
            if (name == effectSounds[i].soundName)
            {
                audioSourceEffectsPlayers.PlayOneShot(effectSounds[i].clip); 
                return;
            }
        }
    }


    public void StopBGM()
    {

        audioSourceBgmPlayers.Stop();
    }

    public void StopSFX()
    {
        audioSourceEffectsPlayers.Stop();
    }
}
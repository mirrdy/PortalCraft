using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Clip")]
    public Sound[] BGM;
    public Sound[] SFX;

    [Space(10f)]
    [Header("Audio Source")]
    [SerializeField] public AudioSource BGM_Play;
    [SerializeField] public AudioSource[] SFX_Play;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AudioSetting();
    }

    private void AudioSetting()
    {
        BGM_Play = transform.GetChild(0).GetComponent<AudioSource>();
        SFX_Play = transform.GetChild(1).GetComponents<AudioSource>();
        PlayerBGM("Title");
    }

    public void PlayerBGM(string name)
    {
        foreach (Sound s in BGM)
        {
            if (s.name.Equals(name))
            {
                BGM_Play.clip = s.clip;
                BGM_Play.Play();
                break;
            }
            print(string.Format("BGM_Play : {0}가 없습니다.", name));
        }
    }

    public void StopBGM()
    {
        BGM_Play.Stop();
    }

    public void PlaySFX(string name)
    {
        foreach (Sound s in SFX)
        {
            if (s.name.Equals(name))  // clip을 찾고 
            {
                for (int i = 0; i < SFX_Play.Length; i++)
                {
                    if (!SFX_Play[i].isPlaying)
                    {
                        SFX_Play[i].clip = s.clip;
                        SFX_Play[i].Play();
                        return;
                    }
                }
                print("모든 플레이어가 재생중...\n 호출 Method : PlayerSFX");
                return;
            }
            print(string.Format("SFX_Play : {0}가 없습니다.", name));
        }
    }
}

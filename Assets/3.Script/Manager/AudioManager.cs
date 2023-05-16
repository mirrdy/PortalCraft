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
    public Sound[] bgm;
    public Sound[] sfx;

    [Space(10f)]
    [Header("Audio Source")]
    [SerializeField] public AudioSource bgmPlay;
    [SerializeField] public AudioSource[] sfxPlay;

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
        bgmPlay = transform.GetChild(0).GetComponent<AudioSource>();
        sfxPlay = transform.GetChild(1).GetComponents<AudioSource>();
        PlayerBGM("Title");
    }

    public void PlayerBGM(string name)
    {
        foreach (Sound s in bgm)
        {
            if (s.name.Equals(name))
            {
                bgmPlay.clip = s.clip;
                bgmPlay.Play();
                break;
            }
            print(string.Format("BGM_Play : {0}가 없습니다.", name));
        }
    }

    public void StopBGM()
    {
        bgmPlay.Stop();
    }

    public void PlaySFX(string name)
    {
        foreach (Sound s in sfx)
        {
            if (s.name.Equals(name))  // clip을 찾고 
            {
                for (int i = 0; i < sfxPlay.Length; i++)
                {
                    if (!sfxPlay[i].isPlaying)
                    {
                        sfxPlay[i].clip = s.clip;
                        sfxPlay[i].Play();
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

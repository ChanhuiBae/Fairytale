using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BGM_Type
{
    BGM_Home = 0,
    BGM_Stage = 1,
    BGM_Boss = 2,
}

public enum SFX_Type
{
    SFX_Fire01,
    SFX_Explosion_01,
}

public class SoundManager : MonoBehaviour
{
    private AudioSource bgmAudio;
    private List<AudioClip> bgmList;

    public void ChangeBGM(BGM_Type newBGM)
    {
        StartCoroutine(ChangeBGMClip(bgmList[(int)newBGM]));
    }

    private float current;
    private float percent;
    IEnumerator ChangeBGMClip(AudioClip audioClip)
    {
        current = 0;
        percent = 0;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / 1f;
            bgmAudio.volume = Mathf.Lerp(1f, 0f, percent);
            yield return null;
        }

        bgmAudio.clip = audioClip;
        bgmAudio.Play();
        current = 0;
        percent = 0;

        while (percent < 1f)
        {
            current += Time.deltaTime;
            percent = current / 1f;
            bgmAudio.volume = Mathf.Lerp(0f, 1f, percent);
            yield return null;
        }
    }

    private int curser = 0;
    [SerializeField]
    private List<AudioSource> sfxPlayers;

    [SerializeField]
    private List<AudioClip> sfxList;

    public void PlaySFX(SFX_Type SFX)
    {
        sfxPlayers[curser].clip = sfxList[(int)SFX];
        sfxPlayers[curser].Play();

        curser++;
        if (curser > 9)
        {
            curser = 0;
        }
    }

    private static SoundManager instance;
    public static SoundManager Inst
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
}

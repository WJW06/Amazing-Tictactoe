using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioManager;

    [Header("#BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;

    [Header("#SFX")]
    public AudioClip[] sfxClips;
    public int channels;
    public float sfxVolume;
    AudioSource[] sfxPlayers;
    int channelIndex;

    public enum Sfx
    {
        Player1,
        Player2,
        Item,
        Hammer,
        HandGun,
        Shotgun,
        WildCard,
        Win,
        Lose
    }

    private void Awake()
    {
        audioManager = this;
        Init();
    }

    void Init()
    {
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = true;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;

        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for (int i = 0; i < channels; ++i)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = true;
            sfxPlayers[i].volume = sfxVolume;
        }

        PlayBgm(true);
    }

    public void PlayBgm(bool isPlay)
    {
        if (isPlay) bgmPlayer.Play();
        else bgmPlayer.Stop();
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < channels; ++i)
        {
           int loopIndex = (i + channelIndex) % channels;

            if (sfxPlayers[loopIndex].isPlaying) continue;

            channelIndex = loopIndex;
            sfxPlayers[channelIndex].clip = sfxClips[(int)sfx];
            if (sfx == Sfx.WildCard) sfxPlayers[channelIndex].pitch = 2;
            else sfxPlayers[channelIndex].pitch = 1;
            sfxPlayers[channelIndex].Play();
            break;
        }
    }
}

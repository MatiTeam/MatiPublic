using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum BGM
{
    TitleSound,
    PlaySceneSound,
    CareFree,
    YSB_Entrance,
    MeetMati,
    MeetBoss,
    CircusShow,
    BossSceneBGM,
    AfterBossStage
}

public enum SFX
{
    TouchSound,
    HitSound,    
    AttackSound,
    WalkSound,
    JumpSound,
    WStack1Sound,
    WStack3Sound,
    Explosion,
    Cheer,
    YSB_Laughter,
    Umm,
    BossSkillBullet,
    BossSkillJumpDown,
    Failed,
    BossDie,
    SkillR
}

public class AudioManager : MonoSingleton<AudioManager>
{
    public AudioMixer audioMixer;
    public AudioSource curBgm;
    public AudioSource curSfx;

    private Dictionary<BGM, AudioClip> bgmPlayer = new Dictionary<BGM, AudioClip>();
    private Dictionary<SFX, AudioClip> sfxPlayer = new Dictionary<SFX, AudioClip>();

    public float SFXVolume { get; private set; }

    private const string AudioPath = "Audio";

    protected override void Init()
    {
        base.Init();
        
        audioMixer = Resources.Load($"{AudioPath}/AudioGroup", typeof(AudioMixer)) as AudioMixer;

        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = this.transform;
        curBgm = bgmObject.AddComponent<AudioSource>();
        
        GameObject sfxObject = new GameObject("SFXPlayer");
        sfxObject.transform.parent = this.transform;
        curSfx = sfxObject.AddComponent<AudioSource>();

        if (audioMixer != null)
        {
            curBgm.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
            curSfx.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }

        LoadBGMPlayer();
        LoadSFXPlayer();
    }

    #region 믹서 볼륨 변경
    public void SetMasterVol(float vol)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(vol) * 20);
    }

    public void SetBGMVol(float vol)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(vol) * 20);
    }

    public void SetSFXVol(float vol)
    {
        SFXVolume = vol;
        audioMixer.SetFloat("SFX", Mathf.Log10(vol) * 20);
    }
    #endregion
    
    #region 사운드 출력
    // BGM 저장하기
    public void LoadBGMPlayer()
    {
        foreach (var bgm in Enum.GetValues(typeof(BGM)))
        {
            var audioName = bgm.ToString();
            var path = $"{AudioPath}/BGM/{audioName}";
            var audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
            
            bgmPlayer[(BGM)bgm] = audioClip;
        }
    }

    // SFX 저장하기
    public void LoadSFXPlayer()
    {
        foreach (var sfx in Enum.GetValues(typeof(SFX)))
        {
            var audioName = sfx.ToString();
            var path = $"{AudioPath}/SFX/{audioName}";
            var audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
            
            sfxPlayer[(SFX)sfx] = audioClip;
        }
    }
    
    // BGM 출력
    public void PlayBGM(BGM bgm)
    {
        if (curBgm.clip == bgmPlayer[bgm] && curBgm.isPlaying)
            return;

        curBgm.volume = BGMVolumeClip(bgm);
        
        curBgm.Stop();
        curBgm.clip = bgmPlayer[bgm];
        curBgm.loop = true;
        curBgm.Play();
    }
    
    // SFX 출력
    public void PlaySFX(string sfxName, float volume = 1.0f, float pitch = 1.0f)
    {
        if (string.IsNullOrEmpty(sfxName))
        {
            Debug.LogWarning("SFX name is empty or null");
            return;
        }

        if (curSfx.clip != null)
        {
            curSfx.Stop();
        }

        curSfx.clip = GetSFXClip((SFX)Enum.Parse(typeof(SFX), sfxName));
        curSfx.volume = volume;
        curSfx.pitch = pitch;
        curSfx.loop = false;
        curSfx.Play();
    }
    
    // BGM 정지
    public void PauseBGM()
    {
        if (curBgm.isPlaying)
            curBgm.Pause();
    }
    
    // BGM 재시작
    public void ResumeBGM()
    {
        if (curBgm.isPlaying)
            curBgm.UnPause();
    }
    
    // BGM 종료
    public void StopBGM()
    {
        if (curBgm.isPlaying)
            curBgm.Stop();
    }

    // BGM 찾기
    public AudioClip GetBGMClip(BGM bgm)
    {
        if (!bgmPlayer.ContainsKey(bgm))
            return null;
        else
            return bgmPlayer[bgm];
    }

    // SFX 찾기   
    public AudioClip GetSFXClip(SFX sfx)
    {
        if (!sfxPlayer.ContainsKey(sfx))
            return null;
        else
            return sfxPlayer[sfx];
    }
    #endregion

    private float BGMVolumeClip(BGM bgm)
    {
        float defaultVolume = 0.5f;
        return defaultVolume;
    }
}

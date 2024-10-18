using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioSources> _audioSourcesList = new List<AudioSources>();
    [SerializeField, Header("BGMSource")] private AudioSource _BGMSource;
    [SerializeField, Header("MESource")] private AudioSource _MESource;
    [SerializeField, Header("SESource")] private AudioSource _SESource;
    [SerializeField, Header("VoiceSource")] private AudioSource _VoiceSource;
    [SerializeField, Header("EnvironmentSource")] private AudioSource _environmentSource;

    /// <summary>
    /// BGMの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBGM(string soundName)
    {
        if (_audioSourcesList.Find(x => x.SoundName == soundName) == null)
        {
            Debug.LogError($"{soundName}の音がないよ");
            return;
        }

        _BGMSource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _BGMSource.Play();
    }

    /// <summary>
    /// MEの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayME(string soundName)
    {
        if (_audioSourcesList.Find(x => x.SoundName == soundName) == null)
        {
            Debug.LogError($"{soundName}の音がないよ");
            return;
        }

        _MESource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _MESource.Play();
    }

    /// <summary>
    /// SEの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlaySE(string soundName)
    {
        if (_audioSourcesList.Find(x => x.SoundName == soundName) == null)
        {
            Debug.LogError($"{soundName}の音がないよ");
            return;
        }

        _SESource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _SESource.Play();
    }

    /// <summary>
    /// Voiceの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayVoice(string soundName)
    {
        if (_audioSourcesList.Find(x => x.SoundName == soundName) == null)
        {
            Debug.LogError($"{soundName}の音がないよ");
            return;
        }

        _VoiceSource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _VoiceSource.Play();
    }
    
    /// <summary>
    /// 環境音の再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayEnvironmentSounds(string soundName)
    {
        if (_audioSourcesList.Find(x => x.SoundName == soundName) == null)
        {
            Debug.LogError($"{soundName}の音がないよ");
            return;
        }

        _environmentSource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _environmentSource.Play();
    }

    /// <summary>
    /// BGMの停止機能
    /// </summary>
    public void StopBGM()
    {
        _BGMSource.Stop();
    }

    /// <summary>
    /// MEの停止機能
    /// </summary>
    public void StopME()
    {
        _MESource.Stop();
    }

    /// <summary>
    /// SEの停止機能
    /// </summary>
    public void StopSE()
    {
        _SESource.Stop();
    }

    /// <summary>
    /// Voiceの停止機能
    /// </summary>
    public void StopVoice()
    {
        _VoiceSource.Stop();
    }
    
    /// <summary>
    /// 環境音の停止機能
    /// </summary>
    public void StopEnvironmentSounds()
    {
        _environmentSource.Stop();
    }

    /// <summary>
    /// BGMの音量変更
    /// </summary>
    public void ChamgeBGMVolume(float volume)
    {
        _BGMSource.volume = volume;
    }
    /// <summary>
    /// BGMの音量変更
    /// </summary>
    public void ChamgeMEVolume(float volume)
    {
        _MESource.volume = volume;
    }
    /// <summary>
    /// BGMの音量変更
    /// </summary>
    public void ChamgeSEVolume(float volume)
    {
        _SESource.volume = volume;
    }
    /// <summary>
    /// BGMの音量変更
    /// </summary>
    public void ChamgeVoiceVolume(float volume)
    {
        _VoiceSource.volume = volume;
    }
    /// <summary>
    /// 環境音の音量変更
    /// </summary>
    public void ChamgeEnvironmentSoundsVolume(float volume)
    {
        _environmentSource.volume = volume;
    }
}

[Serializable]
public class AudioSources
{
    public string SoundName;
    [FormerlySerializedAs("clip")] public AudioClip Clip;
}

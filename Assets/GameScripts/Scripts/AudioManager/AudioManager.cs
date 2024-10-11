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

    /// <summary>
    /// BGMの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayBGM(string soundName)
    {
        if(_audioSourcesList.Find(x => x.SoundName == soundName) == null) return;
        _BGMSource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _BGMSource.Play();
    }
    /// <summary>
    /// MEの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayME(string soundName)
    {
        if(_audioSourcesList.Find(x => x.SoundName == soundName) == null) return;
        _MESource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _MESource.Play();
    }
    /// <summary>
    /// SEの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlaySE(string soundName)
    {
        if(_audioSourcesList.Find(x => x.SoundName == soundName) == null) return;
        _SESource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _SESource.Play();
    }
    /// <summary>
    /// Voiceの再生機能
    /// </summary>
    /// <param name="soundName"></param>
    public void PlayVoice(string soundName)
    {
        if(_audioSourcesList.Find(x => x.SoundName == soundName) == null) return;
        _VoiceSource.clip = _audioSourcesList.Find(x => x.SoundName == soundName).Clip;
        _VoiceSource.Play();
    }/// <summary>
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
}

[Serializable]
public class AudioSources
{
    public string SoundName;
    [FormerlySerializedAs("clip")] public AudioClip Clip;
}

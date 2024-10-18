using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class EnvironmentalSounds : MonoBehaviour
{
    private AudioManager _audioManager;
    [SerializeField] private BetweenMinMax _betweenTime;
    

    private void Start()
    {
        _audioManager = FindAnyObjectByType<AudioManager>();
        BirdENvironment();
    }

    /// <summary>
    /// 風環境音再生
    /// </summary>
    private void PlayWindEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("Wind");
    }
    /// <summary>
    /// 川環境音再生
    /// </summary>
    private void PlayRiverEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("River");
    }
    /// <summary>
    /// 滝環境音再生
    /// </summary>
    private void PlayWaterFallEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("WaterFall");
    }
    /// <summary>
    /// 血環境音再生
    /// </summary>
    private void PlayBloodyEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("Bloody");
    }
    /// <summary>
    /// 水が滴る環境音再生
    /// </summary>
    private void PlayDropWaterEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("WaterDroplets");
    }
    /// <summary>
    /// うぐいす環境音再生
    /// </summary>
    private void PlayWarblersEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("Warblers");
    }
    /// <summary>
    /// キジバト環境音再生
    /// </summary>
    private void PlayPigeonEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("PheasantPigeon");
    }
    /// <summary>
    /// カラス環境音再生
    /// </summary>
    private void PlayCrowEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("Crow");
    }
    /// <summary>
    /// オオルリ環境音再生
    /// </summary>
    private void PlayOruriEnviromentSounds()
    {
        if (!_audioManager) return;
        _audioManager.PlayEnvironmentSounds("Oruri");
    }
    
    private async  void BirdENvironment()
    {
        while (true)
        {
            float randTime = Random.Range(_betweenTime.Min, _betweenTime.Max);
            int randSounds = Random.Range(1, 100);

            await UniTask.Delay(TimeSpan.FromSeconds(randTime));

            if (randSounds > 25)
            {
                PlayWarblersEnviromentSounds();
            }
            else if (randSounds > 50)
            {
                PlayPigeonEnviromentSounds();
            }
            else if (randSounds > 75)
            {
                PlayCrowEnviromentSounds();
            }
            else
            {
                PlayOruriEnviromentSounds();
            }

            PlayWindEnviromentSounds();
            
            Random.InitState(Random.Range(1, 255));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        _audioManager.StopEnvironmentSounds();

        PlayRiverEnviromentSounds();
        PlayWaterFallEnviromentSounds();
    }
}

[Serializable]
public class BetweenMinMax
{
    [SerializeField, Range(1f ,100f)] public float Min;
    [SerializeField, Range(1f, 100f)] public float Max;
}

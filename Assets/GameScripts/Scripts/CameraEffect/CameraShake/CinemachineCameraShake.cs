using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineCameraShake : MonoBehaviour
{
    /// <summary>
    /// 動かすVirtualCamera
    /// </summary>
    CinemachineVirtualCamera _cinemaVc;
    [SerializeField, Tooltip("揺らす強度")] float _shakeAmplitube = 1f;
    [SerializeField, Tooltip("揺らす速さ")] float _shakeFrequency = 1f;
    [SerializeField, Tooltip("揺らす時間")] float _shakeTime = 0.2f;
    /// <summary>
    /// 揺れの制御変数
    /// </summary>
    CinemachineBasicMultiChannelPerlin _multiChannelPerlin;

    private void Start()
    {
        _cinemaVc = GetComponent<CinemachineVirtualCamera>();
        StopShake();
    }

    /// <summary>
    /// カメラシェイクを行う時に呼ぶ処理
    /// </summary>
    public void ShakeCamera()
    {
        CinemachineBasicMultiChannelPerlin _multiChannelPerlin = _cinemaVc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _multiChannelPerlin.m_AmplitudeGain = _shakeAmplitube;
        _multiChannelPerlin.m_FrequencyGain = _shakeFrequency;
    }

    public void StopShake()
    {
        CinemachineBasicMultiChannelPerlin _multiChannelPerlin = _cinemaVc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _multiChannelPerlin.m_AmplitudeGain = 0;
        _multiChannelPerlin.m_FrequencyGain = 0;
    }


}

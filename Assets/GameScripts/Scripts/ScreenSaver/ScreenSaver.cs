using System;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// 一定時間放置をすると、指定のビデオを再生する機能を提供する。
/// </summary>
public class ScreenSaver : MonoBehaviour
{
    [SerializeField,Tooltip("タイトルの背景")] GameObject _titleImage;
    [SerializeField] VideoPlayer _videoPlayer;  // ヒエラルキーにステージした動画ファイル
    [SerializeField, Range(0, 100), Tooltip("movieが再生される時間")] float _moviePlayTime;
    
    /// <summary>経過時間を記録する変数</summary>
    float _elapsedTime;

    private void Start()
    {
        var audioSource = GameObject.FindObjectOfType<AudioSource>();   // AudioSource
        _videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _videoPlayer.targetCamera = Camera.main;
        _videoPlayer.SetTargetAudioSource(0, audioSource);
    }

    void Update()
    {
        //ビデオが再生されていないときに毎フレームの経過時間を変数に代入する
        if (!_videoPlayer.isPlaying && _moviePlayTime > _elapsedTime)
        {
            _elapsedTime += Time.deltaTime;
        }
        //経過時間が_moviePlayTimeの数値を超えたらビデオを再生する
        if (_moviePlayTime < _elapsedTime)
        {
            _videoPlayer.Play();
            _elapsedTime = 0;
        }
        //ビデオが再生されているとき背景の表示をオフにする
        if (_videoPlayer.isPlaying)
        {
            _titleImage.SetActive(false);
        }
        else
        {
            _titleImage.SetActive(true);
        }
    }
}

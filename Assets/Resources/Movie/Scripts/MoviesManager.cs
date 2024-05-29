using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 作成 : 五島
/// <summary>
/// ムービーの再生等を行います
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class MoviesManager : MonoBehaviour
{
    [SerializeField, Header("ムービー用タイムライン")] private MoveData[] _timelines;

    private PlayableDirector _playableDirector;

    void Start()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    /// <summary>ムービー再生を行います</summary>
    public void PlayTimeline(MoveData.MoveType type)
    {
        TimelineAsset timeline = _timelines.FirstOrDefault(move => move.Type == type).Timeline;
        _playableDirector.Play(timeline);
    }
    /// <summary>ムービー再生を行います</summary>
    public void PlayTimeline(int id)
    {
        TimelineAsset timeline = _timelines[id].Timeline;
        _playableDirector.Play(timeline);
    }
    
    //一時停止する
    void PauseTimeline()
    {
        _playableDirector.Pause();
    }
 
    //一時停止を再開する
    void ResumeTimeline()
    {
        _playableDirector.Resume();
    }
        
    //停止する
    void StopTimeline()
    {
        _playableDirector.Stop();       
    }
}

/// <summary>ムービー用のデータ</summary>
[System.Serializable]
public struct MoveData
{
    // ムービーの種類
    public enum MoveType
    {
        Tutorial,
        ConfrontingTheBoss,
        DefeatingTheBoss,
        Ending,
    }
    
    public MoveType Type;
    public TimelineAsset Timeline;
}

using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 作成 : 五島
/// <summary>
/// ムービーの再生を行います
/// </summary>
[RequireComponent(typeof(PlayableDirector))]
public class MovesManager : MonoBehaviour
{
    [SerializeField, Header("ムービー用タイムライン")] private MoveData[] _timelines;

    private PlayableDirector _playableDirector;

    void Start()
    {
        _playableDirector = GetComponent<PlayableDirector>();
    }

    /// <summary>ムービー再生を行います</summary>
    public void PlayMove(MoveData.MoveType type)
    {
        TimelineAsset timeline = _timelines.FirstOrDefault(move => move.Type == type).Timeline;
        _playableDirector.Play(timeline);
    }
    /// <summary>ムービー再生を行います</summary>
    public void PlayMove(int id)
    {
        TimelineAsset timeline = _timelines[id].Timeline;
        _playableDirector.Play(timeline);
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

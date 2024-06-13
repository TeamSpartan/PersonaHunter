using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 実装するプレイヤのセーブデータを保持するクラス
/// </summary>
public class ClientDataHolder : MonoBehaviour, IPlayerDataContainable
{
    private static InGameSceneStatus _currentSceneStatus;
    
    private static bool _playedPrologue = false;

    /// <summary>
    /// シーンのステータスを保持
    /// </summary>
    public enum InGameSceneStatus
    {
        /// <summary> タイトルシーン </summary>
        Title,
        /// <summary> 絵巻物シーン </summary>
        Prologue,
        /// <summary> インゲームシーン </summary>
        InGame,
        /// <summary> ボスシーン遷移直後 </summary>
        TransitedBossScene,
        /// <summary> ボス登場ムービー再生中 </summary>
        PlayingAppearanceMovie,
        /// <summary> ボス登場ムービー再生終了 </summary>
        FinishedPlayingAppearanceMovie,
        /// <summary> ボス撃破ムービー再生中 </summary>
        PlayingDefeatedMovie,
        /// <summary> ボス撃破ムービー再生終了 </summary>
        FinishedPlayingDefeatedMovie,
        /// <summary> スタッフロールシーン </summary>
        Epilogue,
    }

    public bool PlayedPrologue
    {
        get { return _playedPrologue; }
    }

    public InGameSceneStatus CurrentSceneStatus
    {
        get { return _currentSceneStatus; }
        set { _currentSceneStatus = value; }
    }

    public void NotifyPlayedPrologue()
    {
        _playedPrologue = true;
    }

    public void SaveData()
    {
        throw new System.NotImplementedException();
    }

    public void LoadData()
    {
        throw new System.NotImplementedException();
    }
}

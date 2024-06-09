using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 実装するプレイヤのセーブデータを保持するクラス
/// </summary>
public class ClientDataHolder : MonoBehaviour, IPlayerDataContainable
{
    private static bool _playedPrologue = false;

    public bool PlayedPrologue
    {
        get
        {
            return _playedPrologue;
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 暫定的に実装するプレイヤのセーブデータを保持するクラス
/// </summary>
public class TemporaryPlayerDataHolder : MonoBehaviour, IPlayerDataContainable
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
}

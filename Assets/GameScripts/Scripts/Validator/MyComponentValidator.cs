using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ユーザー定義のゲーム開始時にヴァリデーション処理を受け持つクラス。
/// </summary>
public class MyComponentValidator : MonoBehaviour
{
    private bool _playedPrologue;

    // Start is called before the first frame update
    void Start()
    {
        Validation();
    }

    private void Validation()
    {
        // プロローグを再生したかの静的フィールドにアクセス
        var tempData = Resources.Load<TemporaryPlayerDataHolder>("Prefabs/GameSystem/TemporaryPlayerDataHolder");
        if (tempData is not null)
        {
            _playedPrologue = tempData.PlayedPrologue;
        }

        // タイトル画面のバックグラウンドのオブジェクト
        var obj = GameObject.Find("TitleImageBackGround");
        var group = obj.transform.GetComponentInChildren<CanvasGroup>();
        if (group is not null)
        {
            group.alpha = _playedPrologue ? 1 : 0;
            group.interactable = group.blocksRaycasts = _playedPrologue;
        }
        else
        {
            // Debug.Log($"TitleImageBackGround Is null");
        }

        // PressAnyButtonのパネル
        obj = GameObject.Find("PressAnyButtonPanel");
        if (obj is not null)
        {
            if (_playedPrologue)
            {
                GameObject.Destroy(obj);
            }
        }
        else
        {
            // Debug.Log($"PressAnyButtonPanel Is null");
        }
    }
}

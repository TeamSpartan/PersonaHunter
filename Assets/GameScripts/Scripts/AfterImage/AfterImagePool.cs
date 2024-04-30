using System.Collections;
using System.Collections.Generic;
using UniRx.Toolkit;
using UnityEngine;

/// <summary>
/// 残像オブジェクトのプーリングクラス
/// </summary>
public class AfterImagePool : ObjectPool<AfterImageBase>
{
    /// <summary>
    /// 残像用オブジェクト
    /// </summary>
    private AfterImageBase _gameObject = null;
    /// <summary>
    /// 生成オブジェクトの親オブジェクト
    /// </summary>
    private Transform _parent = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="parent"></param>
    public AfterImagePool(AfterImageBase gameObject, Transform parent)
    {
        _gameObject = gameObject;
        _parent = parent;
    }

    ~AfterImagePool() 
    { 
        Clear();
    }

    protected override AfterImageBase CreateInstance()
    {
        if (_gameObject == null)
        {
            Debug.LogError("生成元のGameObjectがnullです");
            return null;
        }
        if (_parent == null)
        {
            Debug.LogError("親Transformがnullです");
            return null;
        }

        AfterImageBase afterImage = GameObject.Instantiate<AfterImageBase>(_gameObject, _parent, true);
        afterImage.Initialize();
        return afterImage;
    }

    /// <summary>
    /// 生成オブジェクトの設定
    /// </summary>
    /// <param name="gameObject">残像用オブジェクト</param>
    public void SetPoolObject(AfterImageBase gameObject)
    {
        _gameObject = gameObject;
    }

    /// <summary>
    /// 生成オブジェクトの親オブジェクトの設定
    /// </summary>
    /// <param name="parent">残像の親オブジェクト</param>
    public void SetParent(Transform parent)
    {
        _parent = parent;
    }
}

using System.Collections.Generic;
using UnityEngine;

public interface IAfterImageSetupParam { }

/// <summary>
/// 残像出現時に設定するためのパラメーター抽象クラス
/// </summary>
[System.Serializable]
public class SimpleAfterImageParam : IAfterImageSetupParam
{
    public Transform Transform = null;
    public SimpleAfterImageParam() { }
    public SimpleAfterImageParam(Transform original)
    {
        Transform = original;
    }
    ~SimpleAfterImageParam()
    {
        Transform = null;
    } 
}

/// <summary>
/// 複数のボーンを使用する場合
/// </summary>
[System.Serializable]
public class AfterImageSkinnedMeshParam : IAfterImageSetupParam
{
    public List<Transform> Transforms = null;
    public AfterImageSkinnedMeshParam() { }
    public AfterImageSkinnedMeshParam(List<Transform> original)
    {
        Transforms = original;
    }
    ~AfterImageSkinnedMeshParam()
    {
        Transforms.Clear();
        Transforms = null;
    }
}

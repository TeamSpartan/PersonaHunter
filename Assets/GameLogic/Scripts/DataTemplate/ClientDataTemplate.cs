using System;
using UnityEngine;

[Serializable]
public class ClientDataTemplate : MonoBehaviour
{
    /// <summary>
    /// プレイヤのトランスフォーム情報
    /// </summary>
    public Transform PlayerTransform;

    /// <summary>
    /// シーン名
    /// </summary>
    public string SceneName;
}
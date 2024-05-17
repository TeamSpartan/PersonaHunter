using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// コライダーではなくPhysicsクラスの重なり合わせ判定をする場合にはこれをアタッチして使用する
/// </summary>
public class Overlappable : MonoBehaviour, ICollisionOverLappable
{
    [SerializeField, Header("当たり判定イベント")] private UnityEvent _event;
    public void NotifyOverlap(Transform other)
    {
        _event?.Invoke();
    }
}

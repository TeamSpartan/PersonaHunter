using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 特定のオブジェクトが特定の意当たり判定のバウンズ内に入ったら指定のメソッドを呼ぶ
/// </summary>
public class OverlapDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _targetLayerMask;

    [SerializeField] private Vector3 _bounds;

    [SerializeField] private UnityEvent _unityEvent;

    private void Update()
    {
        if (Physics.CheckBox(transform.position, _bounds / 2, transform.rotation, _targetLayerMask))
        {
            _unityEvent.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, _bounds);
    }
}

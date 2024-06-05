using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定している距離より遠い場合には非アクティブにする機能を提供する
/// </summary>
public class MapleTreeCuller : MonoBehaviour
{
    [SerializeField, Header("カリング開始する距離")] private float _distanceToCulling;

    [SerializeField, Header("このオブジェクトとの距離を計測する")]
    private Transform _target;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, _distanceToCulling);
    }
    
    private void LateUpdate()
    {
        // カリングを開始する距離より近い場合True
        var condition = Vector3.Distance(transform.position, _target.position) < _distanceToCulling;
        this.gameObject.SetActive(condition);
    }
}

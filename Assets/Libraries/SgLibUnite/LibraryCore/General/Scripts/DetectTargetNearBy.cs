// 管理者 菅沼
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace SgLibUnite
{
    enum DetectingMode
    {
        LayerMask,
        Transform,
    }
    /// <summary> 特定の対象が任意の座標の近くにいるのを検知した時のイベント </summary>
    public class DetectTargetNearBy : MonoBehaviour
    {
        // 検知圏内のとる形（球）の半径
        [SerializeField, Range(0, 5),
            Header("境界線の半径。\nスライダーを動かすと、境界線が広がる")]
        float borderRadius;
        // 検知対象のレイヤーマスク
        [SerializeField,
            Header("検知対象のレイヤーにチェックボックスを入れればよい")]
        LayerMask targetLayer;
        // 検知対象のトランスフォーム
        [SerializeField,
            Header("検知モードがTransformの場合\n検知対象のをここにドラッグアンドドロップで割り当てる")]
        Transform targetTransform;
        // 検知した時のイベント
        [SerializeField,
            Header("ここに検知対象を検知した場合\n発火してほしいイベントを割り当てる")]
        UnityEvent eventDetectedInsideBorder;
        // 検知圏内の中心のトランスフォーム
        [SerializeField, Header("検知モードがTransformの時\n検知基準座標（オブジェクト）をここに割り当てる")]
        Transform centerTransform;
        // 検知モード
        [SerializeField, Header("=検知モード=\n" +
            "LayerMask:\nアタッチしたオブジェクトの座標をもとに検知する\n１つのオブジェクトの侵入のみ検知するときに使うとよい\n" +
            "Transform:\n割り当てたレイヤーの不特定数のオブジェクトの侵入を検知するときに使うとよい")]
        DetectingMode mode;
        private void Update()
        {
            switch (mode)
            {
                case DetectingMode.LayerMask:
                    if (Physics.CheckSphere(centerTransform.position, borderRadius, targetLayer))
                        eventDetectedInsideBorder?.Invoke();
                    break;
                case DetectingMode.Transform:
                    if ((targetTransform.position - centerTransform.position).sqrMagnitude < borderRadius * borderRadius)
                        eventDetectedInsideBorder?.Invoke();
                    break;
                default:
                    throw new System.Exception("検知モードが割り当てられていません");
            }
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centerTransform.position, borderRadius);
        }
#endif
    }
}
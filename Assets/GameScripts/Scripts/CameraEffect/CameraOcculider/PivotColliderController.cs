using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider),typeof(Rigidbody))]
public class PivotColliderController : MonoBehaviour
{
    [SerializeField, Tooltip("コライダーの始点")] Transform _start;
    [SerializeField, Tooltip("コライダーの終点")] Transform _end;

    // Update is called once per frame
    void Update()
    {
        //始点と終点の間にpivotを移動させる
        Vector3 pivotPos = (_end.position + _start.position) / 2;
        transform.position = pivotPos;
        //向きを合わせる
        Vector3 dir = _end.position - pivotPos;
        transform.forward = dir;
        //コライダーをstartとendの長さに伸ばす
        BoxCollider col = GetComponent<BoxCollider>();
        float distance = Vector3.Distance(_start.position, _end.position);
        col.size = new Vector3(col.size.x, col.size.y, distance);
    }
}

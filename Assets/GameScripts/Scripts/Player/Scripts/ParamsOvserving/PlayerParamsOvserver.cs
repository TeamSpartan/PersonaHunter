using System;
using System.Collections;
using System.Collections.Generic;
using Player.Param;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary> プレイヤ の パラメータの監視をする </summary>
[RequireComponent(typeof(PlayerParam))]
public class PlayerParamsOvserver : MonoBehaviour
{
    private PlayerParam _params; // プレイヤ の パラメータ

    private void Start()
    {
        _params = GetComponent<PlayerParam>();
    }

    private void OnDisable()
    {
        TaskOnSceneChanged();
    }

    public void TaskOnSceneChanged()
    {
        _params.SetIsAnimation(false);
        _params.SetIsJustAvoid(false);
        _params.SetIsAttack(false);
        _params.SetIsAvoid(false);
        _params.SetIsAttack(false);
        if (TryGetComponent<Rigidbody>(out var c))
        {
            c.velocity = Vector3.zero;
        }
    }

    // ダメージくらったモーション再生終了したときのアニメーションイベント
    public void CallbackOnPlayedDamagedMotion()
    {
        _params.SetIsJustAvoid(false);
        _params.SetIsAttack(false);
        _params.SetIsAvoid(false);
        _params.SetIsParry(false);
        _params.SetIsRun(false);
    }
}

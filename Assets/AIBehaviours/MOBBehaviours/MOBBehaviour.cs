using System;
using System.Collections.Generic;
using AIBehaviours.MOBBehaviours.States;
using UnityEngine;
using UnityEngine.AI;
using SgLibUnite.AI;
using SgLibUnite.StateSequencer;

[RequireComponent(typeof(NavMeshAgent))]
// 作成 ： 菅沼
/// <summary>
/// オモテガリ MOB AI
/// </summary>
public class MobBehaviourParameter
    : MonoBehaviour
        , IEnemyBehaviourParameter
        , IInitializableComponent
        , IDulledTarget
{
    #region Component Parameter

    [SerializeField, Header("The Patrolling Path To Patrol")]
    private PatrollerPathContainer PathContainer;

    [SerializeField, Header("The Health MOB Has")]
    private float Health;

    [SerializeField, Header("THe Flinch Value MOB Has")]
    private float Flinch;

    [SerializeField, Header("The Tag Of Player")]
    private string PlayerTag;

    #endregion

    #region The Default Interface To Get Parameter

    public float GetHealth()
    {
        return this.Health;
    }

    public void SetHealth(float val)
    {
        this.Health = val;
    }

    public float GetFlinchValue()
    {
        return this.Flinch;
    }

    public void SetFlinchValue(float val)
    {
        this.Flinch = val;
    }

    #endregion

    #region States

    private MobStateIdle _stateIdle;
    private MobStatePatrol _statePatrol;
    private MobStateTrack _stateTrack;
    private MobStateAttack _stateAttack;
    private MobStateDeath _stateDeath;

    #endregion

    #region Condition

    /// <summary>
    /// アイドルからパトロール
    /// </summary>
    private bool _initialized;

    /// <summary>
    /// プレイヤーを発見したら
    /// </summary>
    private bool _foundPlayer;

    /// <summary>
    /// 攻撃範囲内に入ったら
    /// </summary>
    private bool _playerIsInAttackRange;

    /// <summary>
    /// ＨＰを削られきったら
    /// </summary>
    private bool _death;

    #endregion

    private StateSequencer _sequencer;
    private Animator _animator;

    public void InitializeThisComponent()
    {
        // ステートマシン
        _sequencer = new();

        // Try Get Animator
        if (this.gameObject.GetComponent<Animator>() != null)
        {
            _animator = GetComponent<Animator>();
        } // if animator is attached on root object
        else if (this.gameObject.GetComponentInChildren<Animator>() != null)
        {
            _animator = GetComponentInChildren<Animator>();
        } // if animator is attached on child object

        _stateIdle = new MobStateIdle();
        _statePatrol = new MobStatePatrol();
        _stateTrack = new MobStateTrack();
        _stateAttack = new MobStateAttack();
        _stateDeath = new MobStateDeath();

        // ステートを追加
        var states =
            new List<ISequensableState>()
                { _stateIdle, _statePatrol, _stateTrack, _stateAttack, _stateDeath };
        _sequencer.ResistStates(states);
        
        // 

        // 起動
        _sequencer.PopStateMachine();
    }

    private void FixedUpdate()
    {
    }

    public void FinalizeThisComponent()
    {
    }

    public void StartDull()
    {
        throw new NotImplementedException();
    }

    public void EndDull()
    {
        throw new NotImplementedException();
    }
}
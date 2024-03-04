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

    private string _tnInit = "Initialize";
    private string _tnInitBack = "Initialize_Back";

    /// <summary>
    /// アイドルからパトロール
    /// </summary>
    [SerializeField] private bool _initialized;

    private string _tnPlayerFound = "PlayerFound";
    private string _tnPlayerFoundBack = "PlayerFound_Back";

    /// <summary>
    /// プレイヤーを発見したら
    /// </summary>
    [SerializeField] private bool _foundPlayer;

    private string _tnIsInAttackingRange = "IsInAttackingRange";
    private string _tnIsInAttackingRangeBack = "IsInAttackingRange_Back";

    /// <summary>
    /// 攻撃範囲内に入ったら
    /// </summary>
    [SerializeField] private bool _playerIsInAttackRange;

    private string _tnDeath = "Death";

    /// <summary>
    /// ＨＰを削られきったら
    /// </summary>
    [SerializeField] private bool _death;

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
                { _stateIdle, _statePatrol, _stateTrack, _stateAttack };
        _sequencer.ResistStates(states);
        _sequencer.ResistStateFromAny(_stateDeath);

        // 遷移の登録
        _sequencer.MakeTransition(_stateIdle, _statePatrol, _tnInit);
        _sequencer.MakeTransition(_statePatrol, _stateIdle, _tnInitBack);

        _sequencer.MakeTransition(_statePatrol, _stateTrack, _tnPlayerFound);
        _sequencer.MakeTransition(_stateTrack, _statePatrol, _tnPlayerFoundBack);

        _sequencer.MakeTransition(_stateTrack, _stateAttack, _tnIsInAttackingRange);
        _sequencer.MakeTransition(_stateAttack, _stateTrack, _tnIsInAttackingRangeBack);

        _sequencer.MakeTransitionFromAny(_stateDeath, _tnDeath);

        // 起動
        _sequencer.PopStateMachine();
    }

    void UpdateTransitions()
    {
        _sequencer.UpdateTransition(_tnInit, ref _initialized);
        _sequencer.UpdateTransition(_tnInitBack, ref _initialized, false);

        _sequencer.UpdateTransition(_tnPlayerFound, ref _foundPlayer);
        _sequencer.UpdateTransition(_tnPlayerFoundBack, ref _foundPlayer, false);

        _sequencer.UpdateTransition(_tnIsInAttackingRange, ref _playerIsInAttackRange);
        _sequencer.UpdateTransition(_tnIsInAttackingRangeBack, ref _playerIsInAttackRange, false);

        _sequencer.UpdateTransitionFromAnyState(_tnDeath, ref _death, true, true);
    }

    private void FixedUpdate()
    {
        UpdateTransitions();
    }

    public void FinalizeThisComponent()
    {
        _sequencer.PushStateMachine();
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
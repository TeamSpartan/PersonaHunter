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
public class MobBehaviour
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

    [SerializeField, Header("The Range Of Sight")]
    private float SightRange;
    
    [SerializeField, Header("The Range Of Attacking")]
    private float AttackingRange;

    [SerializeField, Header("The LayerMask Of Player")]
    private LayerMask PlayerLayerMask;
    
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
    private MobStateFlinch _stateFlinch;
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

    private string _tnFlinch = "Flinch";

    /// <summary>
    /// ダウン値がたまり切ったら
    /// </summary>
    [SerializeField] private bool _flinch;

    private string _tnBackToIdleAnyWhere = "BackToIdle";
    
    /// <summary>
    /// Anyステートからの強制的なアイドルへの遷移フラグ
    /// </summary>
    [SerializeField] private bool _backToIdle;

    #endregion
    
    private StateSequencer _sequencer;
    private Animator _animator;
    private Transform _playerTransform;

    public void StartUp()
    {
        _initialized = true;
    }

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

        // ステートをインスタンス化
        _stateIdle = new MobStateIdle();
        _statePatrol = new MobStatePatrol();
        _stateTrack = new MobStateTrack();
        _stateAttack = new MobStateAttack();
        _stateFlinch = new MobStateFlinch();
        _stateDeath = new MobStateDeath();

        // ステートを追加
        var states =
            new List<ISequensableState>()
                { _stateIdle, _statePatrol, _stateTrack, _stateAttack };
        _sequencer.ResistStates(states);
        _sequencer.ResistStateFromAny(_stateDeath);
        _sequencer.ResistStateFromAny(_stateFlinch);

        // 遷移の登録
        _sequencer.MakeTransition(_stateIdle, _statePatrol, _tnInit);
        _sequencer.MakeTransition(_statePatrol, _stateIdle, _tnInitBack);

        _sequencer.MakeTransition(_statePatrol, _stateTrack, _tnPlayerFound);
        _sequencer.MakeTransition(_stateTrack, _statePatrol, _tnPlayerFoundBack);

        _sequencer.MakeTransition(_stateTrack, _stateAttack, _tnIsInAttackingRange);
        _sequencer.MakeTransition(_stateAttack, _stateTrack, _tnIsInAttackingRangeBack);
    
        // Anyからの遷移
        _sequencer.MakeTransitionFromAny(_stateDeath, _tnDeath);
        _sequencer.MakeTransitionFromAny(_stateFlinch, _tnFlinch);
        
        // Anyステートからのアイドルステートへの強制的な遷移
        _sequencer.MakeTransitionFromAny(_stateIdle, _tnBackToIdleAnyWhere);

        // 起動
        _sequencer.PopStateMachine();
    }

    void UpdateConditions()
    {
        // 各コンディションの更新
            // プレイヤ視認フラグ
        _foundPlayer = Physics.CheckSphere(this.transform.position, SightRange, PlayerLayerMask);
            // 攻撃可能判定フラグ
        _playerIsInAttackRange = Physics.CheckSphere(this.transform.position, AttackingRange, PlayerLayerMask);
        // プレイヤー（目標）のトランスフォーム
        _playerTransform = GameObject.FindWithTag(PlayerTag).transform;
    }

    void UpdateTransitions()
    {
        // 各コンディション更新
        UpdateConditions();
        
        // 各遷移を更新
        _sequencer.UpdateTransition(_tnInit, ref _initialized);
        _sequencer.UpdateTransition(_tnInitBack, ref _initialized, false);

        _sequencer.UpdateTransition(_tnPlayerFound, ref _foundPlayer);
        _sequencer.UpdateTransition(_tnPlayerFoundBack, ref _foundPlayer, false);

        _sequencer.UpdateTransition(_tnIsInAttackingRange, ref _playerIsInAttackRange);
        _sequencer.UpdateTransition(_tnIsInAttackingRangeBack, ref _playerIsInAttackRange, false);

        // Anyステートからの遷移の更新
        _sequencer.UpdateTransitionFromAnyState(_tnDeath, ref _death, true, true);
        _sequencer.UpdateTransitionFromAnyState(_tnFlinch, ref _flinch, true, true);
        
        // Anyステートからのアイドルステートへの強制的な遷移 の更新
        _sequencer.UpdateTransitionFromAnyState(_tnBackToIdleAnyWhere, ref _backToIdle, true, true);
        
        // 各ステートを更新
        _stateIdle.UpdateState(this.transform, _playerTransform);
        _statePatrol.UpdateState(this.transform, _playerTransform);
        _stateTrack.UpdateState(this.transform, _playerTransform);
        _stateAttack.UpdateState(this.transform, _playerTransform);
        // Anyからの遷移のステート
        _stateDeath.UpdateState(this.transform, _playerTransform);
        _stateFlinch.UpdateState(this.transform, _playerTransform);
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

    private void OnDrawGizmos()
    {
        // 視野の範囲
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(this.transform.position, SightRange);
        
        // 攻撃圏内
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, AttackingRange);
    }
}
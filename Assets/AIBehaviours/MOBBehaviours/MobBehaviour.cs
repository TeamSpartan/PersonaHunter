﻿using System.Collections;
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
        , IDamagedComponent
{
    #region Component Parameter Exposing

    [SerializeField, Header("The Patrolling Path To Patrol")]
    private PatrollerPathContainer PathContainer;

    [SerializeField, Header("The Health MOB Has")]
    private float Health;

    [SerializeField, Header("The Flinch Threshold Value MOB Has")]
    private float FlinchThreshold;

    [SerializeField, Header("The Flinching Time")]
    private float FlinchingTime = 0f;

    [SerializeField, Header("The Tag Of Player")]
    private string PlayerTag;

    [SerializeField, Header("The Range Of Sight")]
    private float SightRange;

    [SerializeField, Header("The Range Of Attacking")]
    private float AttackingRange;

    [SerializeField, Header("The Damage This Component Will Apply Every Damage Process")]
    private float Damage;

    [SerializeField, Header("The Interval Per Attack")]
    private float IntervalAttacking;

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
        return this._flinchValue;
    }

    public void SetFlinchValue(float val)
    {
        this._flinchValue = val;
    }

    #endregion

    #region States

    private MobStateIdle _stateIdle;
    private MobStatePatrol _statePatrol;
    private MobStateTrack _stateTrack;
    private MobStateAttack _stateAttack;
    private MobStateFlinch _stateFlinch;
    private MobStateDeath _stateDeath;
    private MobStateDamaged _stateDamaged;
    
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


    /// <summary>
    /// プレイヤーからダメージを受けた時のフラグ
    /// </summary>
    [SerializeField] private bool _damaged;

    private string _tnDamaged = "DamagedFromPlayer";
    
    /// <summary>
    /// Anyステートからの強制的なアイドルへの遷移フラグ
    /// </summary>
    [SerializeField] private bool _backToIdle;
    
    private string _tnBackToIdleAnyWhere = "BackToIdle";

    #endregion

    private StateSequencer _sequencer;
    private Animator _animator;
    private Transform _playerTransform;
    private NavMeshAgent _agent;

    /// <summary>
    /// 内部パラメータ ひるみ値
    /// </summary>
    [SerializeField] // デバッグ用属性
    private float _flinchValue;

    /// <summary>
    /// このAIを起動する
    /// </summary>
    public void StartUp()
    {
        _initialized = true;
    }

    IEnumerator BackToIdleRoutine(float interval)
    {
        yield return new WaitForSeconds(interval);
        _backToIdle = true;
    }

    public void InitializeThisComponent()
    {
        // ステートマシン
        _sequencer = new();

        // Navigation Mesh
        _agent = this.gameObject.GetComponent<NavMeshAgent>();

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
        _statePatrol = new MobStatePatrol(PathContainer);
        _stateTrack = new MobStateTrack(AttackingRange);
        _stateAttack = new MobStateAttack(AttackingRange, Damage, IntervalAttacking, PlayerLayerMask
            , () =>
        {
            _backToIdle = true;
            _playerIsInAttackRange = false;
        });
        _stateDamaged = new MobStateDamaged();
        _stateFlinch = new MobStateFlinch(FlinchingTime
            ,() =>
        {
            _backToIdle = true;
            _flinchValue = 0f;
            Debug.Log("Flinch End");
        });
        _stateDeath = new MobStateDeath(1.5f
            , () =>
        {
            Destroy(this.GetComponent<MobBehaviour>());
        });

        // ステートを追加
        var states =
            new List<ISequensableState>()
                { _stateIdle, _statePatrol, _stateTrack };
        _sequencer.ResistStates(states);
        _sequencer.ResistStateFromAny(_stateDeath);
        _sequencer.ResistStateFromAny(_stateFlinch);
        _sequencer.ResistStateFromAny(_stateAttack);
        _sequencer.ResistStateFromAny(_stateDamaged);

        // 遷移の登録
        // アイドルからパトロール
        _sequencer.MakeTransition(_stateIdle, _statePatrol, _tnInit);
        _sequencer.MakeTransition(_statePatrol, _stateIdle, _tnInitBack);

        // パトロールから追跡
        _sequencer.MakeTransition(_statePatrol, _stateTrack, _tnPlayerFound);
        _sequencer.MakeTransition(_stateTrack, _statePatrol, _tnPlayerFoundBack);

        // Anyからの遷移
        _sequencer.MakeTransitionFromAny(_stateDeath, _tnDeath);
        _sequencer.MakeTransitionFromAny(_stateFlinch, _tnFlinch);
        _sequencer.MakeTransitionFromAny(_stateDamaged, _tnDamaged);
        _sequencer.MakeTransitionFromAny(_stateAttack, _tnIsInAttackingRange);

        // Anyステートからのアイドルステートへの強制的な遷移
        _sequencer.MakeTransitionFromAny(_stateIdle, _tnBackToIdleAnyWhere);

        // 内部パラメータの初期化
        _flinchValue = 0;

        // 起動
        _sequencer.PopStateMachine();

        // 各アニメーションレイヤのweightを初期化
        // レイヤ添え字 = 1 → 鈍化 アニメーションレイヤ
        _animator.SetLayerWeight(0, 1f);
        _animator.SetLayerWeight(1, 0f);
    }

    void UpdateConditions()
    {
        // 各コンディションの更新
        // プレイヤ視認フラグ
        _foundPlayer = Physics.CheckSphere(this.transform.position, SightRange, PlayerLayerMask) && !_flinch && !_death;
        
        // 攻撃可能判定フラグ
        _playerIsInAttackRange = Physics.CheckSphere(this.transform.position, AttackingRange, PlayerLayerMask) && !_flinch && !_death;

        // プレイヤー（目標）のトランスフォーム
        _playerTransform = GameObject.FindWithTag(PlayerTag).transform;

        // HPを削り切ったら
        _death = Health <= 0;

        // ダウン値がたまり切ったら
        _flinch = _flinchValue >= FlinchThreshold;

        // animator-controller のパラメータを初期化
        _animator.SetBool("Initialized", _initialized);
    }

    void UpdateTransitions()
    {
        // 各コンディション更新
        UpdateConditions();
        
        // 各ステートを更新
        _stateIdle.UpdateState(this.transform, _playerTransform, this._agent);
        _statePatrol.UpdateState(this.transform, _playerTransform, this._agent);
        _stateTrack.UpdateState(this.transform, _playerTransform, this._agent);

        // Anyからの遷移のステート
        _stateDeath.UpdateState(this.transform, _playerTransform, this._agent);
        _stateFlinch.UpdateState(this.transform, _playerTransform, this._agent);
        _stateAttack.UpdateState(this.transform, _playerTransform, this._agent);
        _stateDamaged.UpdateState(this.transform, _playerTransform, this._agent);

        // 各遷移を更新
        _sequencer.UpdateTransition(_tnInit, ref _initialized);
        _sequencer.UpdateTransition(_tnInitBack, ref _initialized, false);

        _sequencer.UpdateTransition(_tnPlayerFound, ref _foundPlayer);
        _sequencer.UpdateTransition(_tnPlayerFoundBack, ref _foundPlayer, false);

        // Anyステートからの遷移の更新
        _sequencer.UpdateTransitionFromAnyState(_tnDeath, ref _death);
        _sequencer.UpdateTransitionFromAnyState(_tnFlinch, ref _flinch);
        _sequencer.UpdateTransitionFromAnyState(_tnIsInAttackingRange, ref _playerIsInAttackRange);
        _sequencer.UpdateTransitionFromAnyState(_tnDamaged, ref _damaged, true, true);

        // Anyステートからのアイドルステートへの強制的な遷移 の更新
        _sequencer.UpdateTransitionFromAnyState(_tnBackToIdleAnyWhere, ref _backToIdle, true, true);
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
        // レイヤ添え字 ＝ １ ＝＞ 鈍化 アニメーションレイヤ
        _animator.SetLayerWeight(0, 0f);
        _animator.SetLayerWeight(1, 1f);
    }

    public void EndDull()
    {
        // レイヤ添え字 ＝ １ ＝＞ 鈍化 アニメーションレイヤ
        _animator.SetLayerWeight(0, 1f);
        _animator.SetLayerWeight(1, 0f);
    }

    public void AddDamage(float dmg)
    {
        this.Health -= dmg;
        _damaged = true;
    }

    public void Kill()
    {
        this.Health = 0f;
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
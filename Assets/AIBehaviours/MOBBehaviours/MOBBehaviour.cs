using System;
using System.Collections.Generic;
using UnityEngine;
using SgLibUnite.StateSequencer;
using UnityEngine.AI;
using MOBState;
using SgLibUnite.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
// 作成 ： 菅沼
/// <summary>
/// オモテガリ MOB AI
/// </summary>
public class MOBBehaviour
    : EnemyBehaviour
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

    public override float GetHealth()
    {
        return Health;
    }

    public override void SetHealth(float val)
    {
        this.Health = val;
    }

    public override float GetFlinchValue()
    {
        return Flinch;
    }

    public override void SetFlinchValue(float val)
    {
        this.Flinch = val;
    }

    #endregion

    private StateSequencer _sSeq;
    private NavMeshAgent _agent;
    private Animator _anim;
    private Transform _playerTransform;

    #region States

    private MOBStateIdle _idle;
    private MOBStatePatrol _patrol;
    private MOBStateTrack _track;
    private MOBStateAttack _attack;
    private MOBStateDeath _death;

    #endregion

    #region TransitionNames

    /// <summary>
    /// パトロールから追跡
    /// </summary>
    private string _gonnaTrack = "StartingTrack";
    private bool _cGonnaTrack = false;
    
    #endregion

    void InitStates()
    {
        // パトロールステート
        this._patrol = new MOBStatePatrol(_agent
            , PathContainer
            , this.gameObject.transform
            , _playerTransform);
    }

    void SetUpTransitions()
    {
        List<ISequensableState> states = new List<ISequensableState>()
            { _patrol, _track, _attack, _death };
        _sSeq.ResistStates(states);
        
    }

    void UpdateEachState()  // 各ステートの更新 
    {
        _patrol.TaskOnUpdate(this.gameObject.transform, _playerTransform);
    }
    
    void UpdateTransitions() // 各遷移の更新
    {
    }
    
    public void InitializeThisComponent()
    {

        _sSeq = new StateSequencer();
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _playerTransform = GameObject.FindWithTag(PlayerTag).transform;
        
        InitStates();
        SetUpTransitions();
    }
    
    private void FixedUpdate()
    {
        UpdateTransitions();
        UpdateEachState();
    }

    public void FinalizeThisComp()
    {
        _agent.ResetPath();
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

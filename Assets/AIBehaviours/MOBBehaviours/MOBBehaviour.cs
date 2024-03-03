using System;
using System.Collections.Generic;
using UnityEngine;
using SgLibUnite.StateSequencer;
using UnityEngine.AI;
using MOBState;
using SgLibUnite.AI;

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
    private Transform _playerTransform;

    #region States

    private MOBStateDefault _stateDefault;
    private MOBStateTrack _stateTrack;
    private MOBStateAttack _stateAttack;
    private MOBStateDeath _stateDeath;

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
        this._stateDefault = new MOBStateDefault(_agent
            , PathContainer
            , this.gameObject.transform
            , _playerTransform);
    }

    void SetUpTransitions()
    {
        List<ISequensableState> states = new List<ISequensableState>()
            { _stateDefault, _stateTrack, _stateAttack, _stateDeath };
        _sSeq.ResistStates(states);
        
        _sSeq.MakeTransition(_stateDefault, _stateTrack, _gonnaTrack);
    }

    void UpdateEachState()
    {
        _stateDefault.TaskOnUpdate(this.gameObject.transform, _playerTransform);
    }

    void TickStates()
    {
        
    }
    
    public void InitializeThisComp()
    {
        InitStates();

        _sSeq = new StateSequencer();
        
        SetUpTransitions();
    }
    
    private void FixedUpdate()
    {
        TickStates();
        UpdateEachState();
        
        // 遷移を更新
        _sSeq.UpdateTransition(_gonnaTrack, ref _cGonnaTrack);
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

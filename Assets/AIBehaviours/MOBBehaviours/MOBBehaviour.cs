using System;
using UnityEngine;
using SgLibUnite.StateSequencer;
using UnityEngine.AI;
using MOBState;

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

    #region States

    private MOBStateDefault _stateDefault;
    private MOBStateTrack _stateTrack;
    private MOBStateAttack _stateAttack;
    private MOBStateDeath _stateDeath;

    #endregion
    
    public void InitializeThisComp()
    {
        throw new NotImplementedException();
    }

    public void FinalizeThisComp()
    {
        throw new NotImplementedException();
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

using System;
using UnityEngine;
using UnityEngine.AI;
using SgLibUnite.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
// 作成 ： 菅沼
/// <summary>
/// オモテガリ MOB AI
/// </summary>
public class MOBBehaviour
    : MonoBehaviour
        , IEnemyBehaviour
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

    public void InitializeThisComponent()
    {
        Debug.Log("Init");
    }

    public void FinalizeThisComp()
    {
        Debug.Log("Fin");
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
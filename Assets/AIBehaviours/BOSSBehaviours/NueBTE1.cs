using System;
using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

// 作成 菅沼
[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 鵺 改良１型 </summary>
public class NueBTE1
    : MonoBehaviour
        , IMobBehaviourParameter
        , IInitializableComponent
        , IDulledTarget
        , IDamagedComponent
{
    #region Parameter Exposing

    [SerializeField, Header("Health")] private float _health;

    [SerializeField, Header("Flinch Threshold")]
    private float _flinchThreshold;

    [SerializeField, Header("Awaiting Time When Get Parry[sec]")]
    private float _awaitOnStumble;

    [SerializeField, Header("Awaiting Time When Get Flinch[sec]")]
    private float _awaitOnFlinching;

    [SerializeField, Range(1f, 100f), Header("Sight Range")]
    private float _sightRange;

    [Header("Attacking Range")] [SerializeField, Range(1f, 100f)]
    private float _clawAttackRange;

    [SerializeField, Range(5f, 100f)] private float _taleAttackRange;
    [SerializeField, Range(10f, 100f)] private float _rushAttackRange;

    [SerializeField, Header("Player LayerMask")]
    private LayerMask _playerLayers;

    [SerializeField, Header("Player Tag")] private string _playerTag;

    #endregion

    #region Behaviours

    private BTBehaviour _btbGetClose;
    private BTBehaviour _btbAwait;
    private BTBehaviour _btbThinkForNextBehaviour;
    private BTBehaviour _btbAwayFromPlayer;
    private BTBehaviour _btbFlinch;
    private BTBehaviour _btbDeath;
    private BTBehaviour _btbStumble;

    #endregion

    #region Conditions

    #endregion

    #region Transition Name

    #endregion

    #region Timers

    #endregion

    public float GetHealth()
    {
        return _health;
    }

    public void SetHealth(float val)
    {
        _health = val;
    }

    public void InitializeThisComponent()
    {
    }

    public void FixedUpdate()
    {
    }

    public void FinalizeThisComponent()
    {
    }

    public void StartDull()
    {
    }

    public void EndDull()
    {
    }

    public void AddDamage(float dmg)
    {
        _health -= dmg;
    }

    public void Kill()
    {
        _health = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _clawAttackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _taleAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _rushAttackRange);
    }
}

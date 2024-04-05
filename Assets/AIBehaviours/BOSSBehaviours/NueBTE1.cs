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
    private BTBehaviour _btbClaw;
    private BTBehaviour _btbTale;
    private BTBehaviour _btbRush;

    #endregion

    #region Conditions

    private bool _btcPlayerFound;
    private bool _btcPlayerIsInARange;

    #endregion

    #region Transition Name

    private string _bttStartThink = "unnko";

    #endregion

    #region Timers

    private float _tFlinchET;
    private float _tStumbleET;
    private float _tAwaitET;

    #endregion

    #region Parameter Inside(Hidden)

    private BehaviourTree _bt = new BehaviourTree();
    private float _flinchVal;

    #endregion

    #region Each Behaviour Structs Fuctions

    private void ThinkNextBehaviour() // think next behaviour
    {
    }

    private void GetClose() // get close to player
    {
    }


    private void Await() // await and do nothing 
    {
    }

    private void Away() // get distance from player
    {
    }

    private void Flinch() // when get flinch. On Got Many Attacks
    {
    }

    private void Death() // when health has been 0 or less
    {
    }

    private void Stumble() // when get parry
    {
    }

    private void Claw() // claw atk
    {
    }

    private void Tale() // tale atk
    {
    }

    private void Rush() // rusing atk
    {
    }

    #endregion

    #region Setups

    private void SetUpBehaviours()
    {
        // instantiate behviours
        _btbAwait = new();
        _btbClaw = new();
        _btbDeath = new();
        _btbFlinch = new();
        _btbRush = new();
        _btbStumble = new();
        _btbTale = new();
        _btbGetClose = new();
        _btbAwayFromPlayer = new();
        _btbThinkForNextBehaviour = new();

        _btbAwait.AddBehaviour(Await);
        _btbClaw.AddBehaviour(Claw);
        _btbDeath.AddBehaviour(Death);
        _btbFlinch.AddBehaviour(Flinch);
        _btbRush.AddBehaviour(Rush);
        _btbStumble.AddBehaviour(Stumble);
        _btbTale.AddBehaviour(Tale);
        _btbGetClose.AddBehaviour(GetClose);
        _btbAwayFromPlayer.AddBehaviour(Away);
        _btbThinkForNextBehaviour.AddBehaviour(ThinkNextBehaviour);
    }

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

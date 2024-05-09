using System;
using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

// 作成 菅沼
[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 鵺 改良１型 のはずだった </summary>
public class NueBT_
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

    private BTBehaviour _btbIdle;
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

    [SerializeField] private bool _btcPlayerFound;
    [SerializeField] private bool _btcPlayerIsInARange;

    #endregion

    #region Transition Name

    private string _bttStartThink = "01";
    private string _bttStartGetClose = "02";

    #endregion

    #region Timers

    private float _tFlinchET;
    private float _tStumbleET;
    private float _tAwaitET;

    #endregion

    #region Parameter Inside(Hidden)

    private BehaviourTree _bt;
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _flinchVal;

    #endregion

    #region Each Behaviour Structs Fuctions

    private void Idle() // idle stay on point
    {
        Debug.Log($"Idle");
    }

    private void FindPlayer() // find player , and get transform
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    private void ThinkNextBehaviour() // think next behaviour
    {
        Debug.Log($"Think");
    }

    private void GetClose() // get close to player
    {
        Debug.Log($"Get Close");
    }

    private void Await() // await and do nothing 
    {
        Debug.Log($"Await");
    }

    private void Away() // get distance from player
    {
        Debug.Log($"Away");
    }

    private void Flinch() // when get flinch. On Got Many Attacks
    {
        Debug.Log($"Flinch");
    }

    private void Death() // when health has been 0 or less
    {
        Debug.Log($"Death");
    }

    private void Stumble() // when get parry
    {
        Debug.Log($"Stumble");
    }

    private void Claw() // claw atk
    {
        Debug.Log($"Claw");
    }

    private void Tale() // tale atk
    {
        Debug.Log($"Tale");
    }

    private void Rush() // rushing atk
    {
        Debug.Log($"Rush");
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
        _bt = new BehaviourTree();
        
        // instantiate behviours
        _btbAwait = new();
        _btbClaw = new();
        _btbDeath = new();
        _btbFlinch = new();
        _btbIdle = new();
        _btbRush = new();
        _btbStumble = new();
        _btbTale = new();
        _btbGetClose = new();
        _btbAwayFromPlayer = new();
        _btbThinkForNextBehaviour = new();

        // setup each
        _btbIdle.AddBehaviour(Idle);
        _btbGetClose.AddBehaviour(GetClose);
        _btbThinkForNextBehaviour.AddBehaviour(ThinkNextBehaviour);

        _btbAwait.AddBehaviour(Await);
        _btbAwait.SetYieldMode(true);

        _btbClaw.AddBehaviour(Claw);
        _btbClaw.SetYieldMode(true);

        _btbDeath.AddBehaviour(Death);
        _btbDeath.SetYieldMode(true);

        _btbFlinch.AddBehaviour(Flinch);
        _btbFlinch.SetYieldMode(true);

        _btbRush.AddBehaviour(Rush);
        _btbRush.SetYieldMode(true);

        _btbStumble.AddBehaviour(Stumble);
        _btbStumble.SetYieldMode(true);

        _btbTale.AddBehaviour(Tale);
        _btbTale.SetYieldMode(true);

        _btbAwayFromPlayer.AddBehaviour(Away);
        _btbAwayFromPlayer.SetYieldMode(true);
        
        _bt.ResistBehaviours(new[]
        {
            _btbAwait, _btbClaw, _btbDeath, _btbFlinch, _btbIdle, _btbRush, _btbStumble, _btbThinkForNextBehaviour,
            _btbStumble,
            _btbTale, _btbGetClose, _btbAwayFromPlayer
        });
        
        _bt.MakeTransition(_btbIdle, _btbGetClose, _bttStartGetClose);
        _bt.MakeTransition(_btbGetClose, _btbThinkForNextBehaviour, _bttStartThink);

        FindPlayer();
        if (_bt.IsPaused)
        {
            _bt.StartBT();
        }

        if (_bt.CurrentBehaviour != _btbIdle)
        {
            _bt.JumpTo(_btbIdle);
        }
        
        _agent = GetComponent<NavMeshAgent>();
        if (GetComponent<Animator>() != null)
        {
            _animator = GetComponent<Animator>();
        }
        else if (GetComponentInChildren<Animator>() != null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
    }

    public void FixedThickThisComponent()
    {
        throw new NotImplementedException();
    }

    public void ThickThisComponent()
    {
        throw new NotImplementedException();
    }

    public void FixedUpdate()
    {
        FindPlayer();
        
        _bt.UpdateEventsYield();

        _btcPlayerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayers);
        _btcPlayerIsInARange = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);
        _bt.UpdateTransition(_bttStartGetClose, ref _btcPlayerFound);
        _bt.UpdateTransition(_bttStartThink, ref _btcPlayerIsInARange);
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

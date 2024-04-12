using System;
using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

public class Nue_v1_e2 : MonoBehaviour
    , IMobBehaviourParameter
    , IInitializableComponent
    , IDulledTarget
    , IDamagedComponent
{
    #region Parameter Exposed

    [SerializeField, Header("Health")] private float _healthMaxValue;

    [SerializeField, Header("Flinch Threshold")]
    private float _flinchThreshold;

    [SerializeField, Header("Awaiting Time[sec]")]
    private float _awaitingTime;

    [SerializeField, Header("Awaiting Time When Get Parry[sec]")]
    private float _awaitTimeOnStumble;

    [SerializeField, Header("Awaiting Time When Get Flinch[sec]")]
    private float _awaitTimeOnFlinching;

    [SerializeField, Range(1f, 100f), Header("Sight Range")]
    private float _sightRange;

    [Header("Attacking Range")] [SerializeField, Range(1f, 100f)]
    private float _clawAttackRange;

    [SerializeField, Range(5f, 100f)] private float _taleAttackRange;
    [SerializeField, Range(10f, 100f)] private float _rushAttackRange;

    [SerializeField, Header("Player LayerMask")]
    private LayerMask _playerLayers;

    [SerializeField, Header("Player Tag")] private string _playerTag;

    [SerializeField, Range(1f, 50f)] private float _baseMoveSpeed;
    [SerializeField, Range(1f, 50f)] private float _moveSpeedOnMad;

    #endregion

    #region Behaviours

    private BTBehaviour _btbIdle = new BTBehaviour();
    private BTBehaviour _btbGetClose = new BTBehaviour();
    private BTBehaviour _btbAwait = new BTBehaviour();
    private BTBehaviour _btbThink = new BTBehaviour();
    private BTBehaviour _btbAway = new BTBehaviour();
    private BTBehaviour _btbFlinch = new BTBehaviour();
    private BTBehaviour _btbDeath = new BTBehaviour();
    private BTBehaviour _btbStumble = new BTBehaviour();
    private BTBehaviour _btbClaw = new BTBehaviour();
    private BTBehaviour _btbTale = new BTBehaviour();
    private BTBehaviour _btbRush = new BTBehaviour();

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
    private float _thinkET; // 思考時 の 待機時間。 怒り状態で待機時間が減るのでこれが必要

    #endregion

    #region Parameter Inside(Hidden)

    private BehaviourTree _bt = new BehaviourTree();
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _flinchVal;
    [SerializeField] private float _health;
    private bool _isGettingMad;
    [SerializeField] private int _attackCount;

    // メソッドへの一度のみのエントリーを制限したいときのフラグ
    [SerializeField] private bool _clawEntryLocked;
    [SerializeField] private bool _taleEntryLocked;
    [SerializeField] private bool _rushEntryLocked;

    #endregion

    #region Functions

    void Idle()
    {
    }

    void GetClose()
    {
    }

    void Think()
    {
    }

    void Await()
    {
    }

    void Away()
    {
    }

    void Claw()
    {
    }

    void Tale()
    {
    }

    void Rush()
    {
    }

    void Death()
    {
    }

    void Flinch()
    {
    }

    void Stumble()
    {
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
        #region Add Function To Behaviour 1.

        _btbIdle.AddBehaviour(Idle);
        _btbGetClose.AddBehaviour(GetClose);
        _btbThink.AddBehaviour(Think);

        _btbGetClose.EEnd += () => { _agent.ResetPath(); };

        _btbAwait.AddBehaviour(Await);
        _btbAwait.SetYieldMode(true);

        _btbClaw.AddBehaviour(Claw);
        _btbClaw.SetYieldMode(true);

        _btbDeath.AddBehaviour(Death);
        _btbDeath.SetYieldMode(true);

        _btbFlinch.AddBehaviour(Flinch);
        _btbFlinch.SetYieldMode(true);

        _btbStumble.AddBehaviour(Stumble);
        _btbStumble.SetYieldMode(true);

        _btbRush.AddBehaviour(Rush);
        _btbRush.SetYieldMode(true);

        _btbTale.AddBehaviour(Tale);
        _btbTale.SetYieldMode(true);

        _btbAway.AddBehaviour(Away);
        _btbAway.SetYieldMode(true);

        #endregion

        // Resist Behaviours 2.
        _bt.ResistBehaviours(new[]
        {
            _btbAwait, _btbClaw, _btbDeath, _btbFlinch, _btbIdle, _btbGetClose, _btbStumble, _btbTale, _btbRush,
            _btbAway, _btbThink
        });

        #region Make Transition 3.

        _bt.MakeTransition(_btbIdle, _btbGetClose, _bttStartGetClose);
        _bt.MakeTransition(_btbGetClose, _btbThink, _bttStartThink);

        #endregion

        // Start BT 4.
        _bt.StartBT();

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

        _health = _healthMaxValue;
    }

    private void FixedUpdate()
    {
        _bt.UpdateEventsYield();

        _btcPlayerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayers);
        _btcPlayerIsInARange = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);
        _isGettingMad = (_healthMaxValue * .5f) >= _health;

        if (!_btcPlayerFound)
        {
            _bt.JumpTo(_btbIdle);
        }

        if (!_btcPlayerIsInARange && _btcPlayerFound)
        {
            _bt.JumpTo(_btbGetClose);
        }

        _bt.UpdateTransition(_bttStartGetClose, ref _btcPlayerFound);
        _bt.UpdateTransition(_bttStartThink, ref _btcPlayerIsInARange);
    }

    public void FinalizeThisComponent()
    {
        throw new System.NotImplementedException();
    }

    public void StartDull()
    {
        throw new System.NotImplementedException();
    }

    public void EndDull()
    {
        throw new System.NotImplementedException();
    }

    public void AddDamage(float dmg)
    {
        _health -= dmg;
    }

    public void Kill()
    {
        _health = 0;
    }
}

using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// 作成 菅沼
[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 鵺 </summary>
public class NueBT
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

    [FormerlySerializedAs("_awaitOnParry")] [SerializeField, Header("Awaiting Time When Get Parry[sec]")]
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
    private BTBehaviour _btbCheckAttackType;
    private BTBehaviour _btbClaw;
    private BTBehaviour _btbTale;
    private BTBehaviour _btbRush;
    private BTBehaviour _btbDeath;
    private BTBehaviour _btbFlinch;
    private BTBehaviour _btbStumble;

    #endregion

    #region Conditions

    private bool _clawAttackable;
    private bool _taleAttackable;
    private bool _rushable;

    #endregion

    #region Transition Name

    private string _bttIdleToClose = "01";
    private string _bttCloseToCheckAtkType = "02";
    private string _bttStartClaw = "03";
    private string _bttStartTale = "04";
    private string _bttStartRush = "05";

    #endregion

    #region Timers

    private float _flinchingETime;
    private float _stumblingETime;

    #endregion

    private BehaviourTree _bt;
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _flinchValue;

    #region States

    private void Stay()
    {
        if (_agent.destination != transform.position)
        {
            _agent.ResetPath();
            _agent.SetDestination(transform.position);
        }
    }

    private void GetCloseToPlayer()
    {
        if (_agent.destination != _player.position)
        {
            _agent.SetDestination(_player.position);
        }
    }

    private void CheckAttackingType()
    {
        // 範囲内かまず判定
        _clawAttackable = Physics.CheckSphere(transform.position, _clawAttackRange, _playerLayers);
        _taleAttackable = Physics.CheckSphere(transform.position, _taleAttackRange, _playerLayers);
        _rushable = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);

        // 前後判定を条件に上乗せ
        var vt = (_player.position - transform.position).normalized;
        var frontCond = Vector3.Dot(vt, transform.forward) >= 0;

        _clawAttackable = _clawAttackable && frontCond;
        _taleAttackable = _taleAttackable && !frontCond;
    }

    private void Claw()
    {
        _agent.ResetPath();
    }

    private void Tale()
    {
        _agent.ResetPath();
    }

    private void Rush()
    {
        _agent.ResetPath();
    }

    private void Death()
    {
        _agent.ResetPath();
    }

    private void Flinch()
    {
        _flinchingETime += Time.deltaTime;
        if (_flinchingETime > _awaitOnFlinching)
        {
        }
    }

    private void Stumble()
    {
        _stumblingETime += Time.deltaTime;
        if (_stumblingETime > _awaitOnStumble)
        {
        }
    }

    #endregion

    private void SetupBehaviours()
    {
        _btbIdle = new();
        _btbGetClose = new();
        _btbCheckAttackType = new();
        _btbClaw = new();
        _btbTale = new();
        _btbRush = new();
        _btbDeath = new();
        _btbFlinch = new();
        _btbStumble = new();

        _btbIdle.AddBehaviour(Stay);
        _btbGetClose.AddBehaviour(GetCloseToPlayer);
        _btbCheckAttackType.AddBehaviour(CheckAttackingType);
        _btbClaw.AddBehaviour(Claw);
        _btbTale.AddBehaviour(Tale);
        _btbRush.AddBehaviour(Rush);
        _btbDeath.AddBehaviour(Death);
        _btbFlinch.AddBehaviour(Flinch);
        _btbStumble.AddBehaviour(Stumble);
    }

    private void SetupTransition()
    {
        _bt.MakeTransition(_btbIdle, _btbGetClose, _bttIdleToClose);
        _bt.MakeTransition(_btbGetClose, _btbCheckAttackType, _bttCloseToCheckAtkType);
        _bt.MakeTransition(_btbCheckAttackType, _btbClaw, _bttStartClaw);
        _bt.MakeTransition(_btbCheckAttackType, _btbTale, _bttStartTale);
        _bt.MakeTransition(_btbCheckAttackType, _btbRush, _bttStartRush);
    }

    private void SetupBT()
    {
        SetupBehaviours();
        SetupTransition();
        _bt.ResistBehaviours(new[]
        {
            _btbIdle, _btbGetClose, _btbCheckAttackType, _btbClaw, _btbTale, _btbRush, _btbDeath, _btbFlinch,
            _btbStumble
        });
    }

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
        _bt = new();

        _agent = GetComponent<NavMeshAgent>();

        if (GetComponent<Animator>() != null)
        {
            _animator = GetComponent<Animator>();
        }
        else if (GetComponentInChildren<Animator>() != null)
        {
            _animator = GetComponentInChildren<Animator>();
        }
        
        SetupBT();
        _bt.StartBT();
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

    private void FixedUpdate()
    {
        
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

using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

// 作成 菅沼
[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 鵺 </summary>
public class NueBTProto
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
    private BTBehaviour _btbClaw;
    private BTBehaviour _btbTale;
    private BTBehaviour _btbRush;
    private BTBehaviour _btbDeath;
    private BTBehaviour _btbFlinch;
    private BTBehaviour _btbStumble;

    #endregion

    #region Conditions

    [SerializeField] private bool _clawAttackable;
    [SerializeField] private bool _taleAttackable;
    [SerializeField] private bool _rushable;
    [SerializeField] private bool _floundPlayer;

    #endregion

    #region Transition Name

    private string _bttIdleToClose = "01";
    private string _bttStartClaw = "02";
    private string _bttStartTale = "03";
    private string _bttStartRush = "04";

    #endregion

    #region Timers

    private float _flinchingETime;
    private float _stumblingETime;

    #endregion

    private BehaviourTree _bt;
    private NavMeshAgent _agent;
    private Transform _player;
    private Animator _animator;
    private float _flinchValue;

    #region States

    private void Stay()
    {
        Debug.Log($"Stay Here");
        if (_agent.destination != transform.position)
        {
            _agent.ResetPath();
            _agent.SetDestination(transform.position);
        }
    }

    private void GetCloseToPlayer()
    {
        Debug.Log($"Getting Close");
        if (_agent.destination != _player.position)
        {
            _agent.SetDestination(_player.position);
        }
    }

    private void CheckAttackingType()
    {
        Debug.Log($"Finding Attacking Method");

        // 範囲内かまず判定
        _clawAttackable = Physics.CheckSphere(transform.position, _clawAttackRange, _playerLayers);
        _taleAttackable = Physics.CheckSphere(transform.position, _taleAttackRange, _playerLayers);
        _rushable = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);

        // 前後判定を条件に上乗せ
        var vt = (_player.position - transform.position).normalized;
        var frontCond = Vector3.Dot(vt, transform.forward) >= 0;

        _clawAttackable = _clawAttackable && frontCond;
        _taleAttackable = _taleAttackable && (!frontCond);
        _rushable = _rushable && !_clawAttackable;
    }

    private void Claw()
    {
        Debug.Log($"Claw");
        _agent.ResetPath();
        if (!_clawAttackable)
        {
            _bt.JumpTo(_btbGetClose);
        }
    }

    private void Tale()
    {
        Debug.Log($"Tale");
        _agent.ResetPath();
        if (!_taleAttackable)
        {
            _bt.JumpTo(_btbGetClose);
        }
    }

    private void Rush()
    {
        Debug.Log($"Rush");
        if (_agent.destination != _player.position)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            _agent.ResetPath();
            if (!_rushable)
            {
                _bt.JumpTo(_btbGetClose);
            }
        }
    }

    private void Death()
    {
        Debug.Log($"Death");
        Stay();
        
        _bt.EndYieldBehaviourFrom(_btbDeath);
    }

    private void Flinch()
    {
        Debug.Log($"Flinch");
        Stay();

        _flinchingETime += Time.deltaTime;
        if (_flinchingETime > _awaitOnFlinching)
        {
            _flinchValue = _flinchingETime = 0;
        }
    }

    private void Stumble()
    {
        Debug.Log($"Stumble");
        Stay();

        _stumblingETime += Time.deltaTime;
        if (_stumblingETime > _awaitOnStumble)
        {
            _flinchingETime = 0;
        }
    }

    #endregion

    private void SetupBehaviours()
    {
        _btbIdle = new();
        _btbGetClose = new();
        _btbClaw = new();
        _btbTale = new();
        _btbRush = new();
        _btbDeath = new();
        _btbFlinch = new();
        _btbStumble = new();

        _btbIdle.AddBehaviour(Stay);
        _btbGetClose.AddBehaviour(GetCloseToPlayer);
        _btbClaw.AddBehaviour(Claw);
        _btbTale.AddBehaviour(Tale);
        _btbRush.AddBehaviour(Rush);
        _btbDeath.AddBehaviour(Death);
        _btbFlinch.AddBehaviour(Flinch);
        _btbStumble.AddBehaviour(Stumble);

        _btbDeath.SetYieldMode(true);
        _btbFlinch.SetYieldMode(true);
        _btbStumble.SetYieldMode(true);
    }

    private void SetupTransition()
    {
        _bt.MakeTransition(_btbIdle, _btbGetClose, _bttIdleToClose);
        _bt.MakeTransition(_btbGetClose, _btbClaw, _bttStartClaw);
        _bt.MakeTransition(_btbGetClose, _btbTale, _bttStartTale);
        _bt.MakeTransition(_btbGetClose, _btbRush, _bttStartRush);
    }

    private void SetupBT()
    {
        SetupBehaviours();
        SetupTransition();
        _bt.ResistBehaviours(new[]
        {
            _btbIdle, _btbGetClose, _btbClaw, _btbTale, _btbRush, _btbDeath, _btbFlinch,
            _btbStumble
        });
    }

    private void UpdateTransitions()
    {
        _floundPlayer = Physics.CheckSphere(transform.position, _sightRange, _playerLayers);
        CheckAttackingType();
        
        if (!_floundPlayer)
        {
            _bt.JumpTo(_btbIdle);
        }

        _bt.UpdateTransition(_bttIdleToClose, ref _floundPlayer);
        _bt.UpdateTransition(_bttStartClaw, ref _clawAttackable);
        _bt.UpdateTransition(_bttStartTale, ref _taleAttackable);
        _bt.UpdateTransition(_bttStartRush, ref _rushable);
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

    public void FixedThickThisComponent()
    {
        throw new System.NotImplementedException();
    }

    public void ThickThisComponent()
    {
        throw new System.NotImplementedException();
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
        if (_health <= 0)
        {
            _bt.YieldAllBehaviourTo(_btbDeath);
        }
        
        _player = GameObject.FindWithTag("Player").transform;
        _bt.UpdateEventsYield();
        UpdateTransitions();
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

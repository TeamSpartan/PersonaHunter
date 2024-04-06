using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;

// 作成 菅沼
[RequireComponent(typeof(NavMeshAgent))]
/// <summary> オモテガリ 鵺 改良１型 </summary>
public class NueBTV1E1
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

    #endregion

    #region Behaviours

    private BTBehaviour _btbIdle = new BTBehaviour();
    private BTBehaviour _btbGetClose = new BTBehaviour();
    private BTBehaviour _btbAwait = new BTBehaviour();
    private BTBehaviour _btbThinkForNextBehaviour = new BTBehaviour();
    private BTBehaviour _btbAwayFromPlayer = new BTBehaviour();
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

    #endregion

    #region Parameter Inside(Hidden)

    private BehaviourTree _bt = new BehaviourTree();
    private Transform _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private float _flinchVal;

    #endregion

    #region States

    private void FindPlayer()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    private void Idle()
    {
        FindPlayer();
        Debug.Log($"Idle");
        if (_agent.destination != transform.position)
        {
            _agent.SetDestination(transform.position);
        }
    }

    private void GetClose()
    {
        Debug.Log($"Get Close");
        FindPlayer();
        if (!_agent.hasPath)
        {
            _agent.SetDestination(_player.position);
        }
    }

    private void Await()
    {
        Debug.Log($"Await");
        _tAwaitET += Time.deltaTime;
        if (_tAwaitET > _awaitingTime)
        {
            _tAwaitET = 0;
            _bt.JumpTo(_btbThinkForNextBehaviour);
        }
    }

    private void Think()
    {
        Debug.Log($"Think");
        if (_agent.hasPath)
        {
            _agent.ResetPath();
        }
    }

    private void Away()
    {
        Debug.Log($"Away");
    }

    private void Flinch()
    {
        Debug.Log($"Flinch");
        _tFlinchET += Time.deltaTime;
        if (_tFlinchET > _awaitTimeOnFlinching)
        {
            _tFlinchET = 0;
            _bt.JumpTo(_btbThinkForNextBehaviour);
        }
    }

    private void Death()
    {
        Debug.Log($"Death");
    }

    private void Stumble()
    {
        Debug.Log($"Stumble");
        _tStumbleET += Time.deltaTime;
        if (_tStumbleET > _awaitTimeOnStumble)
        {
            _tStumbleET = 0;
            _bt.JumpTo(_btbThinkForNextBehaviour);
        }
    }

    private void Claw()
    {
        Debug.Log($"Claw");
    }

    private void Tale()
    {
        Debug.Log($"Tale");
    }

    private void Rush()
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
        #region Add Function To Behaviour 1.

        _btbIdle.AddBehaviour(Idle);
        _btbGetClose.AddBehaviour(GetClose);
        _btbThinkForNextBehaviour.AddBehaviour(Think);
        
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
        
        _btbAwayFromPlayer.AddBehaviour(Away);
        _btbAwayFromPlayer.SetYieldMode(true);

        #endregion

        // Resist Behaviours 2.
        _bt.ResistBehaviours(new[]
        {
            _btbAwait, _btbClaw, _btbDeath, _btbFlinch, _btbIdle, _btbGetClose, _btbStumble, _btbTale, _btbRush,
            _btbAwayFromPlayer, _btbThinkForNextBehaviour
        });

        #region Make Transition 3.

        _bt.MakeTransition(_btbIdle, _btbGetClose, _bttStartGetClose);
        _bt.MakeTransition(_btbGetClose, _btbThinkForNextBehaviour, _bttStartThink);

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
    }

    private void FixedUpdate()
    {
        _bt.UpdateEventsYield();

        _btcPlayerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayers);
        _btcPlayerIsInARange = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);

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

using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using Debug = UnityEngine.Debug;

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

    [SerializeField, Range(5f, 100f)] private float _tailAttackRange;
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
    private BTBehaviour _btbTail = new BTBehaviour();
    private BTBehaviour _btbRush = new BTBehaviour();
    private BTBehaviour _btbFacePlayer = new BTBehaviour();
    private BTBehaviour _btbCurrentYieldedBehaviour = new BTBehaviour();

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
    [SerializeField] private float _health;
    private bool _isGettingMad;
    private float _distanceBetPlayer;
    private Vector3 _pDirection;

    // メソッドへの一度のみのエントリーを制限したいときのフラグ
    [SerializeField] private bool _clawEntryLocked;
    [SerializeField] private bool _tailEntryLocked;
    [SerializeField] private bool _rushEntryLocked;

    #endregion

    #region Functions

    void FindPlayer()
    {
        _player = GameObject.FindWithTag("Player").transform;
    }

    public void BackToBehaviourThink()
    {
        Debug.Log($"Back To Think");
        _bt.EndYieldBehaviourFrom(_btbCurrentYieldedBehaviour);
        _agent.ResetPath();
    }

    void Idle()
    {
        if (!_agent.hasPath)
        {
            Debug.Log($"Idle:Set Destination");
            _agent.SetDestination(transform.position);
        }
        else
        {
            Debug.Log($"Idle:Reset Path");
            _agent.ResetPath();
        }
    }

    void GetClose()
    {
        FindPlayer();
        if (_agent.destination != _player.position && !_agent.hasPath)
        {
            Debug.Log($"Getting Close To Player");
            _agent.SetDestination(_player.position);
        }
    }

    void Think()
    {
        Debug.Log($"Thinking");
        _bt.YeildAllBehaviourTo(_btbAwait);
    }

    void Await()
    {
        Debug.Log($"Awaiting");
        _tAwaitET += Time.deltaTime;

        var dest = (_player.position - transform.position).normalized;
        var dot = Vector3.Dot(dest, transform.forward);
        var forwardRad = Mathf.Cos(67.5f * Mathf.Deg2Rad); // 90 * (3/4) = 67.5

        FindPlayer();
        _distanceBetPlayer = Vector3.Distance(transform.position, _player.position);

        var playerIsForward = (dot > 0) && (dot > forwardRad);
        var playerIsSide = (Mathf.Abs(dot) < forwardRad);
        var rushable = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);
        var tailable = Physics.CheckSphere(transform.position, _tailAttackRange, _playerLayers);
        var clawable = Physics.CheckSphere(transform.position, _clawAttackRange, _playerLayers);

        if (_tAwaitET > _awaitingTime)
        {
            _tAwaitET = 0;

            var rand = Random.Range(1, 100);

            _bt.EndYieldBehaviourFrom(_btbCurrentYieldedBehaviour);

            // 以下思考 アルゴリズム
            if (rushable && !tailable) // 突進距離以内かつしっぽ攻撃距離外
            {
                _bt.YeildAllBehaviourTo(_btbRush);
                _animator.SetTrigger("Rush");
            }

            if (playerIsForward && clawable) // ひっかき距離内かつ正面にいるとき
            {
                if (rand > 50)
                {
                    _bt.YeildAllBehaviourTo(_btbClaw);
                    _animator.SetTrigger("Claw");
                }
                else
                {
                    _bt.YeildAllBehaviourTo(_btbTail);
                    _animator.SetTrigger("Tail");
                }
            }

            if (playerIsForward && tailable)
            {
                _bt.YeildAllBehaviourTo(_btbTail);
                _animator.SetTrigger("Tail");
            }

            if (playerIsSide)
            {
                _bt.YeildAllBehaviourTo(_btbFacePlayer);
            }

            Random.InitState(Random.Range(0, 255));
        }
    }

    void Away()
    {
        Debug.Log($"Away From Player");
        _btbCurrentYieldedBehaviour = _btbAway;

        FindPlayer();
        var oppositeVec = (transform.position - _player.position) * .75f;
        _agent.SetDestination(oppositeVec);
    }

    void FaceToPlayer() // プレイヤを向く
    {
        Debug.Log($"Face To Player");
        _btbCurrentYieldedBehaviour = _btbFacePlayer;

        var dRot = Vector3.Lerp(transform.forward, _pDirection, Time.deltaTime);
        transform.forward += dRot;

        var dest = (_player.position - transform.position).normalized;
        var dot = Vector3.Dot(dest, transform.forward);
        var forwardRad = Mathf.Cos(22.5f * Mathf.Deg2Rad); // 90 * (3/4) = 67.5
        var playerIsForward = (dot > 0) && (dot > forwardRad);

        if (playerIsForward)
        {
            _bt.EndYieldBehaviourFrom(_btbCurrentYieldedBehaviour);
        }
    }

    void Claw()
    {
        Debug.Log($"Claw Attack To Player");
        _btbCurrentYieldedBehaviour = _btbClaw;

        if (_agent.destination != transform.position && !_agent.hasPath)
        {
            _agent.SetDestination(transform.position);
        }
    }

    void Tail()
    {
        Debug.Log($"Tail Attack To Player");
        _btbCurrentYieldedBehaviour = _btbTail;

        if (_agent.destination != transform.position && !_agent.hasPath)
        {
            _agent.SetDestination(transform.position);
        }
    }

    void Rush()
    {
        Debug.Log($"Rushing To Player");
        _btbCurrentYieldedBehaviour = _btbRush;

        if (_agent.destination != _player.position && !_agent.hasPath)
        {
            _agent.speed = _baseMoveSpeed * 3f;
            _agent.SetDestination(_player.position);
        }
        else if (_agent.hasPath)
        {
            Debug.Log($"ypaaaaaa");
            var d = Vector3.Distance(_agent.destination, transform.position);

            if (d < _clawAttackRange)
            {
                Debug.Log($"Gotcha!");
                _agent.ResetPath();
                _bt.EndYieldBehaviourFrom(_btbCurrentYieldedBehaviour);
                _bt.JumpTo(_btbThink);
            }
        }
    }

    void Death()
    {
        Debug.Log($"Death Now");
    }

    public void GetFlinch()
    {
        _bt.YeildAllBehaviourTo(_btbFlinch);
        _animator.SetTrigger("GetFlinch");
    }

    void Flinch()
    {
        Debug.Log($"Flinching Now");
        _btbCurrentYieldedBehaviour = _btbFlinch;

        _tFlinchET += Time.deltaTime;

        if (_tFlinchET > _awaitTimeOnFlinching)
        {
            _tFlinchET = 0;
            _animator.SetTrigger("EndFlinch");
        }
    }

    public void GetStumble() // どっかのタイミングで一度だけ呼び出される
    {
        _bt.YeildAllBehaviourTo(_btbStumble);
        _animator.SetTrigger("GetParry");
    }

    void Stumble() // パリィくらった時
    {
        Debug.Log($"Stumbling Now");
        _btbCurrentYieldedBehaviour = _btbStumble;

        _tStumbleET += Time.deltaTime;

        if (_tStumbleET > _awaitTimeOnStumble)
        {
            _tStumbleET = 0;
            _animator.SetTrigger("EndParry");
        }
    }

    public void EndStumbling() // animator event でこれを呼び出し
    {
        _bt.EndYieldBehaviourFrom(_btbCurrentYieldedBehaviour);
    }

    public void EndFlinching() //  animator event でこれを呼び出し
    {
        _bt.EndYieldBehaviourFrom(_btbCurrentYieldedBehaviour);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _clawAttackRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, _tailAttackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _rushAttackRange);
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
        #region Add Function To Behaviour 1.

        _btbIdle.AddBehaviour(Idle);
        _btbIdle.EEnd += () => { _agent.ResetPath(); };

        _btbGetClose.AddBehaviour(GetClose);
        _btbGetClose.EEnd += () => { _agent.ResetPath(); };

        _btbThink.AddBehaviour(Think);
        _btbThink.EEnd += () => { _agent.ResetPath(); };

        _btbAwait.AddBehaviour(Await);
        _btbAwait.SetYieldMode(true);

        _btbClaw.AddBehaviour(Claw);
        _btbClaw.SetYieldMode(true);

        _btbDeath.AddBehaviour(Death);
        _btbDeath.SetYieldMode(true);

        _btbFlinch.AddBehaviour(Flinch);
        _btbFlinch.EBegin += () => { _agent.ResetPath(); };
        _btbFlinch.SetYieldMode(true);

        _btbStumble.AddBehaviour(Stumble);
        _btbStumble.EBegin += () => { _agent.ResetPath(); };
        _btbStumble.SetYieldMode(true);

        _btbRush.AddBehaviour(Rush);
        _btbRush.EBegin += () => { _agent.stoppingDistance = _tailAttackRange * 1.5f; };
        _btbRush.EEnd += () => { _agent.stoppingDistance = 3.5f; };
        _btbRush.SetYieldMode(true);

        _btbFacePlayer.AddBehaviour(FaceToPlayer);
        _btbFacePlayer.EBegin += () =>
        {
            FindPlayer();
            Debug.Log($"Look Player!");
            _pDirection = (_player.position - transform.position);
        };
        _btbFacePlayer.SetYieldMode(true);

        _btbTail.AddBehaviour(Tail);
        _btbTail.SetYieldMode(true);

        _btbAway.AddBehaviour(Away);
        _btbAway.SetYieldMode(true);

        #endregion

        // Resist Behaviours 2.
        _bt.ResistBehaviours(new[]
        {
            _btbAwait, _btbClaw, _btbDeath, _btbFlinch, _btbIdle, _btbGetClose, _btbStumble, _btbTail, _btbRush,
            _btbAway, _btbThink, _btbFacePlayer
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

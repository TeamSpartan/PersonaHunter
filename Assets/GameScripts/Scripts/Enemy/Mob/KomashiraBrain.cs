using System.Collections.Generic;
using System.Linq;
using SgLibUnite.AI;
using UnityEngine;
using SgLibUnite.BehaviourTree;
using UnityEngine.AI;

/// <summary>
/// 菅沼 が 主担当
///  小猿（こましら）の機能を提供する
/// ver 1.0.0
/// </summary>
public class KomashiraBrain : MonoBehaviour
    , IEnemiesParameter
    , IDamagedComponent
    , IPlayerCamLockable
{
    #region 公開パラメータ

    [SerializeField, Header("パトロール経路")] private PatrollerPath _patrolPath;

    [Space(10)] [SerializeField, Header("体力の最大値")]
    private float _maxHealthPoint;

    [SerializeField, Header("待機時間")] private float _awaitingTime;
    [SerializeField, Header("視界 範囲")] private float _sightRange;
    [SerializeField, Header("攻撃 範囲")] private float _attackRange;
    [SerializeField, Header("プレイヤのレイヤー")] private LayerMask _playerLayerMask;
    [SerializeField, Header("ベースの移動速度")] private float _baseMoveSpeed;
    [SerializeField, Header("ベースのダメージ")] private float _baseDamage;
    [SerializeField, Header("探索時間]")] private float _searchingTime;
    [SerializeField, Header("威嚇時間]")] private float _intimidateTime;

    [SerializeField, Header("首のボーン")] private Transform _neckBone;

    #endregion

    #region 内部パラメータ

    /// <summary> アニメータ </summary>
    private Animator _anim;

    /// <summary> エージェント </summary>
    private NavMeshAgent _agent;

    /// <summary> ビヘイビアツリー </summary>
    private BehaviourTree _tree = new();

    /// <summary> プレイヤのトランスフォーム </summary>
    private Transform _player;

    /// <summary> 体力 </summary>
    private float _healthPoint;

    /// <summary> プレイヤからの距離 </summary>
    private float DistanceFromPlayer => Vector3.Distance(transform.position, _player.position);

    /// <summary> プレイヤの向き [正規化されてない]</summary>
    private Vector3 PlayerDirection => _player.position - transform.position;

    /// <summary> パトロールパス </summary>
    private List<Vector3> Path => _patrolPath.GetPatrollingPath.ToList();

    /// <summary> パトロールパスのポイントのインデックス </summary>
    private int _patrolPathIndex;

    private GameLogic _logic;

    #endregion

    #region コンディション

    #endregion

    #region タイマー

    private float _elapsedAwaitTime;

    private float _elapsedSearchingTime;

    private float _elapsedIntimidateTime;

    #endregion

    #region ビヘイビア

    /// <summary> 思考ビヘイビア </summary>
    private BTBehaviour _think = new();

    /// <summary> 指定した距離をパトロールする </summary>
    private BTBehaviour _patrol = new();

    /// <summary> 見渡し </summary>
    private BTBehaviour _search = new();

    /// <summary> プレイヤへ近づく </summary>
    private BTBehaviour _gotoPlayer = new();

    /// <summary> 威嚇 </summary>
    private BTBehaviour _intimidate = new();

    /// <summary> 攻撃 </summary>
    private BTBehaviour _attackToPlayer = new();

    /// <summary> プレイヤを向く </summary>
    private BTBehaviour _lookToPlayer = new();

    /// <summary> 死亡 </summary>
    private BTBehaviour _death = new();

    /// <summary> 現在再生中のイベント </summary>
    private BTBehaviour _currentYielded = new();

    #endregion

    /// <summary> ベースのダメージ </summary>
    public float GetBaseDamage => _baseDamage;

    /// <summary> 現在の体力 </summary>
    public float GetHealthPoint => _healthPoint;

    /// <summary> 体力の最大値 </summary>
    public float GetMaxHealthPoint => _maxHealthPoint;

    public float GetHealth()
    {
        return _healthPoint;
    }

    public void SetHealth(float val)
    {
        _healthPoint = val;
    }

    // Animation Event から 呼び出し

    public void EnableAttackCollider()
    {
        _neckBone.GetComponent<Collider>().enabled = true;
    }

    public void DisableAttackCollider()
    {
        _neckBone.GetComponent<Collider>().enabled = false;
    }

    public void Start()
    {
        _logic = GameObject.FindAnyObjectByType<GameLogic>();
        _logic.ApplyEnemyTransform(transform);

        SetupComponent();
        SetupBehaviours();
        _tree.StartBT();
        FindPlayer();

        if (_tree.CurrentYieldedEvent != _think)
        {
            _tree.YieldAllBehaviourTo(_think);
        }
    }

    private void SetupComponent()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _healthPoint = _maxHealthPoint;
    }

    private void SetupBehaviours()
    {
        _think.AddBehaviour(Think);
        _think.EBegin += () => Debug.Log($"entry-think");
        _think.EEnd += () => Debug.Log($"exit-think");
        _think.SetYieldMode(true);

        _patrol.AddBehaviour(Patrol);
        _patrol.EBegin += () => Debug.Log($"entry-patrol");
        _patrol.EEnd += () => Debug.Log($"exit-patrol");
        _patrol.EBegin += () => { _anim.SetTrigger("Walk"); };
        _patrol.SetYieldMode(true);

        _search.AddBehaviour(Search);
        _search.EBegin += () => Debug.Log($"entry-search");
        _search.EEnd += () => Debug.Log($"exit-search");
        _search.EBegin += () => { _anim.SetTrigger("Search"); };
        _search.SetYieldMode(true);

        _gotoPlayer.AddBehaviour(GotoPlayer);
        _gotoPlayer.EBegin += () => Debug.Log($"entry-goto player");
        _gotoPlayer.EEnd += () => Debug.Log($"exit-goto player");
        _gotoPlayer.EBegin += () => { _anim.SetTrigger("Walk"); };
        _gotoPlayer.EEnd += () => { _anim.ResetTrigger("Walk"); };
        _gotoPlayer.EEnd += ResetPath;
        _gotoPlayer.SetYieldMode(true);

        _intimidate.AddBehaviour(Intimidate);
        _intimidate.EBegin += () => Debug.Log($"entry-inimidate");
        _intimidate.EEnd += () => Debug.Log($"exit-inimidate");
        _intimidate.EBegin += () => { _anim.SetTrigger("StartIntimidate"); };
        _intimidate.SetYieldMode(true);

        _lookToPlayer.AddBehaviour(LookPlayer);
        _lookToPlayer.EBegin += () => Debug.Log($"entry-looktoplayer");
        _lookToPlayer.EEnd += () => Debug.Log($"exit-looktoplayer");
        _lookToPlayer.SetYieldMode(true);

        _attackToPlayer.AddBehaviour(AttackToPlayer);
        _attackToPlayer.EBegin += () => Debug.Log($"entry-attacktkoplayer");
        _attackToPlayer.EEnd += () => Debug.Log($"exit-attacktkoplayer");
        _attackToPlayer.EBegin += () => { _anim.SetTrigger("Pounce"); };
        _attackToPlayer.SetYieldMode(true);

        _death.AddBehaviour(Death);
        _death.EBegin += () => Debug.Log($"entry-death");
        _death.EEnd += () => Debug.Log($"exit-death");
        _death.EBegin += () => { _anim.SetTrigger("Die"); };
        _death.SetYieldMode(true);

        BTBehaviour[] behaviours = new[]
        {
            _think,
            _patrol,
            _search,
            _gotoPlayer,
            _intimidate,
            _attackToPlayer,
            _lookToPlayer,
            _death
        };

        _tree.ResistBehaviours(behaviours);
    }

    /// <summary> ナビメッシュのパスをリセット </summary>
    private void ResetPath() => _agent.ResetPath();

    /// <summary> プレイヤのトランスフォームを取得 </summary>
    private void FindPlayer() => _player = GameObject.FindWithTag("Player").transform;

    public void NotifyToBackThink()
    {
        _tree.EndYieldBehaviourFrom(_currentYielded);
        _tree.YieldAllBehaviourTo(_think);
    }

    /// <summary> 思考 </summary>
    private void Think()
    {
        _currentYielded = _think;

        _elapsedAwaitTime += Time.deltaTime;

        var dot = Vector3.Dot(PlayerDirection.normalized, transform.forward);
        var forwardRadian = Mathf.Cos(90f * (3f / 4f) * Mathf.Deg2Rad);
        var playerIsForward = dot > 0 && dot > forwardRadian;
        var playerIsSide = Mathf.Abs(dot) < forwardRadian;
        var playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);
        var inRange = Physics.CheckSphere(transform.position, _attackRange, _playerLayerMask);

        if (_elapsedAwaitTime > _awaitingTime)
        {
            if (playerFound)
            {
                if (playerIsForward)
                {
                    _tree.EndYieldBehaviourFrom(_currentYielded);
                    _tree.YieldAllBehaviourTo(_gotoPlayer);
                }
                else
                {
                    _tree.EndYieldBehaviourFrom(_currentYielded);
                    _tree.YieldAllBehaviourTo(_lookToPlayer);
                }
            }
            else
            {
                if (_tree.CurrentYieldedEvent != _patrol)
                {
                    _tree.EndYieldBehaviourFrom(_currentYielded);

                    var rand = Random.Range(1, 11);
                    if (rand <= 3)
                    {
                        _tree.YieldAllBehaviourTo(_search);
                    }
                    else
                    {
                        _tree.YieldAllBehaviourTo(_patrol);
                    }
                }
            }
        }
    }

    /// <summary> パトロールをする </summary>
    private void Patrol()
    {
        _currentYielded = _patrol;

        var dis = Vector3.Distance(Path[_patrolPathIndex], transform.position);
        var playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);

        if (playerFound)
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_gotoPlayer);
        }
        else if (dis < 1.5f)
        {
            var rand = Random.Range(1, 11);
            if (rand <= 3)
            {
                _tree.EndYieldBehaviourFrom(_currentYielded);
                _tree.YieldAllBehaviourTo(_search);
            }
            else
            {
                _patrolPathIndex = _patrolPathIndex + 1 < Path.Count ? _patrolPathIndex + 1 : 0;
                _agent.SetDestination(Path[_patrolPathIndex]);
            }

            Random.InitState(Random.Range(0, 256));
        }
        else if (_agent.destination != Path[_patrolPathIndex])
        {
            _agent.SetDestination(Path[_patrolPathIndex]);
        }
    }


    /// <summary> プレイヤの捜索 </summary>
    private void Search()
    {
        _currentYielded = _search;

        _elapsedSearchingTime += Time.deltaTime;

        var playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);

        if (playerFound)
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_gotoPlayer);
        }
        else if (_elapsedSearchingTime > _searchingTime)
        {
            _elapsedSearchingTime = 0;
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_think);
        }
    }

    /// <summary> プレイヤへ向かう </summary>
    private void GotoPlayer()
    {
        _currentYielded = _gotoPlayer;

        FindPlayer();
        _agent.SetDestination(_player.position);

        var isInRange = Physics.CheckSphere(transform.position, _attackRange, _playerLayerMask);
        var playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);

        if (!playerFound)
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_think);
        }

        if (isInRange)
        {
            var rand = Random.Range(1, 11);

            if (rand++ <= 9)
            {
                _tree.EndYieldBehaviourFrom(_currentYielded);
                _tree.YieldAllBehaviourTo(_attackToPlayer);
            }
            else
            {
                _tree.EndYieldBehaviourFrom(_currentYielded);
                _tree.YieldAllBehaviourTo(_intimidate);
            }
        }
    }

    /// <summary> 威嚇する </summary>
    private void Intimidate()
    {
        _currentYielded = _intimidate;

        _elapsedIntimidateTime += Time.deltaTime;

        var isInRange = Physics.CheckSphere(transform.position, _attackRange, _playerLayerMask);
        var playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);

        if (!playerFound)
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_think);
        }

        if (_elapsedIntimidateTime > _intimidateTime)
        {
            _elapsedIntimidateTime = 0;
            _anim.SetTrigger("EndIntimidate");
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_think);
        }
    }

    /// <summary> プレイヤーを攻撃する </summary>
    private void AttackToPlayer()
    {
        _currentYielded = _attackToPlayer;

        var playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);

        if (!playerFound)
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.YieldAllBehaviourTo(_think);
        }
    }

    /// <summary> プレイヤを向く </summary>
    private void LookPlayer()
    {
        _currentYielded = _lookToPlayer;

        var deltaRotation = Vector3.Lerp(transform.forward, PlayerDirection, Time.deltaTime);
        transform.forward += deltaRotation;

        var dot = Vector3.Dot(PlayerDirection.normalized, transform.forward);
        var forwardRadian = Mathf.Cos(90f / 4f * Mathf.Deg2Rad);
        var playerIsForward = dot > 0 && dot > forwardRadian;

        if (playerIsForward)
        {
            _tree.EndYieldBehaviourFrom(_lookToPlayer);
            _tree.YieldAllBehaviourTo(_gotoPlayer);
        }
    }

    /// <summary> 死亡 </summary>
    private void Death()
    {
        _currentYielded = _death;
        Debug.Log("Death Now");
    }

    public void FixedUpdate()
    {
        _tree.UpdateEventsYield();
    }

    public void OnDisable()
    {
        _tree.PauseBT();
    }

    public void StartDull()
    {
        _anim.SetLayerWeight(0, 0f);
        _anim.SetLayerWeight(1, 1f);
    }

    public void EndDull()
    {
        _anim.SetLayerWeight(0, 1f);
        _anim.SetLayerWeight(1, 0f);
    }

    public void AddDamage(float dmg)
    {
        if (_healthPoint > 0)
        {
            _healthPoint -= dmg;
            if (_healthPoint <= 0)
            {
                _tree.EndYieldBehaviourFrom(_currentYielded);
                _tree.YieldAllBehaviourTo(_death);
            }
        }
    }

    public void Kill()
    {
        _healthPoint = 0;
        _tree.EndYieldBehaviourFrom(_currentYielded);
        _tree.YieldAllBehaviourTo(_death);
    }

    public void ConfirmDeath()
    {
        _tree.PauseBT();
        Destroy(this.gameObject, 1f);
    }

    public Transform GetLockableObjectTransform()
    {
        return transform;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

using System.Collections.Generic;
using System.Linq;
using SgLibUnite.AI;
using UnityEngine;
using SgLibUnite.BehaviourTree;
using SgLibUnite.CodingBooster;
using UnityEngine.AI;

/// <summary>
/// 菅沼 が 主担当
///  ぬゑ 幼体 （雑魚）
/// ver 1.0.0
/// </summary>
///              ～～フロー～～
///               [PATROL]
///                  | (1)
///                  V                       [ANY BEHAVIOURs]
///       ,-<----------------->-,                (6)
///       |(2)                  |(3)             |
///       V                     V                V
///    [SURVEY]----(3)---->[GOTO-PLAYER]      [DEATH]
///                            |(4)
///                            V
///                     [intimidate]
///                           |(5)
///                           V
///                     [ATTACK PLAYER]
/// 
/// (1) パトロール中50%で遷移
/// (2) 捜索処理
/// (3) プレイヤが発見出来たら
/// (4) 攻撃範囲内に入ったら
/// (5) 威嚇終わったら
/// (6) 体力が0なら
public class NuweJuvenile : MonoBehaviour
    , IMobBehaviourParameter
    , IInitializableComponent
    , IDulledTarget
    , IDamagedComponent
    , IPlayerCamLockable
{
    #region 公開パラメータ

    [SerializeField, Header("パトロール経路")] private PatrollerPathContainer _patrolPath;

    [Space(10)] [SerializeField, Header("体力の最大値")]
    private float _maxHealthPoint;

    [SerializeField, Header("待機時間")] private float _awaitingTime;
    [SerializeField, Header("視界 範囲")] private float _sightRange;
    [SerializeField, Header("攻撃 範囲")] private float _attackRange;
    [SerializeField, Header("プレイヤのレイヤー")] private LayerMask _playerLayerMask;
    [SerializeField, Header("ベースの移動速度")] private float _baseMoveSpeed;
    [SerializeField, Header("ベースのダメージ")] private float _baseDamage;

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

    /// <summary> 開発ブースター </summary>
    private CBooster _booster = new();

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

    private bool _dummyCond = false;

    #endregion

    #region タイマー

    private float _elapsedAwaitTime;

    #endregion

    #region トランジション

    private string _tnEntryToThink = "0";

    #endregion

    #region ビヘイビア

    /// <summary> ダミービヘイビア </summary>
    private BTBehaviour _dummy = new();

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

    /// <summary> Animation Event から 呼び出し </summary>
    public void EndIntimidate()
    {
        _anim.SetTrigger("EndIntimidate");
    }

    public void NotifyEndAttackMotion()
    {
        _tree.EndYieldBehaviourFrom(_attackToPlayer);
    }

    public void NotifyEndSearching()
    {
        _tree.EndYieldBehaviourFrom(_search);
    }

    public void InitializeThisComponent()
    {
        SetupComponent();
        SetupBehaviours();
        SetupTransitions();
        _tree.StartBT();
        FindPlayer();
    }

    private void SetupComponent()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void SetupBehaviours()
    {
        _dummy.AddBehaviour(() => { });

        _think.AddBehaviour(Think);

        _patrol.AddBehaviour(Patrol);
        _patrol.EBegin += () => { _anim.SetTrigger("Walk"); };
        _patrol.EEnd += ResetPath;
        _patrol.EBegin += FindPlayer;
        _patrol.SetYieldMode(true);

        _search.AddBehaviour(Search);
        _search.EBegin += ResetPath;
        _search.EBegin += FindPlayer;
        _search.EBegin += () => { _anim.SetTrigger("Search"); };
        _search.SetYieldMode(true);

        _gotoPlayer.AddBehaviour(GotoPlayer);
        _gotoPlayer.EBegin += () => { _anim.SetTrigger("Walk"); };
        _gotoPlayer.EBegin += ResetPath;
        _gotoPlayer.EBegin += FindPlayer;
        _gotoPlayer.SetYieldMode(true);

        _intimidate.AddBehaviour(Intimidate);
        _intimidate.EBegin += () => { _anim.SetTrigger("StartIntimidate"); };
        _intimidate.SetYieldMode(true);

        _lookToPlayer.AddBehaviour(LookPlayer);
        _lookToPlayer.EBegin += () => { _anim.SetTrigger("FoundPlayer"); };
        _lookToPlayer.EEnd += ResetPath;
        _lookToPlayer.SetYieldMode(true);

        _attackToPlayer.AddBehaviour(AttackToPlayer);
        _attackToPlayer.EBegin += () => { _anim.SetTrigger("Pounce"); };
        _attackToPlayer.EBegin += ResetPath;

        _death.AddBehaviour(Death);
        _death.SetYieldMode(true);

        BTBehaviour[] behaviours = new[]
        {
            _think,
            _dummy,
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

    private void SetupTransitions()
    {
        _tree.MakeTransition(_think, _dummy, _tnEntryToThink);
    }

    /// <summary> ナビメッシュのパスをリセット </summary>
    private void ResetPath() => _agent.ResetPath();

    /// <summary> プレイヤのトランスフォームを取得 </summary>
    private void FindPlayer() => _player = GameObject.FindWithTag("Player").transform;

    /// <summary> 思考 </summary>
    private void Think()
    {
        FindPlayer();
        var playerIsInRange = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);
        var playerIsInAttackRange = Physics.CheckSphere(transform.position, _attackRange, _playerLayerMask);
        var dotProduct = Vector3.Dot(PlayerDirection.normalized, transform.forward);
        var forwardRad = Mathf.Cos(90f * (3f / 4f) * Mathf.Deg2Rad);

        var playerIsForward = dotProduct > 0 && dotProduct > forwardRad;
        var playerIsSide = Mathf.Abs(dotProduct) < forwardRad;

        if (!playerIsInRange)
        {
            Debug.Log($"TRANSIT PATROL");
            _tree.YieldAllBehaviourTo(_patrol);
        }
    }

    /// <summary> パトロールをする </summary>
    private void Patrol()
    {
        Debug.Log($"PATROL");
        _currentYielded = _patrol;

        var dis = Vector3.Distance(Path[_patrolPathIndex], transform.position);

        if (dis < 1.5f)
        {
            
            var rand = Random.Range(1, 11);
            if (rand <= 3)
            {
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

        var isFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayerMask);

        if (isFound)
        {
            _tree.YieldAllBehaviourTo(_lookToPlayer);
        }
    }

    /// <summary> プレイヤへ向かう </summary>
    private void GotoPlayer()
    {
        _currentYielded = _gotoPlayer;

        if (!_agent.hasPath && _agent.destination != _player.position)
        {
            _agent.SetDestination(_player.position);
        }
        else
        {
            var d = Vector3.Distance(_agent.destination, _player.position);
            if (d < 1.5f)
            {
                _agent.ResetPath();
            }
        }
    }

    /// <summary> 威嚇する </summary>
    private void Intimidate()
    {
        _currentYielded = _intimidate;

        _elapsedAwaitTime += Time.deltaTime;

        if (_elapsedAwaitTime > _awaitingTime)
        {
            _elapsedAwaitTime = 0;
            _anim.SetTrigger("EndIntimidate");
        }
    }

    /// <summary> プレイヤーを攻撃する </summary>
    private void AttackToPlayer()
    {
        _currentYielded = _attackToPlayer;
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
        }
    }

    /// <summary> 死亡 </summary>
    private void Death()
    {
        _currentYielded = _death;
    }

    public void FixedTickThisComponent()
    {
        _tree.UpdateEventsYield();
        _tree.UpdateTransition(_tnEntryToThink, ref _dummyCond);
    }

    public void TickThisComponent()
    {
    }

    public void FinalizeThisComponent()
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
        _healthPoint -= dmg;
    }

    public void Kill()
    {
        _healthPoint = 0;
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

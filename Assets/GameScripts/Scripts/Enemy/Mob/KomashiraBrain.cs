using System;
using System.Linq;
using UnityEngine;
using SgLibUnite.AI;
using UnityEngine.AI;
using UnityEngine.UI;
using SgLibUnite.BehaviourTree;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// 菅沼 が 主担当
///  小猿（こましら）の機能を提供する
/// ver 1.0.0
/// </summary>
public class KomashiraBrain : MonoBehaviour
    , IEnemiesParameter
    , IDamagedComponent
    , IPlayerCamLockable
    , IEnemyDieNotifiable
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

    private KomashiraHPBar _bar;

    private AudioManager _audioManager;

    /// <summary> 体力 </summary>
    private float _healthPoint;

    private Slider _slider;

    /// <summary> プレイヤからの距離 </summary>
    private float DistanceFromPlayer => Vector3.Distance(transform.position, _player.position);

    /// <summary> プレイヤの向き [正規化されてない]</summary>
    private Vector3 PlayerDirection => _player.position - transform.position;

    /// <summary> パトロールパス </summary>
    private List<Vector3> Path => _patrolPath.GetPatrollingPath.ToList();

    /// <summary> パトロールパスのポイントのインデックス </summary>
    private int _patrolPathIndex;

    private MainGameLoop _loop;

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

    /// <summary>
    /// 歩行SE再生
    /// </summary>
    public void PlayWalkSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("EnemyWalk");
    }
    /// <summary>
    /// ジャンピング攻撃SE再生
    /// </summary>
    public void PlayJumpingAttackSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("EnemyAttackJumping");
    }
    /// <summary>
    /// ひっかき攻撃SE再生
    /// </summary>
    public void PlayScratchSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("EnemyAttackScratch");
    }
    /// <summary>
    /// ダメージSE再生
    /// </summary>
    public void PlayDamageSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("EnemyDamage");
    }
    /// <summary>
    /// ジャンプSE再生
    /// </summary>
    public void PlayJumpSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("");
    }
    /// <summary>
    /// 着地音SE再生
    /// </summary>
    public void PlayJumpAgoSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("EnemyJumpAgo");
    }
    /// <summary>
    /// ダウンSE再生
    /// </summary>
    public void PlayDownSE()
    {
        if(!_audioManager)return;
        _audioManager.PlaySE("EnemyDown");
    }
    
    
    private void Start()
    {
        var bar = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/KomashiraHPBar"));
        _bar = bar.GetComponent<KomashiraHPBar>();
        _bar.SetFollowingTarget(transform);

        _slider = _bar.GetComponent<Slider>();
        _slider.maxValue = _maxHealthPoint;

        GetComponent<Rigidbody>().mass = 100;

        _loop = GameObject.FindAnyObjectByType<MainGameLoop>();
        var t = transform;
        var p = transform.position;
        p += Vector3.up;
        t.position = p;
        _loop.ApplyEnemyTransform(t);
        _loop.EDiveInZone += StartFreeze;
        _loop.EGetOutZone += EndFreeze;

        SetupComponent();
        SetupBehaviours();

        _tree.StartBT();

        FindPlayer();

        if (_tree.CurrentYieldedEvent != _think)
        {
            _tree.YieldAllBehaviourTo(_think);
        }

        SceneManager.activeSceneChanged += OnactiveSceneChanged;
    }

    private void OnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        if (_loop is not null && SceneManager.GetActiveScene().name is not ConstantValues.InGameScene)
        {
            _loop.EDiveInZone -= StartFreeze;
            _loop.EGetOutZone -= EndFreeze;
        }
    }

    private void Update()
    {
        if (_slider is not null)
            _slider.value = _healthPoint;
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
        _think.SetYieldMode(true);

        _patrol.AddBehaviour(Patrol);
        _patrol.EBegin += () => { _anim.SetTrigger("Walk"); };
        _patrol.SetYieldMode(true);

        _search.AddBehaviour(Search);
        _search.EBegin += () => { _anim.SetTrigger("Search"); };
        _search.SetYieldMode(true);

        _gotoPlayer.AddBehaviour(GotoPlayer);
        _gotoPlayer.EBegin += () => { _anim.SetTrigger("Walk"); };
        _gotoPlayer.EEnd += () => { _anim.ResetTrigger("Walk"); };
        _gotoPlayer.EEnd += ResetPath;
        _gotoPlayer.SetYieldMode(true);

        _intimidate.AddBehaviour(Intimidate);
        _intimidate.EBegin += () => { _anim.SetTrigger("StartIntimidate"); };
        _intimidate.SetYieldMode(true);

        _lookToPlayer.AddBehaviour(LookPlayer);
        _lookToPlayer.SetYieldMode(true);

        _attackToPlayer.AddBehaviour(AttackToPlayer);
        _attackToPlayer.EBegin += () => { _anim.SetTrigger("Pounce"); };
        _attackToPlayer.SetYieldMode(true);

        _death.AddBehaviour(Death);
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
    }

    public void FixedUpdate()
    {
        _tree.UpdateEventsYield();
    }

    public void StartFreeze()
    {
        if (_healthPoint <= 0) return;

        // _anim.SetLayerWeight(0, 0f);
        // _anim.SetLayerWeight(1, 1f);

        _tree.EndYieldBehaviourFrom(_currentYielded);
        _tree.PauseBT();

        _agent.ResetPath();
        _anim.enabled = false;
    }

    public void EndFreeze()
    {
        if (_healthPoint <= 0) return;

        // _anim.SetLayerWeight(0, 1f);
        // _anim.SetLayerWeight(1, 0f);

        _tree.StartBT();
        _tree.YieldAllBehaviourTo(_think);
        _anim.enabled = true;
    }

    public void AddDamage(float dmg)
    {
        if (_healthPoint < 0 && _currentYielded == _death) return;

        if (_bar is null)
        {
            var hpBars =
                GameObject.FindObjectsByType<KomashiraHPBar>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var bar in hpBars)
            {
                if (bar.FollowingTarget == transform && bar.TryGetComponent<KomashiraHPBar>(out var c))
                {
                    _bar = c;
                }
            }
        }

        _bar.PunchGuage();

        _healthPoint -= dmg;

        if (_healthPoint <= 0)
        {
            if (_tree.IsPaused)
            {
                _anim.enabled = true;
                _tree.StartBT();
                _tree.YieldAllBehaviourTo(_death);
            }
            else
            {
                _tree.EndYieldBehaviourFrom(_currentYielded);
                _tree.YieldAllBehaviourTo(_death);
            }

            _loop.EDiveInZone -= StartFreeze;
            _loop.EGetOutZone -= EndFreeze;
        }
    }

    public void Kill()
    {
        _healthPoint = 0;
        _tree.EndYieldBehaviourFrom(_currentYielded);
        _tree.YieldAllBehaviourTo(_death);
    }

    public void ConfirmDeath() /* アニメションを再生仕切ってから破棄したいのでここに処理を書いている */
    {
        _tree.PauseBT();

        // 死亡通知
        _loop.NotifyEnemyIsDeath(IEnemyDieNotifiable.EnemyType.Komashira, gameObject);

        // コンポーネントの破棄
        _bar.DestroySelf();
        Destroy(GetComponent<Rigidbody>());
        Destroy(_anim);
        Destroy(_agent);
        Destroy(GetComponent<KomashiraBrain>());
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

    public void NotifyEnemyIsDeath(IEnemyDieNotifiable.EnemyType type, GameObject enemy)
    {
    }
}

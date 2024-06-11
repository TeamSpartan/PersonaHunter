using SgLibUnite.BehaviourTree;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

/* 菅沼が主担当 */

/// <summary>
/// ぬえの第一形態の機能を提供
/// ver 3.3.1
/// </summary>
public class NuweBrain : MonoBehaviour
    , IEnemiesParameter
    , IDamagedComponent
    , IPlayerCamLockable
{
    #region 外部パラメータ

    [SerializeField, Header("体力の最大値")] private float _healthMaxValue;

    [SerializeField, Header("待機時間")] private float _awaitingTime;

    [SerializeField, Header("パリィ成功時の待機時間")]
    private float _awaitTimeOnStumble;

    [SerializeField, Header("ひるみ成功時の待機時間")]
    private float _awaitTimeOnFlinching;

    [SerializeField, Range(1f, 100f), Header("視界 範囲")]
    private float _sightRange;

    [SerializeField, Range(1f, 100f), Header("ひっかき攻撃 範囲")]
    private float _clawAttackRange;

    [SerializeField, Range(5f, 100f), Header("しっぽ攻撃 範囲")]
    private float _tailAttackRange;

    [SerializeField, Range(10f, 100f), Header("突進攻撃 範囲")]
    private float _rushAttackRange;

    [SerializeField, Header("プレイヤ レイヤマスク")]
    private LayerMask _playerLayers;

    [SerializeField, Header("プレイヤのタグ")] private string _playerTag;

    [SerializeField, Range(1f, 50f), Header("ベースの移動速度")]
    private float _baseMoveSpeed;

    [SerializeField, Range(1f, 50f), Header("怒り時の移動速度")]
    private float _moveSpeedOnMad;

    [SerializeField, Header("右前足首 ボーン")] private Transform _rightWristBone;

    [SerializeField, Header("首の付け根 ボーン")] private Transform _neckRootBone;

    [SerializeField, Header("しっぽ ボーン")] private Transform _tailBone;


    [SerializeField, Header("ベースのダメージ")] private float _baseDamage;

    /// <summary> ベースのダメージ量 </summary>
    public float GetBaseDamage => _baseDamage;

    /// <summary> 体力を取得する </summary>
    public float GetHealthPoint => _healthPoint;

    /// <summary> 最大体力を取得する </summary>
    public float GetMaxHP => _healthMaxValue;

    #endregion

    #region ビヘイビア

    /// <summary> デレ行動 </summary>
    private BTBehaviour _idle = new();

    /// <summary> プレイヤとの距離を詰める </summary>
    private BTBehaviour _getClose = new();

    /// <summary> 待機、思考行動 </summary>
    private BTBehaviour _await = new();

    /// <summary> プレイヤから離れる </summary>
    private BTBehaviour _away = new();

    /// <summary> ひるみ行動 </summary>
    private BTBehaviour _flinch = new();

    /// <summary> 死亡行動 </summary>
    private BTBehaviour _death = new();

    /// <summary> パリィ成功時の行動 </summary>
    private BTBehaviour _stumble = new();

    /// <summary> ひっかき攻撃 </summary>
    private BTBehaviour _claw = new();

    /// <summary> しっぽ攻撃 </summary>
    private BTBehaviour _tail = new();

    /// <summary> 突進攻撃 </summary>
    private BTBehaviour _rush = new();

    /// <summary> プレイヤを向く </summary>
    private BTBehaviour _lookPlayer = new();

    /// <summary> 現在ツリーから枝分かれしていない行動で実行中の行動 </summary>
    private BTBehaviour _currentYielded = new();

    #endregion

    #region ビヘイビア遷移のコンディション

    /// <summary> プレイヤ発見フラグ </summary>
    private bool _playerFound;

    /// <summary> プレイヤが攻撃範囲内かのフラグ </summary>
    private bool _playerIsInRange;

    #endregion

    #region トランジション名

    /// <summary> 待機から次の行動へ移行する遷移の登録名 </summary>
    private string _tNameStartThink = "_";

    /// <summary> 思考から距離つめへ移行する遷移の登録名 </summary>
    private string _tNameStartGetClose = "__";

    #endregion

    #region タイマー

    /// <summary> ひるみ時間 タイマ </summary>
    private float _elapsedFlinchingTime;

    /// <summary> パリィ後待機時間 タイマ </summary>
    private float _elapsedStumbleingTime;

    /// <summary> 待機時間 タイマ </summary>
    private float _elapsedAwaitingTime;

    #endregion

    #region 内部パラメータ

    /// <summary> ビヘイビアツリー </summary>
    private BehaviourTree _tree = new();

    /// <summary> プレイヤのトランスフォーム </summary>
    private Transform _player;

    /// <summary> ナビメッシュ </summary>
    private NavMeshAgent _agent;

    /// <summary> アニメータ </summary>
    private Animator _anim;

    /// <summary> プレイヤの方向 </summary>
    private Vector3 _playerDirection;

    /// <summary> ひるみ値 : 100 で怯み状態に移行 </summary>
    private float _flinchPoint;

    /// <summary> 体力値 : 0 で死亡状態へ移行 </summary>
    private float _healthPoint;

    /// <summary> プレイヤとの距離 </summary>
    private float DistanceFromPlayer => Vector3.Distance(transform.position, _player.position);

    /// <summary> 怒り状態かのフラグ </summary>
    private bool IsMad => _healthMaxValue * .5f > _healthPoint;

    /// <summary> ひっかき行動の実行ロックのフラグ </summary>
    private bool _lockedClaw;

    /// <summary> しっぽ攻撃行動の実行ロックのフラグ </summary>
    private bool _lockedTail;

    /// <summary> 突進行動の実行ロックのフラグ </summary>
    private bool _lockedRush;

    /// <summary> しっぽ攻撃時の当たり判定中かのフラグ </summary>
    private bool _isCheckingTailColDetection;

    /// <summary> ひっかき攻撃時の当たり判定中かのフラグ </summary>
    private bool _isCheckingClawColDetection;

    /// <summary> 突進攻撃時の当たり判定中かのフラグ </summary>
    private bool _isCheckingRushColDetection;

    private GameLogic _logic;

    #endregion

    public void AddDamage(float dmg)
    {
        _healthPoint -= dmg;
        _flinchPoint += dmg * .25f;
    }

    public void Kill()
    {
        _healthPoint = 0;
    }

    public float GetHealth()
    {
        return _healthPoint;
    }

    public void SetHealth(float val)
    {
        _healthPoint = val;
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

    private void OnDrawGizmosSelected()
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

    private void OnTriggerEnter(Collider other) // 右手、しっぽ、本体 にアタッチされているコライダーからの当たり判定のイベント
    {
        if (other.CompareTag("Player"))
        {
            CheckPlayerIsGuarding(other);
        }
    }

    /// <summary>
    /// 当たり判定で検出したオブジェクト(i) のトランスフォームに対応したぬえの攻撃タイプを判定。
    /// <para>
    /// (i) 攻撃ごとの当たり判定のオブジェクト
    /// </para>
    /// </summary>
    public NueAttackType GetAttackType(Transform other)
    {
        var ret = NueAttackType.None;
        if (other == _tailBone)
        {
            ret = NueAttackType.Tail;
        }

        if (other == _neckRootBone)
        {
            ret = NueAttackType.Rush;
        }

        if (other == _rightWristBone)
        {
            ret = NueAttackType.Claw;
        }

        return ret;
    }
    
    public NueAttackType GetAttackType(NueAttackType attackType)
    {
        Transform ret = null;
        
    
        return ret;
    }

    /// <summary>
    /// ぬえ 攻撃バリエーション 列挙型
    /// </summary>
    public enum NueAttackType
    {
        Rush,
        Claw,
        Tail,
        None
    }

    /// <summary>
    /// 待機に戻る。
    /// </summary>
    public void BackToAwait()
    {
        _tree.EndYieldBehaviourFrom(_currentYielded);
        ResetAgentPath();
    }

    /// <summary>
    /// 右手のコライダーを有効化
    /// </summary>
    public void EnableClawCollider()
    {
        _rightWristBone.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// 右手のコライダーを無効化
    /// </summary>
    public void DisableClawCollider()
    {
        _rightWristBone.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// 突進のコライダーを有効化
    /// </summary>
    public void EnableRushCollider()
    {
        _neckRootBone.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// 突進のコライダーを無効化
    /// </summary>
    public void DisableRushCollider()
    {
        _neckRootBone.GetComponent<Collider>().enabled = false;
    }

    /// <summary>
    /// しっぽのコライダーを有効化
    /// </summary>
    public void EnableTailCollider()
    {
        _tailBone.GetComponent<Collider>().enabled = true;
    }

    /// <summary>
    /// しっぽのコライダーを無効化
    /// </summary>
    public void DisableTailCollider()
    {
        _tailBone.GetComponent<Collider>().enabled = false;
    }

    public Transform GetLockableObjectTransform()
    {
        return this.transform;
    }

    /// <summary>
    /// プレイヤと当たった判定のイベントが発火されたときにこれを呼び出してプレイヤがガード中か判定してそれに合わせた処理をする
    /// </summary>
    public void CheckPlayerIsGuarding(Collider other)
    {
        if (other.GetComponent<IAbleToParry>() != null)
        {
            if (other.GetComponent<IAbleToParry>().NotifyPlayerIsGuarding())
            {
                other.GetComponent<IAbleToParry>().ParrySuccess();
                GetParry();
            }
        }
    }

    public void Start()
    {
        _logic = GameObject.FindAnyObjectByType<GameLogic>();
        _logic.ApplyEnemyTransform(transform);

        SetupBehaviours();
        MakeTransitions();
        _tree.StartBT();
        SetupComponent();
    }

    public void FixedUpdate()
    {
        _tree.UpdateEventsYield(); // 割り込みイベントを更新
        _playerFound = Physics.CheckSphere(transform.position, _sightRange, _playerLayers);
        _playerIsInRange = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);

        if (!_playerFound) // プレイヤが見つからないならIdleに戻る
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.JumpTo(_idle);
        }
        else if (!_playerIsInRange) // 発見はしたが攻撃範囲内にいないとき
        {
            _tree.EndYieldBehaviourFrom(_currentYielded);
            _tree.JumpTo(_getClose);
        }

        if (_flinchPoint > 100) // 攻撃を受け続けて怯み値がたまり切ったら
        {
            GetFinch();
        }

        // イベントではない通常ビヘイビアの遷移が可能かチェック（更新）
        _tree.UpdateTransition(_tNameStartGetClose, ref _playerFound);
        _tree.UpdateTransition(_tNameStartThink, ref _playerIsInRange);
    }

    /// <summary>
    /// 各ビヘイビアをセットアップ
    /// </summary>
    private void SetupBehaviours()
    {
        _idle.AddBehaviour(Idle);
        _idle.EEnd += ResetAgentPath;

        _getClose.AddBehaviour(GetClose);
        _getClose.EEnd += ResetAgentPath;

        _await.AddBehaviour(Await);
        _await.SetYieldMode(true);

        _death.AddBehaviour(Death);
        _death.SetYieldMode(true);

        _flinch.AddBehaviour(Flinch);
        _flinch.SetYieldMode(true);
        _flinch.EEnd += ResetAgentPath;

        _stumble.AddBehaviour(Stumble);
        _stumble.SetYieldMode(true);
        _stumble.EEnd += ResetAgentPath;

        _rush.AddBehaviour(Rush);
        _rush.SetYieldMode(true);
        _rush.EBegin += () => { _anim.SetTrigger("Rush"); };
        _rush.EEnd += ResetAgentPath;

        _tail.AddBehaviour(Tail);
        _tail.EBegin += () => { _anim.SetTrigger("Tail"); };
        _tail.SetYieldMode(true);

        _claw.AddBehaviour(Claw);
        _claw.EBegin += () => { _anim.SetTrigger("Claw"); };
        _claw.SetYieldMode(true);

        _lookPlayer.AddBehaviour(LookPlayer);
        _lookPlayer.SetYieldMode(true);
        _lookPlayer.EBegin += FindPlayerDirection;

        // ↑ でビヘイビアをどんなビヘイビアか指定している。また、ビヘイビアへ入った際の処理もここで指定している。

        BTBehaviour[] behaviours = new[]
        {
            _idle,
            _getClose,
            _await,
            _away,
            _flinch,
            _death,
            _stumble,
            _claw,
            _tail,
            _rush,
            _lookPlayer,
        };

        // BTにビヘイビアを登録。これがないと動かない
        _tree.ResistBehaviours(behaviours);
    }

    /// <summary>
    /// BTにトランジションを登録
    /// </summary>
    private void MakeTransitions()
    {
        _tree.MakeTransition(_idle, _getClose, _tNameStartGetClose);
        _tree.MakeTransition(_getClose, _await, _tNameStartThink);
    }

    private void SetupComponent()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        _healthPoint = _healthMaxValue;

        if (_tree.CurrentBehaviour != _idle)
        {
            _tree.JumpTo(_idle); // 初期化時のビヘイビアがIdle出ない場合にはIdleにジャンプしてIdleから遷移していくようにする
        }
    }

    private void ResetAgentPath() => _agent.ResetPath();

    /// <summary>
    /// プレイヤのトランスフォームを取得する
    /// </summary>
    private Transform PlayerPR => GameObject.FindWithTag("Player").transform;

    private void FindPlayerDirection()
    {
        _player = PlayerPR;
        _playerDirection = _player.position - transform.position;
    }

    public void GetFinch() // 怯み発生時のメソッド
    {
        _tree.YieldAllBehaviourTo(_flinch);
        _anim.SetTrigger("GetFlinch");
    }

    public void NotifyEndFlinchMotion()
    {
        _tree.EndYieldBehaviourFrom(_flinch);
    }

    public void GetParry() // パリィ発生時のメソッド
    {
        _tree.YieldAllBehaviourTo(_stumble);
        _anim.SetTrigger("GetParry");
    }

    public void NotifyEndStumbleMotion()
    {
        _tree.EndYieldBehaviourFrom(_stumble);
    }

    private void Idle() // そのばにとどまる
    {
        if (!_agent.hasPath)
        {
            _agent.SetDestination(transform.position);
        }
        else
        {
            ResetAgentPath();
        }
    }

    private void GetClose() // プレイヤとの距離を詰める
    {
        _player = PlayerPR;
        if (_agent.destination != _player.position && !_agent.hasPath)
        {
            var rand = Random.Range(1f, 100f);
            if (rand < 50)
            {
                _anim.SetTrigger("Run");
                _agent.speed = _baseMoveSpeed * 2f;
            }
            else
            {
                _anim.SetTrigger("Walk");
                _agent.speed = _baseMoveSpeed;
            }

            _agent.SetDestination(_player.position);
        }
    }

    private void Await() // 指定した時間待ち、プレイヤの位置や相対的な向きに応じて次にとるビヘイビアへ移行する
    {
        _elapsedAwaitingTime += Time.deltaTime;

        FindPlayerDirection();
        var dot = Vector3.Dot(_playerDirection.normalized, transform.forward);
        var forwardRadian = Mathf.Cos(90f * (3f / 4f) * Mathf.Deg2Rad);
        _player = PlayerPR;

        var playerIsForward = dot > 0 && dot > forwardRadian;
        var playerIsSide = Mathf.Abs(dot) < forwardRadian;
        var rushRange = Physics.CheckSphere(transform.position, _rushAttackRange, _playerLayers);
        var tailRange = Physics.CheckSphere(transform.position, _tailAttackRange, _playerLayers);
        var clawRange = Physics.CheckSphere(transform.position, _clawAttackRange, _playerLayers);

        if (_elapsedAwaitingTime > _awaitingTime)
        {
            _elapsedAwaitingTime = 0;

            var rand = Random.Range(1f, 100f);
            _tree.EndYieldBehaviourFrom(_currentYielded);

            // 突進距離以内かつしっぽ攻撃圏外
            if (rushRange && !tailRange)
            {
                _tree.YieldAllBehaviourTo(_rush);
            }

            // ひっかき距離内かつ正面にいるとき
            if (playerIsForward && clawRange)
            {
                if (rand > 50)
                {
                    _tree.YieldAllBehaviourTo(_claw);
                }
                else
                {
                    _tree.YieldAllBehaviourTo(_tail);
                }
            }

            if (playerIsForward && tailRange)
            {
                _tree.YieldAllBehaviourTo(_tail);
            }

            if (playerIsSide)
            {
                _tree.YieldAllBehaviourTo(_lookPlayer);
            }

            Random.InitState(Random.Range(0, 128));
        }
    }


    private void Death()
    {
        _currentYielded = _death;
        GameObject.FindAnyObjectByType<GameLogic>().NotifyBossIsDeath();
    }

    private void Flinch() // 攻撃を受け続けてひるみ値がたまり切った際のイベント
    {
        _currentYielded = _flinch;

        _elapsedFlinchingTime += Time.deltaTime;

        if (_elapsedFlinchingTime > _awaitTimeOnFlinching)
        {
            _elapsedFlinchingTime = 0;
            _flinchPoint = 0;
            _anim.SetTrigger("EndFlinch");
        }
    }

    private void Stumble() // パリィ発生時のイベント
    {
        _currentYielded = _stumble;
        _elapsedStumbleingTime += Time.deltaTime;

        if (_elapsedStumbleingTime > _awaitTimeOnStumble)
        {
            _elapsedStumbleingTime = 0;
            _anim.SetTrigger("EndParry");
        }
    }

    private void Rush() // 突進攻撃
    {
        _currentYielded = _rush;

        if (_agent.destination != _player.position && !_agent.hasPath)
        {
            _agent.speed = _baseMoveSpeed * 3f;
            _agent.SetDestination(_player.position);
        }
        else if (_agent.hasPath)
        {
            if (DistanceFromPlayer < _clawAttackRange)
            {
                _agent.ResetPath();
                _tree.EndYieldBehaviourFrom(_rush); // _currentYielded -> _rush
                _tree.JumpTo(_await);
            }
        }
    }

    private void Tail() // しっぽ攻撃
    {
        _currentYielded = _tail;

        if (_agent.destination != transform.position && !_agent.hasPath)
        {
            _agent.SetDestination(transform.position);
        }
    }

    private void Claw() // ひっかき攻撃
    {
        _currentYielded = _claw;

        if (_agent.destination != transform.position && !_agent.hasPath)
        {
            _agent.SetDestination(transform.position);
        }
    }

    private void LookPlayer() // プレイヤを向く
    {
        _currentYielded = _lookPlayer;

        var deltaRotation = Vector3.Lerp(transform.forward, _playerDirection, Time.deltaTime);
        transform.forward += deltaRotation;

        var dot = Vector3.Dot(_playerDirection.normalized, transform.forward);
        var forwardRadian = Mathf.Cos(90f / 4f * Mathf.Deg2Rad);
        var playerIsForward = dot > 0 && dot > forwardRadian;

        if (playerIsForward)
        {
            _tree.EndYieldBehaviourFrom(_lookPlayer);
        }
    }

    public void OnDisable()
    {
        _tree.PauseBT();
    }
}

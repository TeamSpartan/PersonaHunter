using System;
using SgLibUnite.Singleton;
using SgLibUnite.CodingBooster;
using UnityEngine;
using UnityEngine.Rendering;

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : SingletonBaseClass<GameLogic>
        , IGuardEventHandler
        , IEnemyAttackEventHandler
{
    [SerializeField] private bool _debugging;

    private GameInfo _info;

    public Action EParrySucceed { get; set; }

    private Volume _volume;
    private DifferenceOfGaussian _dog;

    // コーディング ブースタ クラス
    CBooster booster = new CBooster();

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        var targets = booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.StartDull());

        // var player = GameObject.FindWithTag("Player").transform;
        // var ppos = Camera.main.WorldToViewportPoint(player.position);
        // _dog.center.Override(new Vector2(ppos.x, ppos.y));
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        var targets = booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.EndDull());
    }

    /// <summary>
    /// プレイヤーが呼び出す
    /// </summary>
    public void NotifyPlayerIsGuarding(GameObject enemy)
    {
    }

    /// <summary>
    /// 敵が呼び出す
    /// </summary>
    public void NotifyEnemyAttackCondition(GameObject instance, bool condition)
    {
    }

    void InitializeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Initialized");
        }

        _info = GameObject.FindFirstObjectByType<GameInfo>();

        var targets = booster.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.InitializeThisComponent());
        
        // _volume = GameObject.FindFirstObjectByType<Volume>();
        // _volume.profile.TryGet(out _dog);
    }

    void GameLoop()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Is Running");
        }
    }

    void FinalizeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Finalized");
        }

        var targets = booster.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.FinalizeThisComponent());
    }

    protected override void ToDoAtAwakeSingleton()
    {
    }

    private void Start()
    {
        InitializeGame();
    }

    private void FixedUpdate()
    {
        GameLoop();
    }

    private void OnApplicationQuit()
    {
        FinalizeGame();
    }
}

using System;
using SgLibUnite.Singleton;
using SgLibUnite.CodingBooster;
using UnityEngine;

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
    
    // コーディング ブースタ クラス
    CBooster booster = new CBooster();

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        var targets = booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.StartDull());
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
        throw new NotImplementedException();
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

using SgLibUnite.Singleton;
using SgLibUnite.CodingBooster;
using UnityEngine;

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : SingletonBaseClass<GameLogic>
{
    [SerializeField] private bool _debugging;

    private GameInfo _info;

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        var booster = new CBooster();
        var targets = booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.StartDull());
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        var booster = new CBooster();
        var targets = booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.EndDull());
    }

    void InitializeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Initialized");
        }
        
        _info = GameObject.FindFirstObjectByType<GameInfo>();

        var obj = new CBooster();
        var targets = obj.GetDerivedComponents<IInitializableComponent>();
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
        
        var obj = new CBooster();
        var targets = obj.GetDerivedComponents<IInitializableComponent>();
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

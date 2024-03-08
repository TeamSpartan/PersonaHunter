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
        var obj = new CBooster();
        var targets = obj.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.StartDull());
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        var obj = new CBooster();
        var targets = obj.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.EndDull());
    }

    void InitializeGame()
    {
        _info = GameObject.FindFirstObjectByType<GameInfo>();

        var obj = new CBooster();
        var targets = obj.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.InitializeThisComponent());

        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Initialized");
        }
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
        var obj = new CBooster();
        var targets = obj.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.FinalizeThisComponent());

        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Finalized");
        }
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

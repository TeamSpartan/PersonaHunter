using System.Linq;
using SgLibUnite.Singleton;
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

    void InitializeGame()
    {
        _info = GameObject.FindFirstObjectByType<GameInfo>();

        GameObject.FindObjectsOfType<GameObject>()
            .Where(_ => _.GetComponent<IInitializableComponent>() != null)
            .Select(_ => _.GetComponent<IInitializableComponent>())
            .ToList().ForEach(_ => _.InitializeThisComponent());
        
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
        GameObject.FindObjectsOfType<GameObject>()
            .Where(_ => _.GetComponent<IInitializableComponent>() != null)
            .Select(_ => _.GetComponent<IInitializableComponent>())
            .ToList().ForEach(_ => _.FinalizeThisComponent());

        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Finalized");
        }
    }

    protected override void ToDoAtAwakeSingleton()
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
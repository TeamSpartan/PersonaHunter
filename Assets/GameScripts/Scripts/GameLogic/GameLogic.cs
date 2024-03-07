using System;
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

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        GameObject.FindObjectsOfType<GameObject>()
            .Where(_ => _.GetComponent<IDulledTarget>() != null)
            .Select(_ => _.GetComponent<IDulledTarget>())
            .ToList().ForEach(_ => _.StartDull());
    }
    
    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        GameObject.FindObjectsOfType<GameObject>()
            .Where(_ => _.GetComponent<IDulledTarget>() != null)
            .Select(_ => _.GetComponent<IDulledTarget>())
            .ToList().ForEach(_ => _.EndDull());
    }

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
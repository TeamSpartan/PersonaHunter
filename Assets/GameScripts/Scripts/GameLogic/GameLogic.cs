using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
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
            .ToList().ForEach(_ => _.FinalizeThisComp());

        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Finalized");
        }
    }

    private void Awake()
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
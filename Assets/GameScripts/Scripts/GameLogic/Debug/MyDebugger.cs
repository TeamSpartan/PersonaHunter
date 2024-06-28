using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyDebugger : MonoBehaviour
{
    [SerializeField] private bool _debugging;

    private void Start()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name == ConstantValues.TitleScene)
        {
            var data = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
            if (_debugging)
                data.NotifyPlayedPrologue();
        }
    }
}

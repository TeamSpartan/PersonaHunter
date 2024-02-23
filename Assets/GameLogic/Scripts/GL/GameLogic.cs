using System;
using System.Linq;
using UnityEngine;

public class GameLogic
    : MonoBehaviour
{
    private void Awake()
    {
        throw new NotImplementedException();
    }

    private void InitializeGame()
    {
        #region Initialize All Initializable User Defined Components

        var c = GameObject.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None).ToList();
        var initializableObj
            = c.Where(obj => (IInitializableComponent)obj != null).ToList()
                .Select(_ => (IInitializableComponent)_).ToList();
        foreach (var obj in initializableObj)
        {
            obj.InitializeThisComp();
        }

        #endregion
    }

    private void FinalizeGame()
    {
        #region Finalize All Initializable User Defined Components

        var c = GameObject.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None).ToList();
        var initializableObj
            = c.Where(obj => (IInitializableComponent)obj != null).ToList()
                .Select(_ => (IInitializableComponent)_).ToList();
        foreach (var obj in initializableObj)
        {
            obj.FinalizeThisComp();
        }

        #endregion
    }
}
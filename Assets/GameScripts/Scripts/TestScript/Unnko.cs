using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using PlayerCam.Scripts;
using UnityEngine;

public class Unnko
    : MonoBehaviour
        , IInitializableComponent
        , IPlayerCameraTrasable
{
    private TesterInput _inputT;

    public void InitializeThisComponent()
    {
        Debug.Log($"{nameof(Unnko)} : Is Init");

        this._inputT = GameObject.FindFirstObjectByType<TesterInput>();
    }

    private void FixedUpdate()
    {
        var brain = GameObject.FindFirstObjectByType<PlayerCameraBrain>();
        var f = Camera.main.transform.forward;
        var r = Camera.main.transform.right;
        var dir = _inputT.GetMoveValue().x * f
                  + _inputT.GetMoveValue().y * r;
        if (!brain.LockingOn)
        {
            transform.Translate(dir);
        }
    }

    public void FinalizeThisComponent()
    {
        Debug.Log($"{nameof(Unnko)} : Is Finalized");
    }

    public Transform GetPlayerCamTrasableTransform()
    {
        return transform;
    }
}

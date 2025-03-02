using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using PlayerCam.Scripts;
using UnityEngine;

public class Unnko
    : MonoBehaviour
        , IPlayerCameraTrasable
{
    private TesterInput _inputT;

    public void Start()
    {
        Debug.Log($"{nameof(Unnko)} : Is Init");

        this._inputT = GameObject.FindFirstObjectByType<TesterInput>();
    }

    public void FixedUpdate()
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

    public void PauseThisComponent()
    {
        throw new NotImplementedException();
    }

    public void ResumeThisComponent()
    {
        throw new NotImplementedException();
    }

    public Transform GetPlayerCamTrasableTransform()
    {
        return transform;
    }
}

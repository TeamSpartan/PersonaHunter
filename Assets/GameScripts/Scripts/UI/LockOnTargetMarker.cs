using System;
using System.Collections;
using System.Collections.Generic;
using Player.Input;
using PlayerCam.Scripts;
using UnityEngine;

/// <summary> 現在ロックオンしているロックオン対象に対してマーカーのUIを表示する機能を提供する </summary>
/// マーカーのオブジェクトに対してアタッチすること
public class LockOnTargetMarker : MonoBehaviour
{
    private PlayerCameraBrain _cameraBrain;
    private Camera _camera;
    private PlayerInputsAction _inputsAction;
    private CanvasGroup _canvasGroup; //  なにもないときには null

    private void Start()
    {
        _cameraBrain = GameObject.FindAnyObjectByType<PlayerCameraBrain>();
        _camera = _cameraBrain.gameObject.GetComponent<Camera>();
        _inputsAction = GameObject.FindAnyObjectByType<PlayerInputsAction>();

        if (TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;

            _canvasGroup = canvasGroup;
        }
        else
        {
            _canvasGroup = null;
        }
    }

    private void Update()
    {
        if (_cameraBrain.LockingOn
            && _cameraBrain.CurrentLockingOnTarget is not null)
        {
            var screenSpace = _camera.WorldToScreenPoint(_cameraBrain.CurrentLockingOnTarget.transform.position
                                                         + Vector3.up);
            transform.position = screenSpace;
        }

        DisplayMarkerWhenLockingOn();
    }

    private void DisplayMarkerWhenLockingOn()
    {
        if (_cameraBrain.LockingOn)
        {
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1;
        }
        else
        {
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0;
        }
    }
}

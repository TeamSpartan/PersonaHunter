using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCam.Scripts;
using UnityEngine;

/// <summary>
/// コマシラに対してアタッチをする
/// </summary>
public class KomashiraHPBarManager : MonoBehaviour
{
    private InGameUIManager _inGameUI;
    private Camera _mainCam;
    private int _myIndex = -1;

    private void Start()
    {
        _mainCam = GameObject.FindAnyObjectByType<PlayerCameraBrain>().gameObject.GetComponent<Camera>();
        _inGameUI = GameObject.FindAnyObjectByType<InGameUIManager>();
        _myIndex = _inGameUI.AddHPBar();
    }

    private void Update()
    {
        _inGameUI.UpdateBarPos(_myIndex, _mainCam.WorldToScreenPoint(transform.position));
        Debug.Log($"My Index ; {_myIndex}");
    }
}

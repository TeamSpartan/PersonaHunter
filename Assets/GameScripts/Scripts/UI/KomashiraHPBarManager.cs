using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCam.Scripts;
using UnityEngine;

/// <summary>
/// コマシラのHPバーに対してアタッチをする
/// </summary>
public class KomashiraHPBarManager : MonoBehaviour
{
    private InGameUIManager inGameUI;
    private KomashiraBrain _komashira;
    private Camera _mainCam;
    private int _myIndex = -1;

    public int Myindex => _myIndex;

    private void Start()
    {
        inGameUI = GameObject.FindAnyObjectByType<InGameUIManager>();
        _komashira = transform.root.gameObject.GetComponent<KomashiraBrain>();
        _mainCam = GameObject.FindAnyObjectByType<PlayerCameraBrain>().gameObject.GetComponent<Camera>();
        _myIndex = inGameUI.AddHPBar(_komashira.GetMaxHealthPoint, InGameUIManager.HPBarTemplate.Komashira);
    }

    private void Update()
    {
        inGameUI.UpdateHPBar(_myIndex, _mainCam.WorldToScreenPoint(transform.position), _komashira.GetHealthPoint);
    }
}

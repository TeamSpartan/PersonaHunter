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
    private KomashiraUIManager _komashiraUI;
    private KomashiraBrain _komashira;
    private Camera _mainCam;
    private int _myIndex = -1;

    public int Myindex => _myIndex;

    private void Start()
    {
        _mainCam = GameObject.FindAnyObjectByType<PlayerCameraBrain>().gameObject.GetComponent<Camera>();
        _komashiraUI = GameObject.FindAnyObjectByType<KomashiraUIManager>();
        _komashira = GameObject.FindAnyObjectByType<KomashiraBrain>();
        _myIndex = _komashiraUI.AddHPBar(_komashira.GetMaxHealthPoint);
    }

    private void Update()
    {
        _komashiraUI.UpdateHPBar(_myIndex, _mainCam.WorldToScreenPoint(transform.position), _komashira.GetHealthPoint);
    }
}

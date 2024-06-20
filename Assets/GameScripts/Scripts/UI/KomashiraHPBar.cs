using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCam.Scripts;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// コマシラのHPバーに対してアタッチをする
/// </summary>
public class KomashiraHPBar : MonoBehaviour
{
    private Transform _followingTarget = null;
    private Camera _mainCam;
    private InGameUIManager _ingameUI;
    private CanvasGroup _canvasGroup;

    public void PunchGuage()
    {
        transform.DOShakePosition(.5f,100);
    }

    public void SetFollowingTarget(Transform t)
    {
        _followingTarget = t;
    }

    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }

    private void Start()
    {
        _mainCam = GameObject.FindAnyObjectByType<PlayerCameraBrain>().GetComponent<Camera>();
        _ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>();
        _ingameUI.AddUIElements(this.gameObject);
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if (_followingTarget is not null)
        {
            transform.position = _mainCam.WorldToScreenPoint(_followingTarget.position);
            _canvasGroup.alpha = transform.position.z > 0 ? 1 : 0;
        }
    }
}

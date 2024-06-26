using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCam.Scripts;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// コマシラのHPバーに対してアタッチをする
/// </summary>
public class KomashiraHPBar : MonoBehaviour
{
    private Transform _followingTarget = null;
    private Camera _mainCam;
    private InGameUIManager _ingameUI;
    private CanvasGroup _canvasGroup;

    public Transform FollowingTarget => _followingTarget;

    public void PunchGuage()
    {
        transform.DOShakePosition(.5f, 100);
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
        if (TryGetComponent<Slider>(out var slider))
        {
            slider.interactable = false;
        }

        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
    }

    private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg0.name == ConstantValues.InGameScene)
        {
            if (transform.root.CompareTag("PlayerUI"))
            {
                SceneManager.MoveGameObjectToScene(gameObject, arg1);
            }

            DestroySelf();
        }
    }

    private void Update()
    {
        if (_mainCam is null)
        {
            _mainCam = Camera.main;
        }

        if (_followingTarget is not null)
        {
            transform.position = _mainCam.WorldToScreenPoint(_followingTarget.position);
            _canvasGroup.alpha = transform.position.z > 0 ? 1 : 0;
        }
    }
}

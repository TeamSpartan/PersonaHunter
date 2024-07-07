using System;
using System.Collections;
using System.Collections.Generic;
using SgLibUnite.Singleton;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

// シングルトン だめ

/// <summary>
/// インゲームのUIに対してアタッチをする
/// </summary>
public class InGameUIManager : WindowManager
{
    [SerializeField] private CanvasGroup _bossHPBar;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _firstSelectedOnPause;

    public void MakeActiveElements(GameObject obj)
    {
        var group = obj.GetComponent<CanvasGroup>();
        group.interactable = group.blocksRaycasts = true;
        group.alpha = 1;
    }

    public void MakePassiveElements(GameObject obj)
    {
        var group = obj.GetComponent<CanvasGroup>();
        group.interactable = group.blocksRaycasts = false;
        group.alpha = 0;
    }

    public void AddUIElements(GameObject elem)
    {
        elem.transform.SetParent(transform);
    }

    public void BossHPBarSetActive(bool c)
    {
        _bossHPBar.alpha = c ? 1 : 0;
    }

    public void DisplayPausingPanel()
    {
        if (_pausePanel.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            Debug.Log($"ポーズ");
            canvasGroup.alpha = 1;
            canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        }
    }

    public void ClosePausingPanel()
    {
        if (_pausePanel.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            Debug.Log($"ポーズ おわり");
            canvasGroup.alpha = 0;
            canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
        }
    }

    private void OnEnable()
    {
        base.EventAtStart += ToDoOnStart;
    }

    private void OnDisable()
    {
        base.EventAtStart -= ToDoOnStart;
    }

    public void ToDoOnStart()
    {
        var gl = GameObject.FindAnyObjectByType<MainGameLoop>();
        gl.EPause += () =>
        {
            GameObject.FindAnyObjectByType<EventSystem>().SetSelectedGameObject(_firstSelectedOnPause);
        };
        gl.EResume += () => { GameObject.FindAnyObjectByType<EventSystem>().SetSelectedGameObject(null); };
    }
}

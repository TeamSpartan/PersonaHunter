using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// インゲームのUIに対してアタッチをする
/// </summary>
public class InGameUIManager : WindowManager
{
    [SerializeField] private CanvasGroup _bossHPBar;
    [SerializeField] private GameObject _pausePanel;
    
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
            canvasGroup.alpha = 1;
        }
    }

    public void ClosePausingPanel()
    {
        if (_pausePanel.TryGetComponent<CanvasGroup>(out var canvasGroup))
        {
            canvasGroup.alpha = 0;
        }
    }
}

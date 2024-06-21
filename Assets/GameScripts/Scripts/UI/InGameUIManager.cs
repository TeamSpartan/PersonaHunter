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
public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup _bossHPBar;
    
    public void AddUIElements(GameObject elem)
    {
        elem.transform.SetParent(transform);
    }

    public void BossHPBarSetActive(bool c)
    {
        _bossHPBar.alpha = c ? 1 : 0;
    }
}

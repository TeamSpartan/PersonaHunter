using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// インゲームのUIに対してアタッチをする
/// </summary>
public class InGameUIManager : MonoBehaviour
{
    public void AddUIElements(GameObject elem)
    {
        elem.transform.SetParent(transform);
    }
}

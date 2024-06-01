using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* 作成 【菅沼】 */

/// <summary>
/// UIの管理をする
/// </summary>
public class WindowManager : MonoBehaviour
{
    [SerializeField] private GameObject _rootWindow;

    public void CloseWindow(GameObject obj)
    {
        obj.GetComponentInChildren<Canvas>().sortingOrder = -32;
        var g = obj.GetComponentInChildren<CanvasGroup>();
            g.alpha = 0f;
            g.interactable = false;
        obj.transform.SetAsFirstSibling();
    }

    public void OpenWindow(GameObject obj)
    {
        obj.GetComponentInChildren<Canvas>().sortingOrder = 0;
        var g = obj.GetComponentInChildren<CanvasGroup>();
        g.alpha = 1f;
        g.interactable = true;
        obj.transform.SetAsLastSibling();
    }

    private void Start()
    {
        OpenWindow(_rootWindow);
    }
}

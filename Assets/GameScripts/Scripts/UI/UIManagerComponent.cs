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
public class UIManagerComponent : MonoBehaviour
{
    [SerializeField] private GameObject _rootWindow;

    public void CloseWindow(GameObject obj)
    {
        obj.GetComponentInChildren<Canvas>().sortingOrder = -32;
        obj.GetComponentInChildren<CanvasGroup>().alpha = 0f;
        obj.transform.SetAsFirstSibling();
    }

    public void OpenWindow(GameObject obj)
    {
        obj.GetComponentInChildren<Canvas>().sortingOrder = 0;
        obj.GetComponentInChildren<CanvasGroup>().alpha = 1;
        obj.transform.SetAsLastSibling();
    }

    private void Start()
    {
        OpenWindow(_rootWindow);
    }
}

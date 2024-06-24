using System;
using System.Collections.Generic;
using UnityEngine;

/* 作成 【菅沼】 */

/// <summary>
/// UIの管理をする。 タイトル画面のUIを管理する
/// </summary>
public class WindowManager : MonoBehaviour
{
    [SerializeField] private GameObject _rootWindow;
    [SerializeField] private List<GameObject> _invisibleWhenStarted;
    public GameObject RootWindow => _rootWindow;

    protected Action EventAtStart;

    public void CloseWindow(GameObject obj)
    {
        obj.GetComponentInChildren<Canvas>().sortingOrder = -32;
        var g = obj.GetComponentInChildren<CanvasGroup>();
        g.alpha = 0f;
        g.interactable = g.blocksRaycasts = false;
        obj.transform.SetAsFirstSibling();
    }

    public void OpenWindow(GameObject obj)
    {
        obj.GetComponentInChildren<Canvas>().sortingOrder = 0;
        var g = obj.GetComponentInChildren<CanvasGroup>();
        g.alpha = 1f;
        g.interactable = g.blocksRaycasts = true;
        obj.transform.SetAsLastSibling();
    }

    private void Start()
    {
        var tempData = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
        if (tempData.PlayedPrologue)
        {
            OpenWindow(_rootWindow);
        }

        foreach (var obj in _invisibleWhenStarted)
        {
            if (obj.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup.alpha = 0;
            }
            else
            {
                obj.GetComponentInChildren<CanvasGroup>().alpha = 0;
            }
        }

        if (EventAtStart is not null)
            EventAtStart();
    }
}

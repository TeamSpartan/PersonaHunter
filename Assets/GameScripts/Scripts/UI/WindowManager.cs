using UnityEngine;

/* 作成 【菅沼】 */

/// <summary>
/// UIの管理をする
/// </summary>
public class WindowManager : MonoBehaviour
{
    [SerializeField] private GameObject _rootWindow;
    public GameObject RootWindow => _rootWindow;

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
    }
}

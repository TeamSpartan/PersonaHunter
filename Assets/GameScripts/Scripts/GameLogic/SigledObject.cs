using UnityEngine;

/// <summary>
/// シーン内のゲームオブジェクトをDDOLへ登録してシーンに必ず１コは存在するようにする機能を提供する。
/// </summary>
public class SigledObject : MonoBehaviour
{
    private void Start()
    {
        GameObject.DontDestroyOnLoad(this.gameObject);
    }
}

using UnityEngine;

/// <summary>
/// 背景オブジェクトをカメラとの距離に応じて入れ替える
/// </summary>
public class MapObjectOptimizer : MonoBehaviour
{
    enum Group
    {
        A,
        B
    }

    [SerializeField, Header("オブジェクトグループA")]
    private GameObject _groupA;

    [SerializeField, Header("オブジェクトグループB")]
    private GameObject _groupB;

    [SerializeField, Header("差し替えをする距離。この距離より遠ければ差し替える")]
    private float _distanceToSwap;

    [SerializeField, Header("指定した距離より遠い場合に有効にするグループ")]
    private Group _targetGroup;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, _distanceToSwap);
    }

    private void LateUpdate()
    {
        // ターゲットのグループを有効化するときにTrue
        var condition = Vector3.Distance(transform.position, Camera.main.transform.position) > _distanceToSwap;

        switch (_targetGroup)
        {
            case Group.A:
            {
                _groupA.SetActive(condition);
                _groupB.SetActive(!condition);
                break;
            }
            case Group.B:
            {
                _groupA.SetActive(!condition);
                _groupB.SetActive(condition);
                break;
            }
        }
    }
}

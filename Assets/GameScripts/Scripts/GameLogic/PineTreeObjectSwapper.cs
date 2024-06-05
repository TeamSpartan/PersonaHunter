using UnityEngine;

/// <summary>
/// 背景オブジェクトをカメラとの距離に応じて入れ替える
/// </summary>
public class PineTreeObjectSwapper : MonoBehaviour
{
    [ SerializeField, Header("差し替えに使うオブジェクト")]
    private GameObject _swapper;

    [SerializeField, Header("差し替えをする距離。この距離より遠ければ差し替える")]
    private float _distanceToSwap;

    [SerializeField, Header("カメラを向きるづけるか")]
    private bool _billboard;

    [SerializeField, Header("向く対象")] private Transform _billboardtarget;

    private MeshRenderer _meshRenderer;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, _distanceToSwap);
    }

    private void OnDrawGizmos()
    {
    }

    private void Start()
    {
        if (_billboardtarget is null)
        {
            _billboardtarget = Camera.main.transform;
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
        
        _swapper = Instantiate(_swapper, transform.position, transform.rotation);
    }

    private void LateUpdate()
    {
        // 素材入れ替え距離内にいる場合には True その外にいる場合にはSwapperの素材をアクティブにする
        var cond = _distanceToSwap > Vector3.Distance(_billboardtarget.transform.position, transform.position);
        _meshRenderer.enabled = cond;
        _swapper.SetActive(!cond);
        if (!cond)
        {
            var dir = (_billboardtarget.transform.position - transform.position).normalized;
            dir.y = 0; // y軸回転のみしてほしいから
            _swapper.transform.forward = dir;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using SgLibUnite.Systems;

/* シングルトンやーめた。万能ではない */

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : MonoBehaviour
        , IBossDieNotifiable
{
    #region Acitons

    /// <summary> パリィ成功時のイベント </summary>
    public Action EParrySucceed;

    /// <summary> 一時停止処理が走った時の処理 </summary>
    public Action EPause;

    /// <summary> 一時停止から抜けるときに走る処理 </summary>
    public Action EResume;

    #endregion

    /// <summary> ガウス差分クラス </summary>
    private DifferenceOfGaussian _dog;

    /// <summary> ポスプロをかけるために必要なVolumeクラス </summary>
    private Volume _volume;

    /// <summary> シーン遷移クラス </summary>
    private SceneLoader _sceneLoader;

    /// <summary> シーンのインデックス </summary>
    private int _selectedSceneIndex;

    /// <summary> 敵のトランスフォーム </summary>
    private List<Transform> _enemies = new List<Transform>();

    public event Action TaskOnBossDefeated;

    private void OnEnable()
    {
        Initialize();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 敵のトランスフォームを登録
    /// </summary>
    public void ApplyEnemyTransform(Transform enemy)
    {
        _enemies.Add(enemy);
    }

    /// <summary>
    /// 敵を取得する
    /// </summary>
    public List<Transform> GetEnemies()
    {
        return _enemies;
    }

    private void StartPostProDoG()
    {
        if (GameObject.FindWithTag("Player").transform is not null)
        {
            var player = GameObject.FindWithTag("Player").transform;
            var playerPos = Camera.main.WorldToViewportPoint(player.position);
            _dog.center.Override(new Vector2(playerPos.x, playerPos.y));
        }
        else
        {
            _dog.center.Override(Vector2.one * .5f);
        }

        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            0f, 1f, .75f);
    }

    /// <summary>
    /// 一時停止を開始する
    /// </summary>
    public void StartPause()
    {
        EPause();

        Debug.Log($"GL ポーズ開始");
    }

    /// <summary>
    /// 一時停止を終了する
    /// </summary>
    public void StartResume()
    {
        EResume();

        Debug.Log($"GL ポーズおーわり");
    }

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        foreach (var brain in GameObject.FindObjectsByType<KomashiraBrain>(FindObjectsSortMode.None))
        {
            brain.StartDull();
        }

        // StartPostProDoG();
        Debug.Log($"GL ときとめ 開始");
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        foreach (var brain in GameObject.FindObjectsByType<KomashiraBrain>(FindObjectsSortMode.None))
        {
            brain.EndDull();
        }
        
        Debug.Log($"GL ときとめ 終了");
    }

    public void NotifyBossIsDeath()
    {
        TaskOnBossDefeated();
    }

    public void Initialize()
    {
        // SceneLoader が Nullである場合には生成。
        if (GameObject.FindFirstObjectByType<SceneLoader>() is null)
        {
            var sl = Resources.Load<GameObject>("Prefabs/GameSystem/SceneLoader");

            var obj = GameObject.Instantiate(sl);
            _sceneLoader = obj.GetComponent<SceneLoader>();
        }

        if (GameObject.FindFirstObjectByType<Volume>() is not null)
            // GameObject.FindFirstObjectByType<Volume>() != null
        {
            _volume = GameObject.FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _dog);
        }
    }
}

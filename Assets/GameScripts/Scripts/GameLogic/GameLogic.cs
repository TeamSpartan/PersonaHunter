using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using SgLibUnite.Systems;
using UnityEngine.SceneManagement;

/* シングルトンやーめた。万能ではない */

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : MonoBehaviour
        , IBossDieNotifiable
{
    /// <summary> パリィ成功時のイベント </summary>
    public event Action EParrySucceed;

    /// <summary> 一時停止処理が走った時の処理 </summary>
    public event Action EPause;

    /// <summary> 一時停止から抜けるときに走る処理 </summary>
    public event Action EResume;

    /// <summary> ゾーンに入るときのイベントをここに登録 </summary>
    public event Action EDiveZone;

    /// <summary> ゾーンから出るときのイベントをここに登録 </summary>
    public event Action EOutZone;

    /// <summary> ガウス差分クラス </summary>
    private DifferenceOfGaussian _dog;

    /// <summary> ポスプロをかけるために必要なVolumeクラス </summary>
    private Volume _volume;

    /// <summary> シーン遷移クラス </summary>
    private SceneLoader _sceneLoader;

    /// <summary> シーンのインデックス </summary>
    private int _selectedSceneIndex;

    #region Flags

    /// <summary> プロローグ再生完了のフラグ </summary>
    private bool _playedPrologue;

    #endregion

    /// <summary> 敵のトランスフォーム </summary>
    private List<Transform> _enemies = new List<Transform>();

    private void Awake()
    {
        // SceneLoader が Nullである場合には生成。
        if (GameObject.FindFirstObjectByType<SceneLoader>() is null)
        {
            var obj = Resources.Load("Prefabs/GameSystem/SceneLoader") as GameObject;

            GameObject.Instantiate(obj);
        }
    }

    private void OnEnable()
    {
        // SceneLoader が Nullである場合には生成。
        if (GameObject.FindFirstObjectByType<SceneLoader>() is null)
        {
            var obj = Resources.Load("Prefabs/GameSystem/SceneLoader") as GameObject;

            GameObject.Instantiate(obj);
        }
    }

    private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg0.name is ConstantValues.BossScene || arg0.name is ConstantValues.InGameScene)
        {
            // プレイヤとUIを破棄する
            Destroy(GameObject.FindWithTag("PlayerUI"));
            Destroy(GameObject.FindWithTag("Player"));
        }
    }

    private void Start()
    {
        InitializeGame();
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
        if (EPause is not null) EPause.Invoke();
    }

    /// <summary>
    /// 一時停止を終了する
    /// </summary>
    public void StartResume()
    {
        if (EResume is not null) EResume.Invoke();
    }

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        if (EDiveZone is not null) EDiveZone.Invoke();

        StartPostProDoG();
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        if (EOutZone is not null) EOutZone.Invoke();
    }

    public void NotifyBossIsDeath()
    {
    }

    private void InitializeGame()
    {
        if (GameObject.FindFirstObjectByType<Volume>() is not null)
            // GameObject.FindFirstObjectByType<Volume>() != null
        {
            _volume = GameObject.FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _dog);
        }

        var scene = SceneManager.GetActiveScene();
    }
}

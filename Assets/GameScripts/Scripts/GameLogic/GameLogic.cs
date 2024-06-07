using System;
using SgLibUnite.CodingBooster;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using SgLibUnite.Systems;
using UnityEngine.SceneManagement;

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : MonoBehaviour
{
    [SerializeField] private bool _debugging;

    [SerializeField, Tooltip("シーンアセットの配列を格納しているアセット")]
    private SceneInfo _sceneInfo;

    /// <summary> パリィ成功時のイベント </summary>
    public Action EParrySucceed { get; set; }

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

    // コーディング ブースタ クラス
    CBooster _booster = new CBooster();

    private void Awake()
    {
        if (_debugging)
        {
            Debug.Log($"Current Scene Index : {CheckSceneIndex(SceneManager.GetActiveScene())}");
        }

        // SceneLoader が Nullである場合には生成。
        if (GameObject.FindFirstObjectByType<SceneLoader>() is null)
        {
            var obj = Resources.Load("Prefabs/GameSystem/SceneLoader") as GameObject;

            GameObject.Instantiate(obj);
        }
    }

    private void Start()
    {
        InitializeGame();
    }

    private void FixedUpdate()
    {
    }

    private void OnApplicationQuit()
    {
    }

    public void TaskOnDivedOnZone()
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
    }

    /// <summary>
    /// 一時停止を終了する
    /// </summary>
    public void StartResume()
    {
    }

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        var targets = _booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.StartDull());
        TaskOnDivedOnZone();
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        var targets = _booster.GetDerivedComponents<IDulledTarget>();
        targets.ForEach(_ => _.EndDull());
    }

    public int CheckSceneIndex(Scene scene)
    {
        return _sceneInfo.MasterScenes.FindIndex(_ => _.name == scene.name);
    }

    private void InitializeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Initialized");
        }

        if (GameObject
                .FindFirstObjectByType<Volume>() is not null) // GameObject.FindFirstObjectByType<Volume>() != null
        {
            _volume = GameObject.FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _dog);
        }
    }
}

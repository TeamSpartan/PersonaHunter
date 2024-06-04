using System;
using System.Collections.Generic;
using SgLibUnite.Singleton;
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
    : SingletonBaseClass<GameLogic>
{
    [SerializeField] private bool _debugging;
    [SerializeField] private SceneInfo _sceneInfo;

    public Action EParrySucceed { get; set; }

    private List<AsyncOperation> _asyncOperation = new List<AsyncOperation>();
    private DifferenceOfGaussian _dog;
    private Volume _volume;
    private SceneLoader _sceneLoader;
    private int _selectedSceneIndex;

    #region Flags

    private bool _playedPrologue;

    #endregion

    // コーディング ブースタ クラス
    CBooster _booster = new CBooster();

    protected override void ToDoAtAwakeSingleton()
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
        GameLoop();
    }

    private void OnApplicationQuit()
    {
        FinalizeGame();
    }

    public void TaskOnDivedOnZone()
    {
        if (GameObject.FindWithTag("Player").transform != null)
        {
            var player = GameObject.FindWithTag("Player").transform;
            var ppos = Camera.main.WorldToViewportPoint(player.position);
            _dog.center.Override(new Vector2(ppos.x, ppos.y));
        }
        else
        {
            _dog.center.Override(Vector2.one * .5f);
        }

        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            0f, 1f, .75f);
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

    private void InitializeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Initialized");
        }

        var targets = _booster.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.InitializeThisComponent());

        if (GameObject.FindFirstObjectByType<Volume>() != null)
        {
            _volume = GameObject.FindFirstObjectByType<Volume>();
            _volume.profile.TryGet(out _dog);
        }
    }

    private void GameLoop()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Is Running");
        }

        var targ = _booster.GetDerivedComponents<IInitializableComponent>();
        targ.ForEach(_ => _.FixedTickThisComponent());
    }

    private void FinalizeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Finalized");
        }

        var targets = _booster.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.FinalizeThisComponent());
    }

    private int CheckSceneIndex(Scene scene)
    {
        return _sceneInfo.MasterScenes.FindIndex(_ => _.name == scene.name);
    }
}

using System;
using SgLibUnite.Singleton;
using SgLibUnite.CodingBooster;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : SingletonBaseClass<GameLogic>
{
    [SerializeField] private bool _debugging;

    public Action EParrySucceed { get; set; }

    private Volume _volume;
    private DifferenceOfGaussian _dog;

    // コーディング ブースタ クラス
    CBooster _booster = new CBooster();

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

    void InitializeGame()
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

    void GameLoop()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Is Running");
        }

        var targ = _booster.GetDerivedComponents<IInitializableComponent>();
        targ.ForEach(_ => _.FixedTickThisComponent());
    }

    void FinalizeGame()
    {
        if (_debugging)
        {
            Debug.Log($"{nameof(GameLogic)}:Game Finalized");
        }

        var targets = _booster.GetDerivedComponents<IInitializableComponent>();
        targets.ForEach(_ => _.FinalizeThisComponent());
    }

    protected override void ToDoAtAwakeSingleton()
    {
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
}

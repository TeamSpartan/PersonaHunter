using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Player.Input;
using SgLibUnite.Systems;
using SgLibUnite.Singleton;
using UnityEngine.SceneManagement;

/* インゲームシーンとボスシーン間でガウス差分の
 ポスプロのボリュームの参照を保持したいのでシングルトン */

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : SingletonBaseClass<GameLogic>
        , IEnemyDieNotifiable
{
    #region Acitons

    /// <summary> パリィ成功時のイベント </summary>
    public event Action EParrySucceed;

    public event Action TaskOnBossDefeated;

    /// <summary> システム面でのイベントをここへ登録。
    /// 画面ポーズが走ったらこれを呼び出す </summary>
    public event Action EPause;

    /// <summary> システム面でのイベントをここへ登録。
    /// 画面リジュームが走ったらこれを呼び出す </summary>
    public event Action EResume;

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


    private InGameUIManager _ingameUI;

    private PlayerInputsAction _playerInputs;

    protected override void ToDoAtAwakeSingleton()
    {
    }

    private void Start()
    {
        Initialize();

        _ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>();
        _playerInputs = GameObject.FindAnyObjectByType<PlayerInputsAction>();

        SceneManager.activeSceneChanged += (arg0, scene) =>
        {
            _enemies.Clear();
        };
    }

    private void Update()
    {
        _dog.center.Override(Vector2.one * .5f);
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

    public void StartPostProDoG()
    {
        Debug.Log($"ガウス さっぶーーーーん");

        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            0f, 1f, .75f);
    }

    /// <summary>
    /// 一時停止を開始する
    /// </summary>
    public void StartPause()
    {
        EPause();
        
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.TryGetComponent<KomashiraBrain>(out var komashira))
            {
                komashira.StartFreeze();
            }

            if (enemy.gameObject.TryGetComponent<NuweBrain>(out var nue))
            {
                nue.StartFreeze();
            }
        }
        
        _ingameUI.DisplayPausingPanel();
    }

    /// <summary>
    /// 一時停止を終了する
    /// </summary>
    public void StartResume()
    {
        EResume();
        
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.TryGetComponent<KomashiraBrain>(out var komashira))
            {
                komashira.EndFreeze();
            }

            if (enemy.gameObject.TryGetComponent<NuweBrain>(out var nue))
            {
                nue.EndFreeze();
            }
        }
        
        _ingameUI.ClosePausingPanel();
    }

    /// <summary>
    /// 集中 を 発火する
    /// </summary>
    public void StartDiveInZone()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.TryGetComponent<KomashiraBrain>(out var komashira))
            {
                komashira.StartFreeze();
            }

            if (enemy.gameObject.TryGetComponent<NuweBrain>(out var nue))
            {
                nue.StartFreeze();
            }
        }
        
        StartPostProDoG();
    }

    /// <summary>
    /// 集中 を 収束する
    /// </summary>
    public void GetOutOverZone()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy.gameObject.TryGetComponent<KomashiraBrain>(out var komashira))
            {
                komashira.EndFreeze();
            }

            if (enemy.gameObject.TryGetComponent<NuweBrain>(out var nue))
            {
                nue.EndFreeze();
            }
        }
        
        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            1f, 0f, .75f);
    }

    public void NotifyEnemyIsDeath(IEnemyDieNotifiable.EnemyType type, GameObject enemy)
    {
        switch (type)
        {
            case IEnemyDieNotifiable.EnemyType.Nue:
                TaskOnBossDefeated();
                break;
            
            case IEnemyDieNotifiable.EnemyType.Komashira:
                break;
        }
    }

    public void Initialize()
    {
        // SceneLoader が Nullである場合には生成。
        if (GameObject.FindFirstObjectByType<SceneLoader>() is null)
        {
            var sceneLoader = Resources.Load<GameObject>("Prefabs/GameSystem/SceneLoader");

            var obj = GameObject.Instantiate(sceneLoader);
            _sceneLoader = obj.GetComponent<SceneLoader>();
        }

        if (GameObject.FindAnyObjectByType<Volume>() is not null)
        {
            _volume = GameObject.FindFirstObjectByType<Volume>();
            if (_volume.profile.TryGet(out _dog))
            {
                Debug.Log($"ガウス差分クラスないんだけど");
            }
        }
        else
        {
            Debug.Log($"ボリュームねぇんだけど");
        }
    }
}

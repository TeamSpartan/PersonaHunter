using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Player.Input;
using SgLibUnite.Systems;
using SgLibUnite.Singleton;
using UnityEditor;
using UnityEngine.SceneManagement;

// コードクリーン実施 【6/26：菅沼】
// 各くそコードのリファクタ

/* インゲームシーンとボスシーン間でガウス差分の
 ポスプロのボリュームの参照を保持したいのでシングルトン */

#region 設計思想

// シングルトンパターン 適応で Title シーン から ヒエラルキーに常駐する

#endregion

// 作成：菅沼
/// <summary>
/// オモテガリ ゲームロジック
/// </summary>
public class GameLogic
    : SingletonBaseClass<GameLogic>, IEnemyDieNotifiable
{
    #region ゲームシステム構成クラス内イベント

    /// <summary> パリィ成功時のイベント </summary>
    public event Action EParrySucceed;

    /// <summary> ボスが撃破された時のタスク（処理、イベント） </summary>
    public event Action TaskOnBossDefeated;

    /// <summary> システム面でのイベントをここへ登録。
    /// 画面ポーズが走ったらこれを呼び出す </summary>
    public event Action EPause;

    /// <summary> システム面でのイベントをここへ登録。
    /// 画面リジュームが走ったらこれを呼び出す </summary>
    public event Action EResume;

    #endregion

    public bool IsPausing => _isPausing;

    /// <summary> ガウス差分クラス </summary>
    private DifferenceOfGaussian _dog;

    /// <summary> ポスプロをかけるために必要なVolumeクラス </summary>
    private Volume _volume;

    /// <summary> シーン遷移クラス </summary>
    private SceneLoader _sceneLoader;

    /// <summary> 敵のトランスフォーム </summary>
    private List<Transform> _enemies = new List<Transform>();

    /// <summary> インゲームUI管理クラス </summary>
    private InGameUIManager _ingameUI;

    /// <summary> 入力バッファクラス </summary>
    private PlayerInputsAction _playerInputs;

    // ポーズ中のフラグ
    private bool _isPausing;

    protected override void ToDoAtAwakeSingleton()
    {
    }

    private void Start() // 生成時 初期化
    {
        Initialize();
    }

    private void Update()
    {
        // ガウス差分クラスに対して毎フレーム始点のオーバーライドをする
        if (_dog is not null)
        {
            _dog.center.Override(Vector2.one * .5f);
        }
        else
        {
            GetDoGComponent();
        }
    }

    /// <summary> 敵のトランスフォームを登録 </summary>
    public void ApplyEnemyTransform(Transform enemy)
    {
        if (_enemies is null)
        {
            _enemies = new List<Transform>();
        }

        _enemies.Add(enemy);
    }

    /// <summary> 敵を取得する </summary>
    public List<Transform> GetEnemies()
    {
        return _enemies;
    }

    /// <summary> ポーズ リジューム が走る時に呼び出す </summary>
    public void PauseResumeInputFired()
    {
        _isPausing = !_isPausing;

        if (_isPausing)
        {
            StartPause();
        }
        else
        {
            StartResume();
        }
    }

    /// <summary> ガウス差分ポスプロをかける </summary>
    public void PlayDoGEffect()
    {
        DOTween.To((_) => { _dog.elapsedTime.Override(_); },
            0f, 1f, .75f);
    }

    /// <summary> 一時停止を開始する </summary>
    public void StartPause()
    {
        if (_playerInputs.PauseInputBlocked) return;

        SetInGameInputBlocked(true);

        EPause?.Invoke();

        foreach (var enemy in _enemies) // 各敵コンポーネントに対して操作
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

        if (_ingameUI is null)
        {
            _ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
            if (!_ingameUI.gameObject.activeSelf)
            {
                _ingameUI.gameObject.SetActive(true);
            }
        }

        _ingameUI.DisplayPausingPanel();
    }

    /// <summary> 一時停止を終了する </summary>
    public void StartResume()
    {
        if (_playerInputs.PauseInputBlocked) return;

        SetInGameInputBlocked(false);

        EResume?.Invoke();

        foreach (var enemy in _enemies) // 各敵コンポーネントに対して操作
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

        if (_ingameUI is null)
        {
            _ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
            if (!_ingameUI.gameObject.activeSelf)
            {
                _ingameUI.gameObject.SetActive(true);
            }
        }

        _ingameUI.ClosePausingPanel();
    }

    /// <summary> 集中 を 発火する </summary>
    public void StartDiveInZone()
    {
        if (_enemies.Count < 1 || _enemies is null) return;

        foreach (var enemy in _enemies) // 各敵コンポーネントに対して操作
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

        PlayDoGEffect();
    }

    /// <summary> 集中 を 収束する </summary>
    public void GetOutOverZone()
    {
        if (_enemies.Count < 1 || _enemies is null) return;

        foreach (var enemy in _enemies) // 各敵コンポーネントに対して操作
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
                TaskOnBossDefeated(); // ボス撃破時の システムイベントの発火
                break;

            case IEnemyDieNotifiable.EnemyType.Komashira:
                break;
        }
    }

    /// <summary> 初期化処理 </summary>
    public void Initialize() // シングルトンでシーンに常駐してるので初期化ように実装
    {
        _ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
        if (_ingameUI is not null && !_ingameUI.gameObject.activeSelf)
        {
            _ingameUI.gameObject.SetActive(true);
        }

        _playerInputs = GameObject.FindAnyObjectByType<PlayerInputsAction>(FindObjectsInactive.Include);
        if (_playerInputs is not null && !_playerInputs.gameObject.activeSelf)
        {
            _playerInputs.gameObject.SetActive(true);
        }

        SceneManager.activeSceneChanged += (arg0, scene) => { _enemies.Clear(); };

        // SceneLoader が Nullである場合には生成。
        var loader = GameObject.FindFirstObjectByType<SceneLoader>(FindObjectsInactive.Include);
        if (loader is null)
        {
            var sceneLoader = Resources.Load<GameObject>("Prefabs/GameSystem/SceneLoader");

            var obj = GameObject.Instantiate(sceneLoader);
            _sceneLoader = obj.GetComponent<SceneLoader>();
        }

        GetDoGComponent();
    }


    /// <summary> ポーズ入力のブロックをする。 condition = True でブロック </summary>
    /// <param name="condition"></param>
    public void SetPauseInputBlocked(bool condition)
    {
        _playerInputs.PauseInputBlocked = condition;
    }

    /// <summary> インゲームの入力をブロックするかコンディションを更新する </summary>
    /// <param name="blockInput"></param>
    public void SetInGameInputBlocked(bool blockInput)
    {
        if (_playerInputs is null)
        {
            _playerInputs = GameObject.FindAnyObjectByType<PlayerInputsAction>(FindObjectsInactive.Include);
            if (!_playerInputs.gameObject.activeSelf)
            {
                _playerInputs.gameObject.SetActive(true);
            }
        }

        // インゲーム入力のブロック解除
        _playerInputs.ControllerInputBlocked
            = _playerInputs.ExternalInputBlocked = blockInput;
    }

    private void GetDoGComponent()
    {
        // ガウス差分クラスの取得
        _volume = GameObject.FindFirstObjectByType<Volume>(FindObjectsInactive.Include);
        if (_volume is not null && !_volume.gameObject.activeSelf)
        {
            _volume.gameObject.SetActive(true);
            if (_volume.profile.TryGet(out DifferenceOfGaussian dog))
            {
                _dog = dog;
            }
        }
    }
}

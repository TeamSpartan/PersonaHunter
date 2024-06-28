using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Player.Action;
using Player.Input;
using Player.Param;
using PlayerCam.Scripts;
using SgLibUnite.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// コードクリーン 実施 【6/26日 ： 菅沼】
// リファクタも実施

#region 設計思想

// シングルトンパターンを適応しない
/* 各シーンのオブジェクトが参照を持っていて依存をしているため、シングルトンだめ */

#endregion

/// <summary>
/// ユーザー定義のゲーム開始時にヴァリデーション処理を受け持つクラス。
/// あくまでもシーン遷移後の処理をするのみに留める
/// </summary>
public class MyComponentValidator : MonoBehaviour
{
    // このクラスのオブジェクトが破棄されたときに同時に破棄するオブジェクトのリスト
    [SerializeField] private List<GameObject> _destroyTargetOnDestroyedThis;

    private GameObject _firstSelectedUIElemInScene;
    private ClientDataHolder _clientData;
    private GameLogic _gameLogic;
    private GameObject _player;
    private GameObject _moviePanel;
    private PlayerCameraBrain _cameraBrain;
    private InGameUIManager _inGameUIManager;
    private PlayerInputsAction _input;

    private bool _playedPrologue;
    private List<GameObject> _thisSceneOnlyObj;

    // Start is called before the first frame update
    private void Start()
    {
        Validation();
        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged_DestroyThisSceneOnlyObj;
    }

    private void SceneManagerOnactiveSceneChanged_DestroyThisSceneOnlyObj(Scene arg0, Scene arg1)
    {
        foreach (var o in _thisSceneOnlyObj)
        {
            Destroy(o);
        }
    }

    /// <summary> 各シーンに配置されたこのコンポーネントがアタッチされているオブジェクトが初期化されたタイミングで呼ばれる </summary>
    private void Validation()
    {
        _clientData = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
        _firstSelectedUIElemInScene = GameObject.FindWithTag("FirstSelectedUIElement");

        _thisSceneOnlyObj = GameObject.FindGameObjectsWithTag("ThisSceneOnly").ToList();

        _gameLogic = GameObject.FindAnyObjectByType<GameLogic>(FindObjectsInactive.Include);
        if (_gameLogic is not null && !_gameLogic.gameObject.activeSelf)
        {
            gameObject.gameObject.SetActive(true);
        }

        _cameraBrain = GameObject.FindAnyObjectByType<PlayerCameraBrain>(FindObjectsInactive.Include);
        if (_cameraBrain is not null && !_cameraBrain.gameObject.activeSelf)
        {
            _cameraBrain.gameObject.SetActive(true);
        }

        var playerMove = GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include);
        if (playerMove is not null && !playerMove.gameObject.activeSelf)
        {
            playerMove.gameObject.SetActive(true);
            _player = playerMove.gameObject;
        }

        _inGameUIManager = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
        if (_inGameUIManager is not null && !_inGameUIManager.gameObject.activeSelf)
        {
            _inGameUIManager.gameObject.SetActive(true);
        }

        _input = GameObject.FindAnyObjectByType<PlayerInputsAction>(FindObjectsInactive.Include);
        if (_input is not null && !_input.gameObject.activeSelf)
        {
            _input.gameObject.SetActive(true);
        }
        else if (_input is not null)
        {
            // 【初期値はUIの入力タイプ】
            _input.InputType = InputType.UI;
        }

        var scene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            case ConstantValues.TitleScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Title;

                ValidationOnTitleScene();
                break;
            }

            case ConstantValues.PrologueScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Prologue;

                break;
            }

            case ConstantValues.InGameScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.InGame;

                GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include).gameObject.SetActive(true);
                GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include).gameObject.SetActive(true);

                _gameLogic.Initialize();
                _cameraBrain.Initialize();
                _inGameUIManager.ToDoOnStart();
                _input.InputType = InputType.Player;

                SpawnPlayerToPoint();
                break;
            }

            case ConstantValues.BossScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.TransitedBossScene;

                ValidationOnBossScene();
                _gameLogic.Initialize();
                _cameraBrain.Initialize();
                _inGameUIManager.ToDoOnStart();
                _input.InputType = InputType.Player;

                break;
            }

            case ConstantValues.EpilogueScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Epilogue;

                _input.InputType = InputType.UI;
                Dispose_InGameObject();
                break;
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var target in _destroyTargetOnDestroyedThis)
        {
            Destroy(target);
        }
    }

    /// <summary> タイトルシーンのヴァリデーション </summary>
    private void ValidationOnTitleScene()
    {
        // プロローグを再生したかの静的フィールドにアクセス
        if (_clientData is not null)
        {
            _playedPrologue = _clientData.PlayedPrologue;
        }

        // EventSystem の選択オブジェクトを変更
        if (_clientData.PlayedPrologue)
        {
            var eventSystem = GameObject.FindAnyObjectByType<EventSystem>(); // 今アクティブなイベントシステムのクラスが欲しい
            if (eventSystem is not null)
            {
                if (_firstSelectedUIElemInScene is null)
                {
                    _firstSelectedUIElemInScene = GameObject.FindGameObjectWithTag("FirstSelectedUIElement");
                }

                eventSystem.firstSelectedGameObject = _firstSelectedUIElemInScene;
            }
        }

        // タイトル画面のバックグラウンドのオブジェクト
        var obj = GameObject.Find("TitleImageBackGround"); // ##
        if (obj is not null)
        {
            var group = obj.transform.GetComponentInChildren<CanvasGroup>();
            if (group is not null)
            {
                group.alpha = _playedPrologue ? 1 : 0;
                group.interactable = group.blocksRaycasts = _playedPrologue;
            }
        }

        // PressAnyButtonのパネル
        obj = GameObject.Find("PressAnyButtonPanel"); // ## 
        if (obj is not null)
        {
            if (_playedPrologue)
            {
                GameObject.Destroy(obj);
            }
        }
    }

    /// <summary> ボスシーンでのヴァリエーション </summary>
    private void ValidationOnBossScene()
    {
        // インゲーム入力をブロック
        _gameLogic.SetInGameInputBlocked(true);

        // ポーズ入力をブロック 【入力タイプがUIになるのを防ぐ】
        _gameLogic.SetPauseInputBlocked(true);

        var ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
        if (!ingameUI.gameObject.activeSelf)
        {
            ingameUI.BossHPBarSetActive(true);
        }

        // コマシラ のHPバーを削除
        foreach (var komashiraHpBar in GameObject.FindObjectsByType<KomashiraHPBar>(FindObjectsSortMode.None))
        {
            Destroy(komashiraHpBar.gameObject);
        }

        // プレイヤ隠ぺい
        if (_player is null)
        {
            _player = GameObject.FindWithTag("Player");
        }

        _player.SetActive(false);

        // ムービー読み込み
        var appearanceMovieGO = Resources.Load<GameObject>("Prefabs/Video/BossAppearance");

        /* 例外がスローされたならここ以降は処理されない */

        // レンダーテクスチャ読み込み
        _moviePanel = Resources.Load<GameObject>("Prefabs/UI/MoviePanel");

        // ムービーオブジェクト生成と再生
        _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.PlayingAppearanceMovie;

        // 描写パネル、ムービーの生成
        var panel = GameObject.Instantiate(_moviePanel);
        var movie_appearance = GameObject.Instantiate(appearanceMovieGO);

        var pd_appearance = movie_appearance.GetComponent<PlayableDirector>();

        pd_appearance.paused += OnloopPointReached_Appearance;
        pd_appearance.paused += (source) =>
        {
            Destroy(movie_appearance);
            Destroy(panel);
            _gameLogic.SetInGameInputBlocked(false);
            _gameLogic.SetPauseInputBlocked(false);
        };

        // ロジックへイベント登録
        _gameLogic.TaskOnBossDefeated += TaskOnBossDefeated;

        // ぬえをまたまた初期化
        GameObject.FindAnyObjectByType<NuweBrain>().Initialize();
    }

    /// <summary> ボス登場シーンの再生が完了したら </summary>
    private void OnloopPointReached_Appearance(PlayableDirector source)
    {
        _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.FinishedPlayingAppearanceMovie;

        GameObject.Destroy(source.gameObject);
        _player.SetActive(true);

        // 入力ブロックを解除
        var input = GameObject.FindAnyObjectByType<PlayerInputsAction>();
        input.ControllerInputBlocked = input.ExternalInputBlocked = false;

        // ぬえのHPバーを表示
        var nuweHP = GameObject.FindAnyObjectByType<NuweHpViewer>(FindObjectsInactive.Include);
        if (!nuweHP.gameObject.activeSelf)
        {
            nuweHP.gameObject.SetActive(true);
        }

        nuweHP.SetVisible(true);

        SpawnPlayerToPoint();
    }

    /// <summary> ボス撃破時に実行する処理群 </summary>
    private void TaskOnBossDefeated()
    {
        _gameLogic.SetInGameInputBlocked(true);
        _gameLogic.SetPauseInputBlocked(true);

        var defeatedMovieGO = Resources.Load<GameObject>("Prefabs/Video/BossDefeated");

        var panel = GameObject.Instantiate(_moviePanel);
        var movie_defeated = GameObject.Instantiate(defeatedMovieGO);

        var pd_defeated = movie_defeated.GetComponent<PlayableDirector>();
        pd_defeated.paused += OnloopPointReached_BossDefeated;
        pd_defeated.paused += (source) =>
        {
            Destroy(panel);
            Destroy(movie_defeated);
            _gameLogic.SetInGameInputBlocked(false);
            _gameLogic.SetPauseInputBlocked(false);
        };
    }

    /// <summary> ボス撃破ムービー再生後 </summary>
    private void OnloopPointReached_BossDefeated(PlayableDirector source)
    {
        // インゲームのコンテンツに使用していたオブジェクトを破棄
        var player = GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include).gameObject;
        var playerUI = GameObject.FindWithTag("PlayerUI");

        if (player is not null)
        {
            Destroy(player);
        }

        if (playerUI is not null)
        {
            Destroy(playerUI);
        }

        var sl = GameObject.FindAnyObjectByType<SceneLoader>(FindObjectsInactive.Include);
        if (sl is not null && !sl.gameObject.activeSelf)
        {
            sl.gameObject.SetActive(true);
        }

        sl.LoadSceneByName(ConstantValues.EpilogueScene);
    }

    /// <summary> プレイヤーを指定の位置にスポーン </summary>
    private void SpawnPlayerToPoint()
    {
        var player = GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include);
        var spawnPos = GameObject.FindWithTag("SpawnPos");

        if (player is not null && spawnPos is not null)
        {
            player.transform.position = spawnPos.transform.position;
            player.transform.rotation = spawnPos.transform.rotation;
        }
    }

    /// <summary> インゲームのオブジェクトを使い回すのでひとまずInActiveにスイッチ </summary>
    private void Dispose_InGameObject()
    {
        var scene = SceneManager.GetActiveScene();

        var ingameUI = GameObject.FindWithTag("PlayerUI");
        if (ingameUI is not null)
        {
            Destroy(ingameUI);
        }

        var player = GameObject.FindAnyObjectByType<PlayerParam>(FindObjectsInactive.Include);
        if (player is not null)
        {
            SceneManager.MoveGameObjectToScene(player.gameObject, scene);
            Destroy(player.gameObject);
        }

        var camera = GameObject.FindAnyObjectByType<PlayerCameraBrain>(FindObjectsInactive.Include);
        if (camera is not null)
        {
            SceneManager.MoveGameObjectToScene(camera.gameObject, scene);
            camera.DisposeCameras();
        }

        var input = GameObject.FindAnyObjectByType<PlayerInputsAction>(FindObjectsInactive.Include);
        if (input is not null)
        {
            SceneManager.MoveGameObjectToScene(input.gameObject, scene);
            Destroy(input.gameObject);
        }

        var logic = GameObject.FindAnyObjectByType<GameLogic>(FindObjectsInactive.Include);
        if (logic is not null)
        {
            SceneManager.MoveGameObjectToScene(logic.gameObject, scene);
            Destroy(logic.gameObject);
        }
    }
}

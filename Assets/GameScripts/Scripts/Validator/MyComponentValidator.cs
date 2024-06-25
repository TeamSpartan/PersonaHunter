using System;
using System.Collections;
using System.Collections.Generic;
using Player.Action;
using Player.Input;
using PlayerCam.Scripts;
using SgLibUnite.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

// コードクリーン 実施 【6/22日 ： 菅沼】
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
    [SerializeField] private GameObject _firstSelectedUIElemInScene;

    // このクラスのオブジェクトが破棄されたときに同時に破棄するオブジェクトのリスト
    [SerializeField] private List<GameObject> _destroyTargetOnDestroyedThis;

    private ClientDataHolder _clientData;
    private GameLogic _gameLogic;
    private GameObject _player;
    private GameObject _moviePanel;
    private PlayerCameraBrain _cameraBrain;
    private InGameUIManager _inGameUIManager;

    private bool _playedPrologue;

    // Start is called before the first frame update
    void Start()
    {
        Validation();
    }

    private void Validation()
    {
        _clientData = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
        _gameLogic = GameObject.FindAnyObjectByType<GameLogic>();
        _cameraBrain = GameObject.FindAnyObjectByType<PlayerCameraBrain>();
        _player = GameObject.FindAnyObjectByType<PlayerMove>()?.gameObject;
        _inGameUIManager = GameObject.FindAnyObjectByType<InGameUIManager>()?.GetComponent<InGameUIManager>();

        var scene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            case ConstantValues.TitleScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Title;

                ValidationOnTitleScene();
                Exclude_InGameObject();
                break;
            }

            case ConstantValues.PrologueScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Prologue;

                Exclude_InGameObject();
                break;
            }

            case ConstantValues.InGameScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.InGame;

                _gameLogic.Initialize();
                _cameraBrain.Init();
                _inGameUIManager.TaskOnStart();
                SpawnPlayerToPoint();
                
                break;
            }

            case ConstantValues.BossScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.TransitedBossScene;

                ValidationOnBossScene();
                _gameLogic.Initialize();
                _cameraBrain.Init();
                _inGameUIManager.TaskOnStart();
                
                break;
            }

            case ConstantValues.EpilogueScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Epilogue;

                Exclude_InGameObject();
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
            var eventSystem = GameObject.FindAnyObjectByType<EventSystem>();
            if (eventSystem is not null)
            {
                eventSystem.firstSelectedGameObject = _firstSelectedUIElemInScene;
            }
        }

        // タイトル画面のバックグラウンドのオブジェクト
        var obj = GameObject.Find("TitleImageBackGround"); // ##
        var group = obj.transform.GetComponentInChildren<CanvasGroup>();
        if (group is not null)
        {
            group.alpha = _playedPrologue ? 1 : 0;
            group.interactable = group.blocksRaycasts = _playedPrologue;
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

    private void ValidationOnBossScene()
    {
        // 入力をブロック
        var input = GameObject.FindAnyObjectByType<PlayerInputsAction>();
        input.ControllerInputBlocked = input.ExternalInputBlocked = true;

        GameObject.FindAnyObjectByType<InGameUIManager>().BossHPBarSetActive(true);

        // コマシラ のHPバーを削除
        foreach (var komashiraHpBar in GameObject.FindObjectsByType<KomashiraHPBar>(FindObjectsSortMode.None))
        {
            Destroy(komashiraHpBar.gameObject);
        }

        // プレイヤ隠ぺい
        _player?.SetActive(false);

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
            DestroyImmediate(movie_appearance);
            DestroyImmediate(panel);
        };

        // ロジックへイベント登録
        _gameLogic.TaskOnBossDefeated += TaskOnBossDefeated;

        // ぬえをまたまた初期化
        GameObject.FindAnyObjectByType<NuweBrain>().Initialize();
    }

    private void OnloopPointReached_Appearance(PlayableDirector source)
    {
        _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.FinishedPlayingAppearanceMovie;

        GameObject.DestroyImmediate(source.gameObject);
        _player.SetActive(true);

        // 入力ブロックを解除
        var input = GameObject.FindAnyObjectByType<PlayerInputsAction>();
        input.ControllerInputBlocked = input.ExternalInputBlocked = false;

        SpawnPlayerToPoint();
    }

    private void TaskOnBossDefeated()
    {
        var defeatedMovieGO = Resources.Load<GameObject>("Prefabs/Video/BossDefeated");

        var panel = GameObject.Instantiate(_moviePanel);
        var movie_defeated = GameObject.Instantiate(defeatedMovieGO);

        var pd_defeated = movie_defeated.GetComponent<PlayableDirector>();
        pd_defeated.paused += OnloopPointReached_BossDefeated;
        pd_defeated.paused += (source) =>
        {
            DestroyImmediate(panel);
            DestroyImmediate(movie_defeated);
        };
    }

    private void OnloopPointReached_BossDefeated(PlayableDirector source)
    {
        // インゲームのコンテンツに使用していたオブジェクトを破棄
        var player = GameObject.FindAnyObjectByType<PlayerMove>().gameObject;
        var playerUI = GameObject.FindWithTag("PlayerUI");
        DestroyImmediate(player);
        DestroyImmediate(playerUI);

        GameObject.FindAnyObjectByType<SceneLoader>().LoadSceneByName(ConstantValues.EpilogueScene);
    }

    private void SpawnPlayerToPoint()
    {
        var player = GameObject.FindAnyObjectByType<PlayerMove>().gameObject;
        var spawnPos = GameObject.FindWithTag("SpawnPos").transform;

        player.transform.position = spawnPos.position;
        player.transform.rotation = spawnPos.rotation;
    }

    private void Exclude_InGameObject()
    {
        // プレイヤとUIを破棄する
        Destroy(GameObject.FindWithTag("PlayerUI"));
        Destroy(GameObject.FindWithTag("Player"));

        // DDOL解除
        var lockOnCam = GameObject.FindWithTag("LockOnCam");
        var followCam = GameObject.FindWithTag("FollowCam");
        var scene = SceneManager.GetActiveScene();
        if (lockOnCam is not null)
        {
            SceneManager.MoveGameObjectToScene(lockOnCam, scene);
            Destroy(lockOnCam);
        }

        if (followCam is not null)
        {
            SceneManager.MoveGameObjectToScene(followCam, scene);
            Destroy(followCam);
        }

        // デストロォォォォォイィィィィィィィ
        // if (GameObject.FindAnyObjectByType<PlayerInputsAction>().gameObject is not null)
        //     Destroy(GameObject.FindAnyObjectByType<PlayerInputsAction>().gameObject);
        // if (GameObject.FindAnyObjectByType<PlayerCameraBrain>().gameObject is not null)
        //     Destroy(GameObject.FindAnyObjectByType<PlayerCameraBrain>().gameObject);
    }
}

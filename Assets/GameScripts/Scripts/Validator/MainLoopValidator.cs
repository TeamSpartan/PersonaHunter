using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using SgLibUnite.Systems;
using PlayerCam.Scripts;
using Player.Action;
using Player.Input;
using Player.Param;
using System.Linq;
using GameScripts.Scripts.GameLogic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

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
public class MainLoopValidator : MonoBehaviour
{
    // このクラスのオブジェクトが破棄されたときに同時に破棄するオブジェクトのリスト
    [SerializeField] private List<GameObject> _destroyTargetOnDestroyedThis;

    /// <summary> クライアントデータ </summary>
    private ClientDataHolder _clientData;

    /// <summary> 入力バッファ </summary>
    private PlayerInputsAction _input;

    /// <summary> プレイヤカメラ </summary>
    private PlayerCameraBrain _cameraBrain;

    /// <summary> インゲームUIマネージャ </summary>
    private InGameUIManager _inGameUIManager;

    /// <summary> そのシーンにのみ存在できるオブジェクトのリスト </summary>
    private List<GameObject> _thisSceneOnlyObj;

    /// <summary> プレイヤ </summary>
    private GameObject _player;

    /// <summary> ムービーのパネル </summary>
    private GameObject _moviePanel;

    /// <summary> 最初に選択されるUIエレメント </summary>
    private GameObject _firstSelectedUIElemInScene;

    /// <summary> ゲームロジック </summary>
    private MainGameLoop _mainGameLoop;

    // Start is called before the first frame update
    private void Start()
    {
        // Move Main Display To Game Window
        List<DisplayInfo> dInfo = new List<DisplayInfo>();
        Screen.GetDisplayLayout(dInfo);
        if (dInfo.Count > 1)
            Screen.MoveMainWindowTo(dInfo[0], dInfo[1].workArea.position);
        // モニタが２以上あるなら

        Validation();
        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged_DestroyThisSceneOnlyObj;
    }

    private void SceneManagerOnactiveSceneChanged_DestroyThisSceneOnlyObj(Scene arg0, Scene arg1)
    {
        foreach (var o in _thisSceneOnlyObj)
            Destroy(o);
    }

    /// <summary> 各シーンに配置されたこのコンポーネントがアタッチされているオブジェクトが初期化されたタイミングで呼ばれる </summary>
    private void Validation()
    {
        _clientData = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
        _firstSelectedUIElemInScene = GameObject.FindGameObjectWithTag("FirstSelectedUIElement");

        _thisSceneOnlyObj = GameObject.FindGameObjectsWithTag("ThisSceneOnly").ToList();

        _mainGameLoop = GameObject.FindAnyObjectByType<MainGameLoop>(FindObjectsInactive.Include);
        if (_mainGameLoop is not null && !_mainGameLoop.gameObject.activeSelf)
            gameObject.gameObject.SetActive(true);


        _cameraBrain = GameObject.FindAnyObjectByType<PlayerCameraBrain>(FindObjectsInactive.Include);
        if (_cameraBrain is not null && !_cameraBrain.gameObject.activeSelf)
            _cameraBrain.gameObject.SetActive(true);


        var playerMove = GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include);
        if (playerMove is not null && !playerMove.gameObject.activeSelf)
        {
            playerMove.gameObject.SetActive(true);
            _player = playerMove.gameObject;
        }

        _inGameUIManager = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
        if (_inGameUIManager is not null && !_inGameUIManager.gameObject.activeSelf)
            _inGameUIManager.gameObject.SetActive(true);

        _input = GameObject.FindAnyObjectByType<PlayerInputsAction>(FindObjectsInactive.Include);
        if (_input is not null && !_input.gameObject.activeSelf)
            _input.gameObject.SetActive(true);
        else if (_input is not null)
            // 【初期値はUIの入力タイプ】
            _input.InputType = InputType.UI;


        var scene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            case ConstantValues.TitleScene:
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Title;

                ValidationOnTitleScene();
                break;

            case ConstantValues.PrologueScene:
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Prologue;

                break;

            case ConstantValues.InGameScene:
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.InGame;

                GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include).gameObject.SetActive(true);
                GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include).gameObject.SetActive(true);

                _mainGameLoop.Initialize();
                _cameraBrain.Initialize();
                _inGameUIManager.ToDoOnStart();
                _input.InputType = InputType.Player;

                SpawnPlayerToPoint();
                break;

            case ConstantValues.BossScene:
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.TransitedBossScene;

                ValidationOnBossScene();
                _mainGameLoop.Initialize();
                _cameraBrain.Initialize();
                _inGameUIManager.ToDoOnStart();
                _input.InputType = InputType.Player;

                break;

            case ConstantValues.EpilogueScene:
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.Epilogue;

                break;
        }
    }

    public void SkipPrologue()
    {
        // スキップ入力
        _clientData.NotifyPlayedPrologue();

        GameObject.FindAnyObjectByType<SceneLoader>()
            .LoadSceneByName(ConstantValues.TitleScene);
    }

    private void OnDestroy()
    {
        foreach (var target in _destroyTargetOnDestroyedThis)
            Destroy(target);
    }

    /// <summary>
    /// PressAnyButtonが押された時のイベント。プロローグ再生が終わっているかによって処理が変化
    /// </summary>
    public void OnPressedAnyButton()
    {
        var eventSystem = GameObject.FindAnyObjectByType<EventSystem>(); // 今アクティブなイベントシステムのクラスが欲しい
        // EventSystem の選択オブジェクトを変更
        if (_clientData.PlayedPrologue)
        {
            eventSystem.firstSelectedGameObject = null;
            // PressAnyButtonのパネル
            var panel = GameObject.Find("PressAnyButtonPanel");
            if ((panel is not null))
                GameObject.Destroy(panel);

            eventSystem.SetSelectedGameObject(GameObject.FindGameObjectWithTag("FirstSelectedUIElement"));
        }
        else
        {
            GameObject.FindAnyObjectByType<SceneLoader>().LoadSceneByName(ConstantValues.PrologueScene);
        }
    }

    /// <summary> タイトルシーンのヴァリデーション </summary>
    private void ValidationOnTitleScene()
    {
        Dispose_InGameObject();

        // タイトル画面のバックグラウンドのオブジェクト
        var obj = GameObject.Find("TitleImageBackGround");
        if (obj is not null)
        {
            var group = obj.transform.GetComponentInChildren<CanvasGroup>();
            if (group is not null)
            {
                group.alpha = _clientData.PlayedPrologue ? 1 : 0;
                group.interactable = group.blocksRaycasts = _clientData.PlayedPrologue;
            }
        }
    }

    /// <summary> ボスシーンでのヴァリエーション </summary>
    private void ValidationOnBossScene()
    {
        // インゲーム入力をブロック
        _mainGameLoop.SetInGameInputBlocked(true);

        // ポーズ入力をブロック 【入力タイプがUIになるのを防ぐ】
        _mainGameLoop.SetPauseInputBlocked(true);
        Cursor.lockState = CursorLockMode.Locked;

        var ingameUI = GameObject.FindAnyObjectByType<InGameUIManager>(FindObjectsInactive.Include);
        if (!ingameUI.gameObject.activeSelf)
            ingameUI.BossHPBarSetActive(true);


        // コマシラ のHPバーを削除
        foreach (var komashiraHpBar in GameObject.FindObjectsByType<KomashiraHPBar>(FindObjectsSortMode.None))
            Destroy(komashiraHpBar.gameObject);


        // プレイヤ隠ぺい
        if (_player is null)
            _player = GameObject.FindWithTag("Player");


        _input.ClearInputBuffer();

        _input.enabled = false;

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
            _mainGameLoop.SetInGameInputBlocked(false);
            _mainGameLoop.SetPauseInputBlocked(false);

            GameObject.Find("BGM Source").GetComponent<AudioSource>().Play();
        }; // 登場ムービーの再生が終わったら

        // ロジックへイベント登録
        _mainGameLoop.TaskOnBossDefeated += TaskOnBossDefeated;
        _mainGameLoop.TaskOnBossMorphologicalChange += TaskOnBossMorphologicalChange;

        // ぬえをまたまた初期化
        GameObject.FindAnyObjectByType<NuweBrain>().Initialize();
    }

    /// <summary> ボス登場シーンの再生が完了したら </summary>
    private void OnloopPointReached_Appearance(PlayableDirector source)
    {
        _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.FinishedPlayingAppearanceMovie;

        GameObject.Destroy(source.gameObject);
        _player.SetActive(true);

        // 入力バッファ有効化
        _input.enabled = true;

        // 入力ブロックを解除
        _input.ControllerInputBlocked = _input.ExternalInputBlocked = false;

        // ぬえのHPバーを表示
        var nuweHP = GameObject.FindAnyObjectByType<NuweHpViewer>(FindObjectsInactive.Include);
        if (!nuweHP.gameObject.activeSelf)
            nuweHP.gameObject.SetActive(true);


        nuweHP.SetVisible(true);

        SpawnPlayerToPoint();
    }

    /// <summary> ボス撃破時に実行する処理群 </summary>
    private void TaskOnBossDefeated()
    {
        _mainGameLoop.SetInGameInputBlocked(true);
        _mainGameLoop.SetPauseInputBlocked(true);

        // BGMを止める
        GameObject.Find("BGM Source").GetComponent<AudioSource>().Pause();

        var defeatedMovieGO = Resources.Load<GameObject>("Prefabs/Video/BossDefeated");

        var panel = GameObject.Instantiate(_moviePanel);
        var movie_defeated = GameObject.Instantiate(defeatedMovieGO);

        var pd_defeated = movie_defeated.GetComponent<PlayableDirector>();
        pd_defeated.paused += OnloopPointReached_BossDefeated;
        pd_defeated.paused += (source) =>
        {
            Destroy(panel);
            Destroy(movie_defeated);
            _mainGameLoop.SetInGameInputBlocked(false);
            _mainGameLoop.SetPauseInputBlocked(false);

            // インゲームでつかっていたオブジェクトを破棄
            Dispose_InGameObject();
        };
    }

    /// <summary> ボス撃破ムービー再生後 </summary>
    private void OnloopPointReached_BossDefeated(PlayableDirector source)
    {
        // インゲームのコンテンツに使用していたオブジェクトを破棄
        var player = GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include).gameObject;
        var playerUI = GameObject.FindWithTag("PlayerUI");
        var audioManager = FindAnyObjectByType<AudioManager>();

        if (player is not null)
            Destroy(player);


        if (playerUI is not null)
            Destroy(playerUI);

        if(audioManager is not null)
            Destroy(audioManager.gameObject);

        var sl = GameObject.FindAnyObjectByType<SceneLoader>(FindObjectsInactive.Include);
        if (sl is not null && !sl.gameObject.activeSelf)
            sl.gameObject.SetActive(true);

        sl.LoadSceneByName(ConstantValues.EpilogueScene);
    }

    /// <summary> ボスの第２形態移行時のに実行する処理群 </summary>
    public void TaskOnBossMorphologicalChange()
    {
        _mainGameLoop.SetInGameInputBlocked(true);
        _mainGameLoop.SetPauseInputBlocked(true);
        
        GameObject.Find("BGM Source").GetComponent<AudioSource>().Pause();

        var defeatedMovieGO = Resources.Load<GameObject>("Prefabs/Video/BossMorphologicalChange");

        var panel = GameObject.Instantiate(_moviePanel);
        var movie_defeated = GameObject.Instantiate(defeatedMovieGO);

        var pd_defeated = movie_defeated.GetComponent<PlayableDirector>();
        pd_defeated.paused += OnloopPointReached_BossMorphologicalChange;
        pd_defeated.paused += (source) =>
        {
            Destroy(panel);
            Destroy(movie_defeated);
            _mainGameLoop.SetInGameInputBlocked(false);
            _mainGameLoop.SetPauseInputBlocked(false);
            Debug.Log("MovieEnd");
        };
    }
    
    /// <summary> ボス第２形態ムービー再生後 </summary>
    private void OnloopPointReached_BossMorphologicalChange(PlayableDirector source)
    {
        // インゲームのコンテンツに使用していたオブジェクトを破棄
        var nuwe = GameObject.FindAnyObjectByType<NuweBrain>(FindObjectsInactive.Include);
        nuwe.BackToAwait();
        GameObject.Find("BGM Source").GetComponent<AudioSource>().Play();
    }

    /// <summary> プレイヤーを指定の位置にスポーン </summary>
    private void SpawnPlayerToPoint()
    {
        var player = GameObject.FindAnyObjectByType<PlayerMove>(FindObjectsInactive.Include);
        var spawnPos = GameObject.FindAnyObjectByType<PlayerSpawnPos>();

        if (player is not null && spawnPos is not null)
        {
            player.transform.position = spawnPos.transform.position;
            player.transform.forward = spawnPos.Direction.normalized;
        }
    }

    /// <summary> インゲームのオブジェクトを使い回すのでひとまずInActiveにスイッチ </summary>
    public void Dispose_InGameObject()
    {
        var scene = SceneManager.GetActiveScene();

        var ingameUI = GameObject.FindWithTag("PlayerUI");
        if (ingameUI is not null)
        {
            SceneManager.MoveGameObjectToScene(ingameUI.gameObject, scene);
            Destroy(ingameUI);
        }

        var option = GameObject.FindWithTag("OptionWindow");
        if (option is not null)
        {
            SceneManager.MoveGameObjectToScene(option, scene);
            Destroy(option);
        }

        var volume = GameObject.FindWithTag("OmotegariVolume");
        if (volume is not null)
        {
            SceneManager.MoveGameObjectToScene(volume, scene);
            Destroy(volume);
        }

        var afterImage = GameObject.FindWithTag("AfterImage");
        if (afterImage is not null)
        {
            SceneManager.MoveGameObjectToScene(afterImage, scene);
            Destroy(afterImage);
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
            Destroy(camera.gameObject);
        }

        var input = GameObject.FindAnyObjectByType<PlayerInputsAction>(FindObjectsInactive.Include);
        if (input is not null)
        {
            SceneManager.MoveGameObjectToScene(input.gameObject, scene);
            Destroy(input.gameObject);
        }

        var logic = GameObject.FindAnyObjectByType<MainGameLoop>(FindObjectsInactive.Include);
        if (logic is not null)
        {
            SceneManager.MoveGameObjectToScene(logic.gameObject, scene);
            Destroy(logic.gameObject);
        }
    }
}

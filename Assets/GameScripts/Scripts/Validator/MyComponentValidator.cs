using System.Collections;
using System.Collections.Generic;
using Player.Action;
using Player.Input;
using PlayerCam.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

/* 各シーンのオブジェクトが参照を持っていて依存をしているため、シングルトンだめ */

/// <summary>
/// ユーザー定義のゲーム開始時にヴァリデーション処理を受け持つクラス。
/// </summary>
public class MyComponentValidator : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelectedInTitle;

    private bool _playedPrologue;

    private ClientDataHolder _clientData;

    private GameObject _bossAppearanceMovie;

    private GameLogic _gameLogic;

    private GameObject _player;

    private GameObject _moviePanel;

    // Start is called before the first frame update
    void Start()
    {
        _clientData = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
        _gameLogic = GameObject.FindAnyObjectByType<GameLogic>();
        _gameLogic.gameObject.SetActive(false);
        if (GameObject.FindAnyObjectByType<PlayerMove>() is not null)
        {
            _player = GameObject.FindAnyObjectByType<PlayerMove>().gameObject;
        }

        Validation();
        EnableLogic();
    }

    private void EnableLogic()
    {
        _gameLogic.gameObject.SetActive(true);
    }

    private void Validation()
    {
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

                SpawnPlayerToPoint();
                break;
            }

            case ConstantValues.BossScene:
            {
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.TransitedBossScene;

                // プレイヤ隠ぺい
                _player.SetActive(false);

                // ムービー読み込み
                _bossAppearanceMovie = Resources.Load<GameObject>("Prefabs/Video/BossAppearance");

                /* 例外がスローされたならここ以降は処理されない */

                // レンダーテクスチャ読み込み
                _moviePanel = Resources.Load<GameObject>("Prefabs/UI/MoviePanel");

                // ムービーオブジェクト生成と再生
                _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.PlayingAppearanceMovie;

                // 描写パネル、ムービーの生成
                var panel = GameObject.Instantiate(_moviePanel);
                var movie = GameObject.Instantiate(_bossAppearanceMovie);

                var vp = movie.GetComponent<VideoPlayer>();

                vp.loopPointReached += OnloopPointReached_Appearance;
                vp.loopPointReached += (source) =>
                {
                    DestroyImmediate(movie);
                    DestroyImmediate(panel);
                };

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

    private void OnloopPointReached_Appearance(VideoPlayer source)
    {
        _clientData.CurrentSceneStatus = ClientDataHolder.InGameSceneStatus.FinishedPlayingAppearanceMovie;
        source.targetCameraAlpha = 0f;
        _player.SetActive(true);
        SpawnPlayerToPoint();
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
            var es = GameObject.FindAnyObjectByType<EventSystem>();
            if (es is not null)
            {
                es.firstSelectedGameObject = _firstSelectedInTitle;
            }
        }

        // タイトル画面のバックグラウンドのオブジェクト
        var obj = GameObject.Find("TitleImageBackGround");
        var group = obj.transform.GetComponentInChildren<CanvasGroup>();
        if (group is not null)
        {
            group.alpha = _playedPrologue ? 1 : 0;
            group.interactable = group.blocksRaycasts = _playedPrologue;
        }

        // PressAnyButtonのパネル
        obj = GameObject.Find("PressAnyButtonPanel");
        if (obj is not null)
        {
            if (_playedPrologue)
            {
                GameObject.Destroy(obj);
            }
        }
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
        SceneManager.MoveGameObjectToScene(lockOnCam, scene);
        SceneManager.MoveGameObjectToScene(followCam, scene);
        // デストロォォォォォイィィィィィィィ
        Destroy(GameObject.FindAnyObjectByType<PlayerInputsAction>().gameObject);
        Destroy(GameObject.FindAnyObjectByType<PlayerCameraBrain>().gameObject);
        Destroy(lockOnCam);
        Destroy(followCam);
    }
}

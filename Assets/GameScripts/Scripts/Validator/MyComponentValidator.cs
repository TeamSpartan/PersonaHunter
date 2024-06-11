using System.Collections;
using System.Collections.Generic;
using Player.Action;
using Player.Input;
using PlayerCam.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/* 各シーンのオブジェクトが参照を持っていて依存をしているため、シングルトンだめ */

/// <summary>
/// ユーザー定義のゲーム開始時にヴァリデーション処理を受け持つクラス。
/// </summary>
public class MyComponentValidator : MonoBehaviour
{
    [SerializeField] private GameObject _firstSelectedInTitle;

    private bool _playedPrologue;

    // Start is called before the first frame update
    void Start()
    {
        Validation();
    }

    private void Validation()
    {
        var scene = SceneManager.GetActiveScene();

        switch (scene.name)
        {
            case ConstantValues.TitleScene:
            {
                ValidationOnTitleScene();
                ExcludeInGameObject();
                break;
            }

            case ConstantValues.PrologueScene:
            {
                ExcludeInGameObject();
                break;
            }

            case ConstantValues.InGameScene:
            {
                SpawnPlayerToPoint();
                break;
            }

            case ConstantValues.BossScene:
            {
                SpawnPlayerToPoint();
                break;
            }

            case ConstantValues.EpilogueScene:
            {
                ExcludeInGameObject();
                break;
            }
        }
    }

    private void ValidationOnTitleScene()
    {
        // プロローグを再生したかの静的フィールドにアクセス
        var tempData = Resources.Load<ClientDataHolder>("Prefabs/GameSystem/ClientDataHolder");
        if (tempData is not null)
        {
            _playedPrologue = tempData.PlayedPrologue;
        }

        // EventSystem の選択オブジェクトを変更
        if (tempData.PlayedPrologue)
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

    private void ExcludeInGameObject()
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

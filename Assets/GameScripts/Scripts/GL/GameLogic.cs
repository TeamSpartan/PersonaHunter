using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// オモテガリ ゲームロジック：メイン
/// </summary>
public class GameLogic
    : MonoBehaviour
{
    /// <summary>
    /// タイトルシーン インゲーム ムービー のシーンをリスト登録しているSO
    /// </summary>
    private SceneInfo _sceneInfo;
    /// <summary>
    /// シーンが １．タイトル ２．インゲーム ３．ユニーク（ムービー） かの区分
    /// </summary>
    private SceneCategory _currentSceneCategory;

    private void SceneManagerOnactiveSceneChanged(Scene arg0, Scene arg1)
    {
        // タイトルへ遷移した場合
        if (_sceneInfo.TitleScenes.Select(_ => _ == arg1).ToList().Count > 0)
        {
            _currentSceneCategory = SceneCategory.TitleScene;
        }

        // インゲームへ遷移した場合
        if (_sceneInfo.IngameScenes.Select(_ => _ == arg1).ToList().Count > 0)
        {
            _currentSceneCategory = SceneCategory.InGameScene;
        }

        // ユニークシーン（ムービー）へ遷移した場合
        if (_sceneInfo.UniqueScenes.Select(_ => _ == arg1).ToList().Count > 0)
        {
            _currentSceneCategory = SceneCategory.UniqueScene;
        }
    }

    private void InitializeGame()
    {
        #region Initialize All Initializable User Defined Components

        var c = GameObject.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None).ToList();
        var initializableObj
            = c.Where(obj => (IInitializableComponent)obj != null).ToList()
                .Select(_ => (IInitializableComponent)_).ToList();
        foreach (var obj in initializableObj)
            obj.InitializeThisComp();

        #endregion
    }

    private void FinalizeGame()
    {
        #region Finalize All Initializable User Defined Components

        var c = GameObject.FindObjectsByType<UnityEngine.Object>(FindObjectsSortMode.None).ToList();
        var initializableObj
            = c.Where(obj => (IInitializableComponent)obj != null).ToList()
                .Select(_ => (IInitializableComponent)_).ToList();
        foreach (var obj in initializableObj)
            obj.FinalizeThisComp();

        #endregion
    }

    private void GameLoop()
    {
    }

    /// <summary>
    /// ゲームシーンが開始したときのイベント １回のみ呼び出される。 ライフサイクルでおけるStart
    /// </summary>
    private void TaskOnSceneStart()
    {
        // switch task on start
        switch (_currentSceneCategory)
        {
            case SceneCategory.TitleScene:
                break;
            case SceneCategory.InGameScene:
                InitializeGame();
                break;
            case SceneCategory.UniqueScene:
                break;
        }
    }

    /// <summary>
    /// ゲームシーンの毎フレームのイベント 毎フレーム呼び出される。 ライフサイクルでおけるFixedUpdate
    /// </summary>
    private void TaskOnSceneTick()
    {
        // switch task on start
        switch (_currentSceneCategory)
        {
            case SceneCategory.TitleScene:
                break;
            case SceneCategory.InGameScene:
                GameLoop();
                break;
            case SceneCategory.UniqueScene:
                break;
        }
    }

    /// <summary>
    /// ゲームシーンが終了するときに呼び出される。 ライフサイクルでおけるOnApplicationQuit
    /// </summary>
    private void TaskOnSceneEnd()
    {
        // switch task on start
        switch (_currentSceneCategory)
        {
            case SceneCategory.TitleScene:
                break;
            case SceneCategory.InGameScene:
                FinalizeGame();
                break;
            case SceneCategory.UniqueScene:
                break;
        }
    }

    private void OnEnable()
    {
        var s = SceneManager.GetActiveScene();
        // タイトルシーンなら
        if (_sceneInfo.TitleScenes.Select(_ => _ == s).ToList().Count > 0)
        {
            _currentSceneCategory = SceneCategory.TitleScene;
        }

        // インゲームシーン
        if (_sceneInfo.IngameScenes.Select(_ => _ == s).ToList().Count > 0)
        {
            _currentSceneCategory = SceneCategory.InGameScene;
        }

        // ユニーク（ムービー）シーン
        if (_sceneInfo.UniqueScenes.Select(_ => _ == s).ToList().Count > 0)
        {
            _currentSceneCategory = SceneCategory.UniqueScene;
        }

        SceneManager.activeSceneChanged += SceneManagerOnactiveSceneChanged;
    }

    private void Start()
    {
        TaskOnSceneStart();
    }

    private void FixedUpdate()
    {
        TaskOnSceneTick();
    }

    private void OnApplicationQuit()
    {
        TaskOnSceneEnd();
    }
}
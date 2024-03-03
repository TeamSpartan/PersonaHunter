using SgLibUnite.Singleton;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// auth 菅沼
public class GameInfo
    : SingletonBaseClass<GameInfo>
{
    /// <summary> 遷移先シーンがどのようなものかを選択、保持する </summary>
    public enum SceneTransitStatus
    {
        WentToTitleScene, // タイトルシーン
        WentToUniqueScene, // ムービーシーンなどの特殊シーン
        WentToInGameScene, // インゲームシーン
    }

    [SerializeField] SceneTransitStatus _sceneStatus;

    [SerializeField] SceneInfo _sInfo;

    public SceneTransitStatus GetSceneStatus
    {
        get { return _sceneStatus; }
    }

    public SceneInfo GetSceneInfo
    {
        get { return _sInfo; }
    }

    protected override void ToDoAtAwakeSingleton()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            #region find matching scene type

            var titles = _sInfo.TitleScenesName.Select(_ => _).ToList();
            var ingames = _sInfo.IngameScenesName.Select(_ => _).ToList();
            var uniques = _sInfo.UniqueScenesName.Select(_ => _).ToList();

            foreach (var title in titles)
            {
                if (title == arg1.name)
                {
                    _sceneStatus = SceneTransitStatus.WentToTitleScene;
                    break;
                }
            }


            foreach (var ingame in ingames)
            {
                if (ingame == arg1.name)
                {
                    _sceneStatus = SceneTransitStatus.WentToInGameScene;
                    break;
                }
            }

            foreach (var unique in uniques)
            {
                if (unique == arg1.name)
                {
                    _sceneStatus = SceneTransitStatus.WentToUniqueScene;
                    break;
                }
            }
        }

        #endregion
    }
}
using SgLibUnite.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// auth 菅沼
public class GameInfo : SingletonBaseClass<GameInfo>
{
    /// <summary> 遷移先シーンがどのようなものかを選択、保持する </summary>
    public enum SceneTransitStatus
    {
        WentToTitleScene, // タイトルシーン
        WentToUniqueScene, // ムービーシーンなどの特殊シーン
        WentToInGameScene, // インゲームシーン
    }

    [SerializeField] SceneTransitStatus sceneStatus;

    [SerializeField] SceneInfo sInfo;

    public SceneTransitStatus GetSceneStatus
    {
        get { return sceneStatus; }
    }

    protected override void ToDoAtAwakeSingleton()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

        void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            #region [For-Each Loop]

            var titles = sInfo.TitleScenes.Select(_ => _.name).ToList();
            var ingames = sInfo.IngameScenes.Select(_ => _.name).ToList();
            var uniques = sInfo.UniqueScenes.Select(_ => _.name).ToList();

            foreach (var title in titles)
            {
                if (title == arg1.name)
                {
                    sceneStatus = SceneTransitStatus.WentToTitleScene;
                    break;
                }
            }


            foreach (var ingame in ingames)
            {
                if (ingame == arg1.name)
                {
                    sceneStatus = SceneTransitStatus.WentToInGameScene;
                    break;
                }
            }

            foreach (var unique in uniques)
            {
                if (unique == arg1.name)
                {
                    sceneStatus = SceneTransitStatus.WentToUniqueScene;
                    break;
                }
            }
        }

        #endregion
    }
}
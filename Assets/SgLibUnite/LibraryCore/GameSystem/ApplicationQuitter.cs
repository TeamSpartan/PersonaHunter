using System;
using SgLibUnite.Singleton;
using UnityEditor;
using UnityEngine;

// 作成 すがぬま
namespace SgLibUnite
{
    namespace Systems
    {
        public class ApplicationQuitter : SingletonBaseClass<ApplicationQuitter>
        {
            [SerializeField, Header("Player Tag")] string PlayerTag;

            event Action<GameInfo.SceneTransitStatus> eventOnTransit;

            public event Action<GameInfo.SceneTransitStatus>
                EventOnQuitApp
                {
                    add { eventOnTransit += value; }
                    remove { eventOnTransit -= value; }
                }

            Transform _pTrans;
            GameInfo _gInfo;

            protected override void ToDoAtAwakeSingleton()
            {
                _gInfo = GameObject.FindFirstObjectByType<GameInfo>();
            }

            /// <summary> アプリケーションを閉じる </summary>
            public void QuitApplication()
            {
                #region TaskOnEditor

#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif

                #endregion

                eventOnTransit(_gInfo.GetSceneStatus);

                Application.Quit();
            }
        }
    }
}
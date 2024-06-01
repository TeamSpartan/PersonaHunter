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

            Transform _player;

            protected override void ToDoAtAwakeSingleton()
            {
            }

            /// <summary> アプリケーションを閉じる </summary>
            public void QuitApplication()
            {
                #region TaskOnEditor

#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#endif

                #endregion

                Application.Quit();
            }
        }
    }
}

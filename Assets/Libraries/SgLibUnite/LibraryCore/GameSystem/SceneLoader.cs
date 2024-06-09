using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SgLibUnite.Singleton;
using UnityEditor;

// Auth : Suganuma
namespace SgLibUnite
{
    namespace Systems
    {
        public class SceneLoader : SingletonBaseClass<SceneLoader>
        {
            protected override void ToDoAtAwakeSingleton()
            {
            }
            
            public void LoadSceneByName(string sceneName)
            {
                StartCoroutine(LoadSceneAcyncByName(sceneName));
            }

            public void UnLoadSceneByName(string sceneName)
            {
                StartCoroutine(UnLoadSceneAsyncByName(sceneName));
            }

            IEnumerator LoadSceneAcyncByName(string sceneName)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
            }

            IEnumerator UnLoadSceneAsyncByName(string sceneName)
            {
                var targetScene = SceneManager.GetSceneByName(sceneName);
                var asyncOperation = SceneManager.UnloadSceneAsync(targetScene);
                while (!asyncOperation.isDone)
                {
                    yield return null;
                }
            }

            public interface IOnSceneTransit
            {
                public void OnSceneTransitComplete(Scene scene);
            }
        }
    }
}

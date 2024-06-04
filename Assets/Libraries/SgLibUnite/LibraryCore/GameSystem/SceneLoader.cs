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
        public class SceneLoader : MonoBehaviour
        {
            public void LoadSceneByName(string sceneName)
            {
                StartCoroutine(LoadSceneAcyncByName(sceneName));
            }

            public void LoadScene(SceneAsset scene)
            {
                StartCoroutine(LoadSceneAcyncByName(scene.name));
            }

            public void LoadSceneAdditive(SceneAsset scene)
            {
                StartCoroutine(LoadSceneAcyncByNameAdditive(scene.name));
            }

            public void UnLoadSceneByName(string sceneName)
            {
                StartCoroutine(UnLoadSceneAsyncByName(sceneName));
            }

            public void UnLoadScene(SceneAsset scene)
            {
                StartCoroutine(UnLoadSceneAsyncByName(scene.name));
            }

            IEnumerator LoadSceneAcyncByName(string sceneName)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }
            }

            IEnumerator LoadSceneAcyncByNameAdditive(string sceneName)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                var scene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(scene);
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

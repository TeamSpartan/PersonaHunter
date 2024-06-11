using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Auth : Suganuma
namespace SgLibUnite
{
    namespace Systems
    {
        /* 各シーンのオブジェクトが参照を持っていて依存をしているため、シングルトンだめ */
        
        public class SceneLoader : MonoBehaviour
        {   
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

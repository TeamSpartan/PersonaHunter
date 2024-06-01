using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using SgLibUnite.Singleton;
using UnityEditor;

// Auth : Suganuma
namespace SgLibUnite
{
    namespace Systems
    {
        public class SceneLoader : SingletonBaseClass<SceneLoader>
        {
            [SerializeField, Header("Now Loading Panel")]
            GameObject nowLoadingPanel;
            [SerializeField, Header("Loading Text")]
            Text loadingText;
            [SerializeField, Header("The Fired Event On Transit Scene")]
            public UnityEvent<Scene> eventOnSceneLoaded;

            public void LoadSceneByName(string sceneName)
            {
                StartCoroutine(LoadSceneAcyncByName(sceneName));
            }
            
            public void LoadSceneByName(SceneAsset scene)
            {
                StartCoroutine(LoadSceneAcyncByName(scene.name));
            }

            protected override void ToDoAtAwakeSingleton()
            {
                nowLoadingPanel.SetActive(false);
                nowLoadingPanel.transform.SetAsFirstSibling();
                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            }

            void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
            {
                eventOnSceneLoaded.Invoke(arg1);   // 他クラスから

                nowLoadingPanel.transform.SetAsFirstSibling();
                nowLoadingPanel.SetActive(false);
            }

            IEnumerator LoadSceneAcyncByName(string sceneName)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
                while (!asyncLoad.isDone)
                {
                    nowLoadingPanel.transform.SetAsLastSibling();
                    nowLoadingPanel.SetActive(!false);
                    yield return null;
                }
            }
        }

        public interface IOnSceneTransit
        {
            public void OnSceneTransitComplete(Scene scene);
        }
    }
}

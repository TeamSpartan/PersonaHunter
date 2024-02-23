using System;
using System.IO;
using SgLibUnite.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

//作成 菅沼

namespace SgLibUnite
{
    namespace Systems
    {
        [Serializable]
        public class ClientDataTemplate
        {
            /// <summary> The Position The Player Was Standing </summary>
            public Vector3 lastStandingPosition; // Pos

            /// <summary> The Rotation The Player Was Looking Direction </summary>
            public Quaternion lastLookingRotation; // Rot

            /// <summary> The Scene Name Player Was There </summary>
            public string sceneNameLastStand; // Scene Name
        }

        /// <summary> 渡されたフィールドの値をもとにScriptableObjectを生成、DataPath直下へ格納。 </summary>
        public class ClientDataSaver : SingletonBaseClass<ClientDataSaver> // セーブデータの保存
        {
            [SerializeField, Header("The Tag That Player Has")]
            string PlayerTag;

            string _playerDataPath;
            string _dataDataFileName;
            
            public string SetDataFileName
            {
                set { _dataDataFileName = value; }
            }

            public string GetDataFileName
            {
                get { return _dataDataFileName; }
            }

            Transform _pTrans;

            GameInfo _gInfo;

            protected override void ToDoAtAwakeSingleton()
            {
                _gInfo = GameObject.FindFirstObjectByType<GameInfo>();
                _playerDataPath = Application.dataPath + "/" + _dataDataFileName + ".json";
            }

            /// <summary> プレイヤーのトランスフォームとシーン名を自動的に取得してセーブ </summary>
            public void SaveData()
            {
                switch (_gInfo.GetSceneStatus)
                {
                    case GameInfo.SceneTransitStatus.WentToTitleScene:
                        break;
                    case GameInfo.SceneTransitStatus.WentToInGameScene:
                        if (_pTrans == null)
                        {
                            _pTrans = GameObject.FindGameObjectWithTag(PlayerTag).transform;
                        }

                        SaveData(_pTrans, SceneManager.GetActiveScene().name);
                        break;
                }
            }

            /// <summary> トランスフォームとシーン名を手動で指定してセーブ </summary>
            public void SaveData(Transform playerStandingTransform, string sceneName)
            {
                ClientDataTemplate template = new ClientDataTemplate();
                template.lastStandingPosition = playerStandingTransform.position;
                template.lastLookingRotation = playerStandingTransform.rotation;
                template.sceneNameLastStand = sceneName;

                string jsonStr = JsonUtility.ToJson(template);

                StreamWriter sw = new StreamWriter(_playerDataPath, false);
                sw.WriteLine(jsonStr);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
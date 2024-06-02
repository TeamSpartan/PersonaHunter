using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SceneInfo_gen", menuName = "CreateSceneInfos", order = 10)]
public class SceneInfo : ScriptableObject
{
    [SerializeField] List<SceneAsset> _masterScenes;
    public List<SceneAsset> MasterScenes => _masterScenes;
}

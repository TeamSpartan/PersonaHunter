using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "SceneInfo_gen", menuName = "CreateSceneInfos", order = 10)]
public class SceneInfo : ScriptableObject
{
    public List<string> TitleScenesName;
    public List<string> IngameScenesName;
    public List<string> UniqueScenesName;
}

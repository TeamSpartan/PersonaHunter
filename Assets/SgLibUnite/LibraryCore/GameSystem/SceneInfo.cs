using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneInfo_gen", menuName = "CreateSceneInfos", order = 10)]
public class SceneInfo : ScriptableObject
{
    public List<Scene> TitleScenes;
    public List<Scene> IngameScenes;
    public List<Scene> UniqueScenes;
}

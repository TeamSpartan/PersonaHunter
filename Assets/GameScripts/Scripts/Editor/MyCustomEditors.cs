using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class DesignInfoStyle
{
    public static GUIStyle GetExplainGUILayout()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 16;
        style.richText = true;
        
        return style;
    }
}

[CustomEditor(typeof(GameLogic))]
public class GameLogicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("<color=#ff0000> シングルトン インスタンス</color>", DesignInfoStyle.GetExplainGUILayout());
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(MyComponentValidator))]
public class MyComponentValidatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("<color=#ff0000>各シーンに配置されるインスタンス</color>", DesignInfoStyle.GetExplainGUILayout());
        base.OnInspectorGUI();
    }
}

[CustomEditor(typeof(PlayerCam.Scripts.PlayerCameraBrain))]
public class PlayerCameraBrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("<color=#ff0000> シングルトン インスタンス</color>", DesignInfoStyle.GetExplainGUILayout());
        base.OnInspectorGUI();
    }
}

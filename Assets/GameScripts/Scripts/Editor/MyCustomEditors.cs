using System.Collections;
using System.Collections.Generic;
using Player.Input;
using Player.Param;
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

    public static GUIStyle GetMoreExplainGUILayout()
    {
        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Normal;
        style.fontSize = 12;
        style.richText = true;

        return style;
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

[CustomEditor(typeof(SingledObject))]
public class SingledObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("<color=#ff0000> DDOLに登録されているインスタンスです </color>", DesignInfoStyle.GetExplainGUILayout());
        GUILayout.Box(@"<color=#ff0000> 
                            このオブジェクトはボス撃破ムービーで破棄されるべきです。
                            </color>", DesignInfoStyle.GetMoreExplainGUILayout());
        base.OnInspectorGUI();
    }
}

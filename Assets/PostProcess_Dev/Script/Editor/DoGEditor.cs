using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.Rendering;

[CustomEditor(typeof(DoG))]
public class DoGEditor : VolumeComponentEditor
{
    private SerializedDataParameter _Intensity;

    public bool hasAdvancedMode => false;

    public override void OnEnable()
    {
        var o = new PropertyFetcher<DoG>(serializedObject);
        _Intensity = Unpack(o.Find(_ => _.itensity));
    }

    public override void OnInspectorGUI()
    {
        PropertyField(_Intensity);
    }
}

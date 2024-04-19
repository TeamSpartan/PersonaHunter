using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.Rendering;

[CustomEditor(typeof(GaussianBlur))]
public class DoGEditor : VolumeComponentEditor
{
    private SerializedDataParameter _Intensity;
    private SerializedDataParameter _Strength;
    private SerializedDataParameter _Gain;
    private SerializedDataParameter _Inverse;

    public bool hasAdvancedMode => true;

    public override void OnEnable()
    {
        var o = new PropertyFetcher<GaussianBlur>(serializedObject);
        _Intensity = Unpack(o.Find(_ => _.itensity));
        _Strength = Unpack(o.Find(_ => _.strength));
        _Gain = Unpack(o.Find(_ => _.gain));
        _Inverse = Unpack(o.Find(_ => _.invers));
    }

    public override void OnInspectorGUI()
    {
        PropertyField(_Intensity);
        PropertyField(_Strength);
        PropertyField(_Gain);
        PropertyField(_Inverse);
    }
}

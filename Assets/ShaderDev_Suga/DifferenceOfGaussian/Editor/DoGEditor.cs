using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.Rendering;

[CustomEditor(typeof(DifferenceOfGaussian))]
public class DoGEditor : VolumeComponentEditor
{
    private SerializedDataParameter _Intensity;
    private SerializedDataParameter _Strength;
    private SerializedDataParameter _Gain;
    private SerializedDataParameter _Inverse;
    private SerializedDataParameter _Coefficient;
    private SerializedDataParameter _Addition;

    public bool hasAdvancedMode => true;

    public override void OnEnable()
    {
        var o = new PropertyFetcher<DifferenceOfGaussian>(serializedObject);
        _Intensity = Unpack(o.Find(_ => _.itensity));
        _Strength = Unpack(o.Find(_ => _.strength));
        _Gain = Unpack(o.Find(_ => _.gain));
        _Inverse = Unpack(o.Find(_ => _.invers));
        _Coefficient = Unpack(o.Find(_ => _.coefficient));
        _Addition = Unpack(o.Find(_ => _.addition));
    }

    public override void OnInspectorGUI()
    {
        PropertyField(_Intensity);
        PropertyField(_Strength);
        PropertyField(_Gain);
        PropertyField(_Inverse);
        PropertyField(_Coefficient);
        PropertyField(_Addition);
    }
}

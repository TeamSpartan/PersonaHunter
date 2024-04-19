using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.HighDefinition;
using UnityEditor.Rendering;

[CustomEditor(typeof(GrayScale))]
public class GrayScaleEditor : VolumeComponentEditor
{
    private SerializedDataParameter _Itensity;
    
    public bool hasAdvancedMode => false;

    public override void OnEnable()
    {
        base.OnEnable();

        var o = new PropertyFetcher<GrayScale>(serializedObject);
        _Itensity = Unpack(o.Find(x => x.intensity));
    }

    public override void OnInspectorGUI()
    {
        PropertyField(_Itensity);
    }
}

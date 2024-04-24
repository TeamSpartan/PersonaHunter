using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

[Serializable, VolumeComponentMenu("Post-Processing/Custom/DifferenceOfGaussian")]
public class DifferenceOfGaussian : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [FormerlySerializedAs("itensity")] [Tooltip("Controls The Intensity of the Effect")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    
    [Tooltip("Controls The Intensity of the Color")]
    public ClampedFloatParameter coefficient = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Controls The Strength of Blur")]
    public ClampedFloatParameter strength = new ClampedFloatParameter(0f, 0f, 10f);

    [Tooltip("Controls Center Coordinate")]
    public Vector2Parameter center = new Vector2Parameter(Vector2.zero);

    [Tooltip("Controls The Gain")] public ClampedFloatParameter gain = new ClampedFloatParameter(0f, 0f, 15f);

    [Tooltip("Menu Of Invers")] public BoolParameter invers = new BoolParameter(false, false);
    
    [Tooltip("Menu Of Invers")] public BoolParameter addition = new BoolParameter(false, false);

    private Material _material;

    public bool IsActive() => _material != null && intensity.value > 0f;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/DifferenceOfGaussian") != null)
        {
            _material = new Material(Shader.Find("Hidden/Shader/DifferenceOfGaussian"));
        }
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (_material == null) return;

        _material.SetFloat("_Intensity", intensity.value);
        _material.SetFloat("_Strength", strength.value);
        _material.SetFloat("_Gain", gain.value);
        _material.SetFloat("_Coefficient", coefficient.value);
        _material.SetVector("_CenterCoordinate", center.value);
        _material.SetInt("_Inverse", invers.value ? 1 : 0);
        _material.SetInt("_Addition", addition.value ? 1 : 0);
        _material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, _material, destination);
    }

    public override void Cleanup() => CoreUtils.Destroy(_material);
}

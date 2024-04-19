using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-Processing/Custom/GaussianBlur")]
public class GaussianBlur : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls The Intensity oh the Effect")]
    public ClampedFloatParameter itensity = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Controls The Strength of Blur")]
    public ClampedFloatParameter strength = new ClampedFloatParameter(0f, 0f, 10f);

    [Tooltip("Controls The Gain")] public ClampedFloatParameter gain = new ClampedFloatParameter(0f, 0f, 15f);

    [Tooltip("Menu Of Invers")] public BoolParameter invers = new BoolParameter(false, false);

    private Material _material;

    public bool IsActive() => _material != null && itensity.value > 0f;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/GaussianBlur") != null)
        {
            _material = new Material(Shader.Find("Hidden/Shader/GaussianBlur"));
        }
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (_material == null) return;

        _material.SetFloat("_Intensity", itensity.value);
        _material.SetFloat("_Strength", strength.value);
        _material.SetFloat("_Gain", gain.value);
        _material.SetInt("_Inverse", invers.value ? 1 : 0);
        _material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, _material, destination);
    }

    public override void Cleanup() => CoreUtils.Destroy(_material);
}

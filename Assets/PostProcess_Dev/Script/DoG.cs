using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-Processing/Custom/DifferenseOfGaussian")]
public class DoG : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls The Intensity oh the Effect")]
    public ClampedFloatParameter itensity = new ClampedFloatParameter(0f, 0f, 1f);

    private Material _material;

    public bool IsActive() => _material != null && itensity.value > 0f;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/DoG") != null)
        {
            _material = new Material(Shader.Find("Hidden/Shader/DoG"));
        }
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (_material == null) return;
        
        _material.SetFloat("_Intensity", itensity.value);
        _material.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, _material, destination);
    }

    public override void Cleanup() => CoreUtils.Destroy(_material);
}

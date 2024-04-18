using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable, VolumeComponentMenu("Post-Processing/Custom/GrayScale")]
public class GrayScale : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    [Tooltip("Controls the intensity of the effect.")]
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1);

    private Material _mat;

    public bool IsActive()
    {
        return _mat != null && intensity.value > 0f;
    }

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/GrayScale") != null)
        {
            _mat = new Material(Shader.Find("Hidden/Shader/GrayScale"));
        }
    }
    
    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if(_mat == null){return;}
        
        _mat.SetFloat("_Itensity",intensity.value);
        _mat.SetTexture("_InputTexture", source);
        HDUtils.DrawFullScreen(cmd, _mat, destination);
    }

    public override void Cleanup() => CoreUtils.Destroy(_mat);
}

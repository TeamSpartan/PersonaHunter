Shader "Hidden/Shader/DifferenceOfGaussian"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _InputTexture("Main Texture", 2DArray) = "white" {}
    }

    HLSLINCLUDE
    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity, _Strength, _Gain, _Coefficient;
    int _Inverse, _Multiply; // 0 = false , NOT 0 = true
    float2 _CenterCoordinate;
    float _ElapsedTime;
    TEXTURE2D_X(_InputTexture);
    SamplerState sampler_InputTexture;
    #define KERNEL_SIZE 9

    half GaussianFunction(half x, half y, half sigma)
    {
        return exp(-((x * x) + y * y) / (2 * sigma * sigma)) / sqrt(2 * 3.14159 * sigma);
    }

    half GaussianBlur(float2 coord, float2 texelSize, float sigma)
    {
        half result = 0.;
        for (int i = -KERNEL_SIZE / 2; i <= KERNEL_SIZE / 2; ++i)
        {
            for (int j = -KERNEL_SIZE / 2; j <= KERNEL_SIZE / 2; ++j)
            {
                float2 offset = float2(i, j) * texelSize;
                result += _InputTexture.Sample(sampler_InputTexture, float3(coord + offset, 0)).r * GaussianFunction(
                    offset.x * offset.x, offset.y * offset.y, sigma);
            }
        }
        return result;
    }

    half4 CustomPostProcess(Varyings input) : SV_Target0
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        uint2 posSS = input.texcoord * _ScreenSize.xy;
        float2 texel_size = 1. / _ScreenParams.xy;
        half3 rawColor = LOAD_TEXTURE2D_X(_InputTexture, posSS).xyz;
        half b1 = GaussianBlur(input.texcoord, texel_size * _Strength / 2., 5.);
        half b2 = GaussianBlur(input.texcoord, texel_size * _Strength, 7.);

        half diff = (b2 - b1) * _Gain;
        half3 d = diff * _Intensity;
        if (_Inverse)
        {
            if (diff >= 0)
            {
                d = 0 * _Coefficient * _Intensity;
            }
            else
            {
                d = 1 * _Coefficient * _Intensity;
            }
        }

        float3 oc = rawColor;
        
        if (length(input.texcoord.xy - _CenterCoordinate.xy) <= _ElapsedTime)
        {
            if (_Multiply)
            {
                oc = half4(d * rawColor, 1.);
            }
            else
            {
                oc = half4(d, 1.);
            }
        }

        return float4(oc, 1.);
    }
    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
        }
        Pass
        {
            Name "DifferenceOfGaussian"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma fragment CustomPostProcess
            #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
Shader "Custom/SamplePaint"
{
    Properties
    {
        _Tex("Texture", 2D) = ""{}
    }
    
    CGINCLUDE
    #include "UnityCG.cginc"
    #define PI 3.14159
    sampler2D _Tex;

    float flower(float2 p, float n, float radius, float angle, float amplitude)
    {
        float th = atan2(p.y, p.x);
        float d = length(p) - radius + sin(th * n + angle) * amplitude;
        float b = 0.01 / abs(d);
        return b;
    }

    float4 paint(float2 uv)
    {
        float2 p = uv * 2. - 1.;
        float3 col = 0;
        col += flower(p, 6., .9, _Time.y * 1.5, .1) * float3(.1, .01, 1.);
        col += flower(p, 3., .2, PI * .5 - _Time.y * .3, .2) * float3(.1, .5, 0.);
        col += flower(p, 4., .5, _Time.y * .3, .1) * float3(0., 1., 1.);

        col += min(flower(p, 18., .7, -_Time.y * 10., .01), 1.) * .1 * float3(.1, .6, .1);
        col += flower(p, 55., .05, _Time.y * 100., .1) * float3(1., .1, .1);
        return float4(col, 1);
    }
    ENDCG

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
        }
        
        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct fin
            {
                float4 vertex : SV_POSITION;
                float4 textcoord : TEXCOORD0;
            };

            fin vert(appdata v)
            {
                fin o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.textcoord = v.texcoord;
                return o;
            }

            float4 frag(fin IN): SV_Target
            {
                return paint(IN.textcoord.xy);
            }
            ENDCG
        }
    }
}
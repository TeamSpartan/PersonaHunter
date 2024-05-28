Shader "Unlit/Pract_0"
{
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 vert(float4 vertex : POSITION):SV_POSITION
            {
                //このシェーダーはUnityのシーン上に配置したオブジェクトのメッシュを表示するために使われる。
                //その時、vert関数の入力のvertexのxyzの値には、メッシュのオブジェクト空間の座標が入っている。
                //オブジェクト空間の座標とは、メッシュデータに記された頂点の座標のことである。
                return UnityObjectToClipPos(vertex);
            }

            float4 frag(float4 vertex : SV_POSITION):SV_TARGET
            {
                //スカラー値はベクトル型、行列型に暗黙的にキャストできる。
                //その時、そのベクトル/行列の値はすべてそのスカラー値になる。
                //したがって、今回は返り値としてfloat4(1, 1, 1, 1)になる。
                //この値では、rgbaの値がすべて1になり、白色の表示になる。
                return float4(0, 1, 0, 1);
            }
            ENDCG
        }
    }
}
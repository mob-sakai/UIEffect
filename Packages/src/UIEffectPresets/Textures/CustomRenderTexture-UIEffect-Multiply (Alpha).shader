Shader "CustomRenderTexture/UIEffect/Multiply (Alpha)"
{
    Properties
    {
        [Header(Main)]
        _MainTex ("Main Texture", 2D) = "white" {}
        _MainTex_Speed ("Main Texture Speed", Vector) = (0,0,0,0)

        [Header(Detail)]
        _DetailTex ("Detail Texture", 2D) = "white" {}
        _DetailTex_Speed ("Detail Texture Speed", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            Name "Update"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            sampler2D   _MainTex;
            float4      _MainTex_ST;
            float2      _MainTex_Speed;
            sampler2D   _DetailTex;
            float4      _DetailTex_ST;
            float2      _DetailTex_Speed;

            half4 frag(v2f_customrendertexture i) : SV_Target
            {
                half main = tex2D(_MainTex, TRANSFORM_TEX(i.localTexcoord, _MainTex) + _Time.y * _MainTex_Speed.xy).a;
                half detail = tex2D(_DetailTex, TRANSFORM_TEX(i.localTexcoord, _DetailTex) + _Time.y * _DetailTex_Speed.xy).a;
                return main * detail;
            }
            ENDCG
        }
    }
}
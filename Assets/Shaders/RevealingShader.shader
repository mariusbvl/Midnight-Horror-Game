Shader "URP/Revealing Under Light"
{
    Properties
    {
        _MyColor("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _LightDirection("Light Direction", Vector) = (0,0,1,0)
        _LightPosition("Light Position", Vector) = (0,0,0,0)
        _LightAngle("Light Angle", Range(0,180)) = 45
        _StrengthScaler("Strength", Float) = 50
        _LightEnabled("Light Enabled", Float) = 0.0
    }

    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            half _Glossiness;
            half _Metallic;
            half4 _MyColor;
            float4 _LightPosition;
            float4 _LightDirection;
            float _LightAngle;
            float _StrengthScaler;
            float _LightEnabled;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.position = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = TransformObjectToWorld(v.vertex).xyz;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float3 Dir = normalize(_LightPosition.xyz - i.worldPos);
                float Scale = dot(Dir, -_LightDirection.xyz);
                float Strength = Scale - cos(_LightAngle * (3.14 / 360.0));
                Strength = saturate(Strength * _StrengthScaler) * _LightEnabled;
                half4 RC = tex2D(_MainTex, i.uv) * _MyColor;
                half3 Albedo = RC.rgb;
                half3 Emission = RC.rgb * RC.a * Strength;
                half Metallic = _Metallic;
                half Smoothness = _Glossiness;
                half Alpha = Strength * RC.a;

                return half4(Albedo + Emission, Alpha);
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}

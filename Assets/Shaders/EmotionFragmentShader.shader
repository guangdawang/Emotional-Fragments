Shader "URP/EmotionFragment"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        _EmissiveColor("Emissive Color", Color) = (0, 0, 0, 1)
        _EmissiveIntensity("Emissive Intensity", Range(0, 5)) = 1.0
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
        
        // 情绪参数
        _ConstraintInfluence("Constraint Influence", Range(0, 1)) = 0.0
        _AnxietyInfluence("Anxiety Influence", Range(0, 1)) = 0.0
        _HopeInfluence("Hope Influence", Range(0, 1)) = 0.0
        
        // 情绪颜色
        _ConstraintColor("Constraint Color", Color) = (0.17, 0.24, 0.31, 1)    // 深蓝色
        _AnxietyColor("Anxiety Color", Color) = (0.91, 0.23, 0.17, 1)          // 红色
        _HopeColor("Hope Color", Color) = (0.18, 0.8, 0.44, 1)                 // 绿色
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Unity渲染功能
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
            };

            // 属性
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _EmissiveColor;
                float _EmissiveIntensity;
                float _Smoothness;
                float _Metallic;
                
                // 情绪参数
                float _ConstraintInfluence;
                float _AnxietyInfluence;
                float _HopeInfluence;
                
                // 情绪颜色
                float4 _ConstraintColor;
                float4 _AnxietyColor;
                float4 _HopeColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS);
                
                OUT.positionCS = vertexInput.positionCS;
                OUT.normalWS = normalInput.normalWS;
                OUT.viewDir = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                OUT.uv = IN.uv;
                OUT.shadowCoord = TransformWorldToShadowCoord(vertexInput.positionWS);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 基础光照计算
                Light mainLight = GetMainLight(IN.shadowCoord);
                half3 normal = normalize(IN.normalWS);
                half3 viewDir = normalize(IN.viewDir);
                
                // 基础颜色混合情绪颜色
                half4 emotionColor = _BaseColor;
                emotionColor.rgb += _ConstraintInfluence * _ConstraintColor.rgb;
                emotionColor.rgb += _AnxietyInfluence * _AnxietyColor.rgb;
                emotionColor.rgb += _HopeInfluence * _HopeColor.rgb;
                
                // BRDF计算
                BRDFData brdfData;
                InitializeBRDFData(emotionColor, _Metallic, _Smoothness, 1.0, brdfData);
                
                half3 diffuse = LightingLambert(mainLight.color, mainLight.direction, normal);
                half3 specular = LightingSpecular(mainLight.color, mainLight.direction, normal, viewDir, brdfData);
                
                // 自发光计算
                half3 emission = _EmissiveColor.rgb * _EmissiveIntensity;
                emission += _HopeInfluence * _HopeColor.rgb * _EmissiveIntensity * 0.5;
                
                // 最终颜色
                half3 finalColor = diffuse + specular + emission;
                
                return half4(finalColor, emotionColor.a);
            }
            ENDHLSL
        }
        
        // 阴影投射Pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            float3 _LightDirection;
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            float4 ShadowPassVertex(Attributes input) : SV_POSITION
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                return TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
            }

            half4 ShadowPassFragment() : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/Lit"
    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}
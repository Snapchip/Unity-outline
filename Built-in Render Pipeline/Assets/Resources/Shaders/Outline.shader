Shader "Hidden/Outline"
{
    HLSLINCLUDE
    #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);    
    TEXTURE2D_SAMPLER2D(_BlurredTex, sampler_BlurredTex);  
    TEXTURE2D_SAMPLER2D(_ObjectsTex, sampler_ObjectsTex);
    float4 _OutlineColor;
    float4 Frag(VaryingsDefault i) : SV_Target
    {        
        float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
        float4 objects = SAMPLE_TEXTURE2D(_ObjectsTex, sampler_ObjectsTex, i.texcoord);
        float4 blurred = SAMPLE_TEXTURE2D(_BlurredTex, sampler_BlurredTex, i.texcoord);        
        float4 outline = saturate(blurred - objects); 

        outline = outline.rgbr;
        outline.rgb *= _OutlineColor.rgb;
        outline *= _OutlineColor.a;
        return color * (1 - outline.a) + outline;
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
                #pragma vertex VertDefault
                #pragma fragment Frag
            ENDHLSL
        }
    }
}
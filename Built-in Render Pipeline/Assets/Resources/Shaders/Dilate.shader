Shader "Hidden/Outline/Dilate" {
	HLSLINCLUDE
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float4 uv01 : TEXCOORD1;
		float4 uv23 : TEXCOORD2;		
	};

	float4 _Offsets;	
	v2f vert(AttributesDefault v) {
		v2f o;
		o.pos = float4(v.vertex.xy, 0.0, 1.0);
		o.uv = TransformTriangleVertexToUV(v.vertex.xy);
	#if UNITY_UV_STARTS_AT_TOP
		o.uv = o.uv * float2(1.0, -1.0) + float2(0.0, 1.0);
	#endif					
		o.uv01 = o.uv.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1);
		o.uv23 = o.uv.xyxy.xyxy + _Offsets.xyxy * float4(1, 1, -1, -1) * 2.0;
		return o;
	}

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	half4 frag(v2f i) : COLOR{
		half4 c1 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv); 
		half4 c2 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.xy);
		half4 c3 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv01.zw);
		half4 c4 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.xy);
		half4 c5 = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv23.zw);
		c1 = max(c1, c2);
		c3 = max(c3, c4);
		c5 = max(c1, c5);
		return max(c3, c5);
	}
	ENDHLSL

	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Pass
		{
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
			ENDHLSL
		}
	}
}

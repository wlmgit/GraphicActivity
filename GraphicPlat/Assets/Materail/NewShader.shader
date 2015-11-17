Shader "Custom/NewShader" {
properties
{
_color("Color",COLOR)=(0,1,1,0.3)
_colorback("Color",COLOR)=(0,1,0,0.3)
}
SubShader
{
Tags{"Queue"="Transparent"}
//		Pass
//		{
//		Cull front
//		CGPROGRAM
//		// Physically based Standard lighting model, and enable shadows on all light types
//		#pragma vertex vert
//
//		// Use shader model 3.0 target, to get nicer looking lighting
//		#pragma fragment frag
//		float4 _color;
//		struct vertexInput
//		{
//		float4 vertex:POSITION;
//		};
//		struct vertexOutput
//		{
//		float4 pos:SV_POSITION;
//		float4 posInObjectCoord:TEXCOORD0;
//		};
//		
//		vertexOutput vert(vertexInput input)
//		{
//		vertexOutput output;
//		output.pos=mul(UNITY_MATRIX_MVP,input.vertex);
//		output.posInObjectCoord=input.vertex;
//		return output;
//		}
//		float4 frag(vertexOutput input):COLOR
//		{
//		if(input.posInObjectCoord.y>0.0)
//		{
//		discard;
//		}
//		return _color;
//		}
//		
//		ENDCG 
//		}
Pass
		{
//		Cull off
Cull Front
ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma fragment frag
		float4 _color;
//		struct vertexInput
//		{
//		float4 vertex:POSITION;
//		};
//		struct vertexOutput
//		{
//		float4 pos:SV_POSITION;
//		float4 posInObjectCoord:TEXCOORD0;
//		};
		
		float4 vert(float4 vertexpos:POSITION):SV_POSITION
		{
//		vertexOutput output;
		return mul(UNITY_MATRIX_MVP,vertexpos);
//		output.posInObjectCoord=input.vertex;
//		return output;
		}
		float4 frag(void):COLOR
		{
//		float dis=sqrt(input.posInObjectCoord.x*input.posInObjectCoord.x+input.posInObjectCoord.y*input.posInObjectCoord.y);
//        float dis=sqrt(input.posInObjectCoord.x*input.posInObjectCoord.x);
//		if(dis>0.5)
//		{
//		discard;
//		}
		return _color;
		}
		
		ENDCG 
		}
		Pass
		{
//		Cull off
Cull Back
ZWrite Off
Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma fragment frag
		float4 _colorback;
//		struct vertexInput
//		{
//		float4 vertex:POSITION;
//		};
//		struct vertexOutput
//		{
//		float4 pos:SV_POSITION;
//		float4 posInObjectCoord:TEXCOORD0;
//		};
		
		float4 vert(float4 vertexpos:POSITION):SV_POSITION
		{
//		vertexOutput output;
		return mul(UNITY_MATRIX_MVP,vertexpos);
//		output.posInObjectCoord=input.vertex;
//		return output;
		}
		float4 frag(void):COLOR
		{
//		float dis=sqrt(input.posInObjectCoord.x*input.posInObjectCoord.x+input.posInObjectCoord.y*input.posInObjectCoord.y);
//        float dis=sqrt(input.posInObjectCoord.x*input.posInObjectCoord.x);
//		if(dis>0.5)
//		{
//		discard;
//		}
		return _colorback;
		}
		
		ENDCG 
		}
}
}

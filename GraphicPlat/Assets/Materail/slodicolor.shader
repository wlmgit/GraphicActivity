Shader "Custom/slodicolor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,0.5)
	}
	SubShader {
		pass
		{
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma fragment frag
		
		float4 _Color;
		
		float4 vert(float4 vertexpos:POSITION):SV_POSITION
		{
//		float4 output=vertexpos+(0.1,0.1,0.1,0.1);
//		return output;
		return mul(UNITY_MATRIX_MVP,float4(1, 1, 1, 1)*vertexpos);
		}
		float4 frag(void):COLOR
		{
		_Color.a=0.5;
		return _Color;
		}

		ENDCG
		}
	} 
	FallBack "Diffuse"
}

﻿Shader "Custom/taetshader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct testTertex
		{
		float4 position:POSITION;
		float4 _color:COLOR;
		};
		
		testTertex mFun(float2 pos:POSITION)
		{
		   testTertex OUT;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

Shader "FishAnimation"
{
	Properties
	{
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_Speed("Speed", Range( 0 , 10)) = 0
		_Scale("Scale", Range( 0 , 1)) = 0.33
		_Yaw("Yaw", Float) = 0.5
		_Roll("Roll", Float) = 0.5
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 2.0
		#pragma multi_compile_instancing
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float _Speed;
		uniform float _Yaw;
		uniform float _Roll;
		uniform float _Scale;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			v.vertex.xyz += ( ( sin( ( ( _Time.w * _Speed ) + ( ase_vertex3Pos.z * _Yaw ) + ( ase_vertex3Pos.y * _Roll ) ) ) * _Scale ) * float3(1,0,0) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord;
			o.Albedo = tex2D( _MainTex, uv_MainTex ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
}
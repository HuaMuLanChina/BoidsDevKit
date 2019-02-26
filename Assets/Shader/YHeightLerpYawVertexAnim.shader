Shader "BoidsDevKit/YHeightLerpYawVertexAnim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Height("Heigth", float) = 0.5
		_Col0("Col0", Color) = (0.5,0.5,0.5,1)
		_Col1("Col1", Color) = (1, 1, 1, 1)
		_SpeedMult("SpdMuti", float) = 1
		_Speed("Speed", float) = 1
		_Yaw("Yaw Pose", range(-1,1))=0
		_YawOffset("Yaw Offset", float) = 0
		_Phase("Phase", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 key0 : TEXCOORD1;
				float3 key1 : TEXCOORD2;
				float3 Yawkey0	: TEXCOORD3;
				float3 Yawkey1	: COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float height : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Height;
			fixed4 _Col0;
			fixed4 _Col1;
			float _SpeedMult;
			float _YawOffset;

			UNITY_INSTANCING_CBUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
				UNITY_DEFINE_INSTANCED_PROP(float, _Phase)
				UNITY_DEFINE_INSTANCED_PROP(float, _Yaw)
			UNITY_INSTANCING_CBUFFER_END

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				o.height = v.vertex.y;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float frequent =  UNITY_ACCESS_INSTANCED_PROP(_Speed)*_SpeedMult;
				float t = abs(sin((_Time.y + UNITY_ACCESS_INSTANCED_PROP(_Phase))*frequent));
				v.vertex.xyz = lerp(v.key0, v.key1, t);
				float yaw = UNITY_ACCESS_INSTANCED_PROP(_Yaw);
				float t2 = saturate(abs(yaw)* _YawOffset);
				float s = step(0,yaw);
				v.vertex.xyz = lerp(v.vertex.xyz, v.Yawkey0, t2*(1-s));
				v.vertex.xyz = lerp(v.vertex.xyz, v.Yawkey1, t2 * s);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mixedCol = lerp(_Col1, _Col0, i.height/_Height);
				return col * mixedCol * 2.0;
			}
			ENDCG
		}
	}
}

Shader "BoidsDevKit/YHeightLerpVertexAnim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Height("Heigth", float) = 0.5
		_Col0("Col0", Color) = (0.5,0.5,0.5,1)
		_Col1("Col1", Color) = (1, 1, 1, 1)
		_SpeedMult("SpdMuti", float) = 1
		_Speed("Speed", float) = 1
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float height : TEXCOORD1;
				float speed  : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Height;
			fixed4 _Col0;
			fixed4 _Col1;
			float _SpeedMult;

			UNITY_INSTANCING_CBUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(float, _Speed)
				UNITY_DEFINE_INSTANCED_PROP(float, _Phase)
			UNITY_INSTANCING_CBUFFER_END

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				o.height = v.vertex.y;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.speed = UNITY_ACCESS_INSTANCED_PROP(_Speed);
				float t = abs(sin((_Time.y + UNITY_ACCESS_INSTANCED_PROP(_Phase))*UNITY_ACCESS_INSTANCED_PROP(_Speed)*_SpeedMult));
				v.vertex.xyz = lerp(v.key0, v.key1, t);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				float height = _Height / 10;

				fixed4 mixedCol = lerp(_Col1, _Col0, i.height/height);
				return col * mixedCol * 2.0;
			}
			ENDCG
		}
	}
}

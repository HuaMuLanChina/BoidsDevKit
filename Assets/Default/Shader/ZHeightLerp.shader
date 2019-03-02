// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "BoidsDevKit/ZHeightLerp"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Height("Heigth", float) = 0.5
		_Col0("Col0", Color) = (0.5,0.5,0.5,1)
		_Col1("Col1", Color) = (1, 1, 1, 1)
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
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float height : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Height;
			fixed4 _Col1;

			UNITY_INSTANCING_CBUFFER_START(Props)
				UNITY_DEFINE_INSTANCED_PROP(fixed4, _Col0)	// Make _Color an instanced property (i.e. an array)
			UNITY_INSTANCING_CBUFFER_END

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.height = v.vertex.z;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				UNITY_SETUP_INSTANCE_ID (i);
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 mixedCol = lerp(_Col1, UNITY_ACCESS_INSTANCED_PROP(_Col0), i.height/_Height);
				return col * mixedCol * 2.0;
			}
			ENDCG
		}
	}
}

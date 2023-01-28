// Dither rendering logic developed by Joseph Kalathil, modified for personal use
// Attribution (https://gist.github.com/josephbk117/8344b204588f328e50556a45db042e9c)
// See attached Readme & License for details

Shader "Hidden/Dither"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			int _ColourDepth;
			float _DitherStrength;

			static const float4x4 ditherTable = float4x4
			(
				-4.0, 0.0, -3.0, 1.0,
				2.0, -2.0, 3.0, -1.0,
				-3.0, 1.0, -4.0, 0.0,
				3.0, -1.0, 2.0, -2.0
			);

			fixed4 frag (v2f i) : SV_TARGET
			{
				fixed4 col = tex2D(_MainTex,i.uv);
				uint2 pixelCoord = i.uv*_ScreenParams.xy; //warning that modulus is slow on integers, so use uint
				col += ditherTable[pixelCoord.x % 4][pixelCoord.y % 4] * _DitherStrength;
				return round(col * _ColourDepth) / _ColourDepth;
			}
			ENDCG
		}
	}
}
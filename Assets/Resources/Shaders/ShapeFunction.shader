Shader "Hidden/ShapeFunction"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "./Includes/Math.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 viewVector : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float2 uv = v.uv * 2 - 1; // Re-maps the uv so that it is centered on the screen
                float3 viewVector = mul(unity_CameraInvProjection, float4(uv.x, uv.y, 0, -1));
                o.viewVector = mul(unity_CameraToWorld, float4(viewVector, 0));

                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            
            float3 _Position;

            float _StepSize;
            int _NumSteps;

            float GetDist(float3 p)
            {
                float planeDist = p.y;
                float sphereDist = length(p - _Position) - 1;

                return sphereDist;
            }

            float RaymarchScene(float3 rayOrigin, float3 rayDir, int numSteps, float stepSize)
            {
                float3 rayPos = rayOrigin;
                float result = 0;

                for (float i = 0; i < numSteps; i++)
                {
                    // Move the ray
                    rayPos += rayDir * stepSize;

                    // Evaluate sdf
                    float dist = GetDist(rayPos);
                    result += dist;

                    if (dist < 0)
                    {
                        break;
                    }
                }

                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 originalCol = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float result = RaymarchScene(rayOrigin, rayDir, _NumSteps, _StepSize);

                return result;
            }
            ENDCG
        }
    }
}

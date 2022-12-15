Shader "Hidden/BlackHole"
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
            float _Mass;

            float _StepSize;
            int _NumSteps;
            float _MaxDistortRadius;
            float _DistortFadeOutDistance;

            float4 RaymarchScene(float3 rayOrigin, float3 rayDir, float stepSize, int numSteps, float schwarzschildRadius)
            {
                float3 rayPos = rayOrigin;
                int radiusCheck = 0;

                for (float i = 0; i < numSteps; i++)
                {
                    // Distort the ray according to Newton's law of universal gravitation
                    float3 difference = _Position - rayPos;
                    float sqrLength = pow(length(difference), 2);

                    float3 direction = normalize(difference);
                    float3 acceleration = direction * gravitationalConst * ((_Mass * 1e-13) / sqrLength);

                    // Move the ray according to this force
                    rayDir += acceleration * stepSize;
                    rayPos += rayDir;

                    if (distance(rayPos, _Position) <= _MaxDistortRadius){
                        //radiusCheck = 1;
                    }

                    if (distance(rayPos, _Position) < schwarzschildRadius) {
                        //break;
                    }
                }

                return float4(rayPos, radiusCheck);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 originalCol = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float2 uv = i.uv;

                float speedOfLightSqrd = pow(speedOfLight, 2);
                float schwarzschildRadius = (2 * gravitationalConst * _Mass) / speedOfLightSqrd; // Equation Source: https://en.wikipedia.org/wiki/Schwarzschild_radius

                float4 rayPos = RaymarchScene(rayOrigin, rayDir, _StepSize, _NumSteps, schwarzschildRadius);

                // If we are within the Schwarzschild Radius, render the event horizon
                float2 hitInfo = raySphere(_Position, schwarzschildRadius, rayOrigin, rayDir);
                if (hitInfo.y > 0) {
                    return float4(0, 0, 0, 0); // 1 - hitInfo.y;
                }

                // ...otherwise, bend the scene around the singularity
                // To do so we must warp the screen space UV's according the effects of Gravitational Lensing

                float3 distortedRayDir = normalize(rayPos - rayOrigin);
                float4 rayCameraSpace = mul(unity_WorldToCamera, float4(distortedRayDir, 0));
                float4 rayUVProjection = mul(unity_CameraProjection, float4(rayCameraSpace));

                float2 distortedScreenUV = float2(rayUVProjection.x + 0.5, rayUVProjection.y / 1.5 + 0.5);
                uv = distortedScreenUV;

                //if (rayPos.w == 1){
                //    uv = distortedScreenUV;
                //}
                //else
                //{
                //    float dist = distance(rayPos, _Position);
                //    uv = lerp(uv, distortedScreenUV, dist * _DistortFadeOutDistance);
                //}

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}

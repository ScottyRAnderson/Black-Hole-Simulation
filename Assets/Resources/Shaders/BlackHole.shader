Shader "Hidden/BlackHole"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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

            v2f vert(appdata v)
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
            float _SchwarzschildRadius;

            float4 _EventHorizonColor;
            float _StepSize;
            int _NumSteps;
            float _MaxDistortRadius;
            float _DistortFadeOutDistance;

            float4 RaymarchScene(float3 rayOrigin, float3 rayDir, int numSteps, float stepSize, float mass)
            {
                float3 rayPos = rayOrigin;

                for (float i = 0; i < numSteps; i++)
                {
                    // Distort the ray according to Newton's law of universal gravitation
                    float3 difference = _Position - rayPos;
                    float sqrLength = pow(length(difference), 2);

                    float3 direction = normalize(difference);
                    float3 acceleration = direction * gravitationalConst * ((mass * 1e-13) / sqrLength);

                    // Move the ray according to this force
                    rayDir += acceleration * stepSize;
                    rayPos += rayDir;

                    if (distance(rayPos, _Position) < _SchwarzschildRadius) {
                        //return float4(rayPos, 1);
                    }

                    if (distance(rayPos, _Position) > _MaxDistortRadius) {
                        return float4(rayPos, 0);
                    }
                }

                return float4(rayPos, 0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 originalCol = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float2 uv = i.uv;

                float speedOfLightSqrd = pow(speedOfLight, 2);
                float mass = (_SchwarzschildRadius * speedOfLightSqrd) / (gravitationalConst * 2); // Re-arranged equation of Schwarzschild Radius

                // If we are within the Schwarzschild Radius, render the event horizon

                float2 boundsHitInfo = raySphere(_Position, _MaxDistortRadius, rayOrigin, rayDir);
                float dstToBounds = boundsHitInfo.x;
                float dstThroughBounds = boundsHitInfo.y;

                // If we are looking through the bounds render the black hole
                if (dstThroughBounds > 0)
                {
                    // Identify the event horizon
                    float2 hitInfo = raySphere(_Position, _SchwarzschildRadius * 1.6f, rayOrigin, rayDir);
                    float dstToEH = hitInfo.x;
                    float dstThroughEH = hitInfo.y;

                    // Move the rayOrigin to the first point within the distortion bounds
                    rayOrigin += rayDir * dstToBounds;
                    float4 rayPos = RaymarchScene(rayOrigin, rayDir, _NumSteps, _StepSize, mass);

                    // If we are within the Schwarzschild Radius, render the event horizon
                    if (dstThroughEH > 0 || rayPos.w == 1) {
                        return _EventHorizonColor;
                    }

                    // ...otherwise, bend the scene around the singularity
                    // To do so we must warp the screen space UV's according the effects of Gravitational Lensing

                    float3 distortedRayDir = normalize(rayPos - rayOrigin);
                    float4 rayCameraSpace = mul(unity_WorldToCamera, float4(distortedRayDir, 0));
                    float4 rayUVProjection = mul(unity_CameraProjection, float4(rayCameraSpace));

                    float2 distortedScreenUV = float2(rayUVProjection.x + 0.5, rayUVProjection.y / 1.5 + 0.5);
                    uv = distortedScreenUV;

                    return tex2D(_MainTex, uv);
                }
                return originalCol;
            }
            ENDCG
        }
    }
}
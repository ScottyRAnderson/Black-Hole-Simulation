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
            #pragma shader_feature DEBUGFADE

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
            sampler2D _CameraDepthTexture;

            float3 _Position;
            float _SchwarzschildRadius;

            float4 _EventHorizonColor;
            float _StepSize;
            float _MaxDistortRadius;
            float _DistortFadeOutDistance;
            float _FadePower;

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
                    float2 hitInfo = raySphere(_Position, _SchwarzschildRadius, rayOrigin, rayDir); // _SchwarzschildRadius * 1.6f
                    float dstToEH = hitInfo.x;
                    float dstThroughEH = hitInfo.y;

                    // ATTEMPTS TO EXCLUDE FORGROUND GEOMETRY FROM LENSING - RESULTS IN WEIRD RENDERING
                    // Ensure we account for scene depth (so that event horizon is not rendered overtop of everything)
                    //float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                    //float sceneDepth = LinearEyeDepth(sceneDepthNonLinear) * length(i.viewVector);
                    //float depthToEH = sceneDepth - dstToEH;
                    //float dstThroughEH = min(hitInfo.y, depthToEH);

                    // Linear depth values close to 1 will be the skybox, so we should exlude these
                    //float depth01 = Linear01Depth(sceneDepthNonLinear);
                    //if (sceneDepth > 0 && depth01 < 0.999){
                    //    return originalCol;
                    //}

                    // Move the rayOrigin to the first point within the distortion bounds
                    rayOrigin += rayDir * dstToBounds;
                    int numSteps = _MaxDistortRadius * 2; // There should be enough steps to make it entirely through the distortion radius
                    float4 rayPos = RaymarchScene(rayOrigin, rayDir, numSteps, _StepSize, mass);

                    // Warp the screen space UV's according the effects of Gravitational Lensing
                    float3 distortedRayDir = normalize(rayPos - rayOrigin);
                    float4 rayCameraSpace = mul(unity_WorldToCamera, float4(distortedRayDir, 0));
                    float4 rayUVProjection = mul(unity_CameraProjection, float4(rayCameraSpace));

                    float2 distortedScreenUV = float2(rayUVProjection.x / 2 + 0.5, rayUVProjection.y / 2 + 0.5);
                    
                    // If we are within the fade-out distance, blend the uv's
                    float blendFactor = 0;
                    if (dstThroughBounds <= _DistortFadeOutDistance)
                    {
                        blendFactor = pow(remap01(_DistortFadeOutDistance, 0, dstThroughBounds), _FadePower);

                        #if DEBUGFADE
                        return blendFactor;
                        #endif
                    }
                    
                    float2 uv = lerp(distortedScreenUV, i.uv, blendFactor);
                    float4 finalCol = tex2D(_MainTex, uv);

                    // If we are within the Schwarzschild Radius, render the event horizon
                    if (dstThroughEH > 0 || rayPos.w == 1)
                    {
                        if (dstThroughEH < 1 && rayPos.w == 0){
                            return finalCol;
                        }
                        return _EventHorizonColor;
                    }

                    // ...otherwise, bend the scene around the singularity
                    return finalCol;
                }
                return originalCol;
            }
            ENDCG
        }
    }
}
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
            #pragma shader_feature DEBUGFALLOFF

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

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            float _StepSize;
            int _StepCount;
            float _GravitationalConst;

            float _MaxEffectRadius;
            float _EffectFadeOutDist;
            float _EffectFalloff;

            float3 _EventHorizonColor;
            float3 _AccretionColor;
            float _AccretionFalloff;
            float _AccretionIntensity;
            float _AccretionOuterRadius;
            float _AccretionInnerRadius;
            float _AccretionWidth;
            float3 _AccretionDir;

            float _NoiseScale;
            int _Octaves;
            float _Persistance;
            float _Lacunarity;
            float _HeightScalar;
            float _ScrollRate;
            sampler2D _AccretionTex;

            float3 _Position;
            float _SchwarzschildRadius;

            float4 _TestVar;

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

            float sample3DPerlin(float3 p)
            {
                float perlinValue = 0;
                float amplitude = 1;
                float frequency = 1;
                float contribution = 1;

                // Loop through each octave and contribute
                for (int o = 0; o < _Octaves; o++)
                {
                    float3 offsetSample = p / _NoiseScale * frequency;

                    float noiseValue = (perlinNoise(offsetSample) + 0.5f) * contribution;
                    perlinValue += noiseValue * amplitude;

                    amplitude *= _Persistance;
                    frequency *= _Lacunarity;
                    contribution /= 2.0f;
                }

                perlinValue *= _HeightScalar;
                return perlinValue;
            }

            float4 accretionColor(float3 samplePos, float transmittance)
            {
                float3 finalCol = 0;

                // If accretion disc not sampled, return 0
                if (transmittance < 1){
                    return 0;
                }

                float distFromDisc = dot(_AccretionDir, samplePos - _Position);
                float distFromCenter = distance(samplePos, _Position);

                float3 radialCoord = cartesianToRadial(samplePos, distFromCenter, distFromDisc);
                radialCoord.y += _Time.x * _ScrollRate;

                float perlinValue = sample3DPerlin(radialCoord);

                float alpha = 1 - perlinValue;

                float falloff = pow(remap01(_AccretionOuterRadius, 0, distFromCenter), _AccretionFalloff);
                finalCol = lerp(_AccretionColor, 1 * _AccretionIntensity, falloff);
                finalCol *= perlinValue;

                return float4(finalCol, alpha);
            }

            float4 sampleGasCloud(float3 rayPos)
            {

            }

            float sampleAccretion(float3 rayDir, float3 rayPos)
            {
                float3 p1 = _Position - 0.5 * _AccretionWidth * _AccretionDir;
                float3 p2 = _Position + 0.5 * _AccretionWidth * _AccretionDir;

                float discDst = intersectDisc(rayPos, rayDir, p1, p2, _AccretionDir, _AccretionOuterRadius, _AccretionInnerRadius);
                return discDst;
            }

            void warpRay(inout float3 rayDir, float3 rayPos, float stepSize)
            {
                float3 centerDir = _Position - rayPos;
                float distance = length(centerDir);
                float sqrLength = pow(distance, _TestVar.x); //2
                centerDir = normalize(centerDir);

                // Application of Newton's law of universal gravitation (modified variant)
                // Because our view ray has no mass, we assume m1 * m2 =~ 1
                float force = _GravitationalConst * (1 / sqrLength);

                float3 acceleration = rayDir + (centerDir * force * stepSize);
                rayDir = normalize(acceleration); // We only want velocity so normalize the acceleration
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Determine the origin & direction of our ray to march through the scene
                float4 originalCol = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);
            
                float speedOfLightSqrd = pow(speedOfLight, 2);
                float singularityMass = (_SchwarzschildRadius * speedOfLightSqrd) / (gravitationalConst * 2); // Re-arranged equation of Schwarzschild Radius
            
                // Figure out where the effect bounds are
                float2 boundsHitInfo = raySphere(_Position, _MaxEffectRadius, rayOrigin, rayDir);
                float dstToBounds = boundsHitInfo.x;
                float dstThroughBounds = boundsHitInfo.y;
            
                // If we are looking through the bounds render the black hole
                if (dstThroughBounds > 0)
                {            
                    // Move the rayOrigin to the first point within the distortion bounds
                    rayOrigin += rayDir * dstToBounds;
                    float3 rayPos = rayOrigin;

                    float transmittance = 0;
                    float3 discSample = float3(maxFloat, 0, 0);
                    float4 gasColor = 0;

                    // March our ray through the scene
                    for (float s = 0; s < _StepCount; s++)
                    {
                        // Step ray forward
                        warpRay(rayDir, rayPos, _StepSize);
                        rayPos += rayDir * _StepSize;

                        // Check for accretion disc
                        if (transmittance < 1)
                        {
                            float discDst = sampleAccretion(rayDir, rayPos);
                            if (discDst < _StepSize)
                            {
                                transmittance = 1;
                                discSample = rayPos + rayDir * discDst;
                            }
                        }

                        if (distance(rayPos, _Position) < _SchwarzschildRadius) {
                            //break;
                        }
                        if (distance(rayPos, _Position) > _MaxEffectRadius) {
                            break;
                        }
                    }

                    //FIGURE THIS OUT YOURSELF:
                    float2 distortedScreenUV = float2(0, 0);
                    if (true)
                    {
                        // Convert the rayPos to a screen space position which can be used to read the screen UVs
                        float3 distortedRayDir = normalize(rayPos - rayOrigin);
                        float4 rayCameraSpace = mul(unity_WorldToCamera, float4(distortedRayDir, 0));
                        float4 rayUVProjection = mul(unity_CameraProjection, float4(rayCameraSpace));
                        distortedScreenUV = float2(rayUVProjection.x / 2 + 0.5, rayUVProjection.y / 2 + 0.5);
                    }
            
                    // If we are within the fade-out distance, blend the uv's
                    float blendFactor = 0;
                    if (dstThroughBounds <= _EffectFadeOutDist)
                    {
                        blendFactor = pow(remap01(_EffectFadeOutDist, 0, dstThroughBounds), _EffectFalloff);
                        #if DEBUGFALLOFF
                        return blendFactor;
                        #endif
                    }
                    
                    // Interpolate between the original uv and the warped uv according to the blendFactor
                    float2 uv = lerp(distortedScreenUV, i.uv, blendFactor);
                    float3 finalCol = tex2D(_MainTex, uv);
                    float alpha = 1;


                    float4 discColor = accretionColor(discSample, transmittance);
                    alpha = discColor.w;

                    finalCol += discColor;
                    return float4(finalCol, alpha);
                }
            
                // If we are not looking through the render bounds, just return the un-modified scene color
                return originalCol;
            }
            ENDCG
        }
    }
}
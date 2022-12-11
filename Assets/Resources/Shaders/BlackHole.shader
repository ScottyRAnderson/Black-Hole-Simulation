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

            static const float maxFloat = 3.402823466e+38;
            static const float speedOfLight = 299792458;
            static const float gravitationalConst = 0.000000000066743;

            sampler2D _MainTex;
            float3 _Position;
            float _Mass;
            float2 _ScreenPos;

            float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir)
            {
                float t = dot(sphereCentre - rayOrigin, rayDir);
                float3 p = rayOrigin + rayDir * t;
                float y = distance(sphereCentre, p);
            
                // If the ray intersects with the sphere
                if (y < sphereRadius)
                {
                    float x = sqrt(sphereRadius * sphereRadius - y * y);
                    float t1 = t - x;
                    float t2 = t + x;
            
                    float3 nearP = rayOrigin + rayDir * t1;
                    float3 farP = rayOrigin + rayDir * t2;
            
                    float nearDst = distance(rayOrigin, nearP);
                    float farDst = distance(rayOrigin, farP);
            
                    return float2(farDst, t2);
                }

                return float2(maxFloat, 0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 origionalCol = tex2D(_MainTex, i.uv);
                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

                float2 screenPos = ComputeScreenPos(float4(_Position, 1)).xy;

                float2 uv = i.uv -_ScreenPos;

                float distToCam = distance(_Position, _WorldSpaceCameraPos); // Dl
                float uvDist = length(float2(uv.x * 2, uv.y)); // Distance from the center of the screen

                float speedOfLightSqrd = pow(speedOfLight, 2);
                float schwarzschildRadius = (2 * gravitationalConst * _Mass) / speedOfLightSqrd; // Equation Source: https://en.wikipedia.org/wiki/Schwarzschild_radius

                // If we are within the Schwarzschild Radius, render the event horizon
                float2 hitInfo = raySphere(_Position, pow(schwarzschildRadius, 2), rayOrigin, rayDir);
                if (hitInfo.y > 0){
                    return origionalCol + 1 - hitInfo.x;
                }

                // ...otherwise, bend the scene around the singularity
                // To do so we must warp the screen space UV's according the effects of Gravitational Lensing

                // We cannot directly compute the lensing equation due to the 2D nature of the screen
                // Therefore we must assume that Dl << Ds and Dl << Dls, thus the deflection angle is inversely proportional to the root of distances
                float einsteinRing = 1 / pow(uvDist * sqrt(distToCam), 2) * schwarzschildRadius * 2;
                uv *= 1 - einsteinRing;
                uv += _ScreenPos;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}

int _AccretionQuality;
float3 _AccretionMainColor;
float3 _AccretionInnerColor;
float _AccretionColorShift;
float _AccretionFalloff;
float _AccretionIntensity;
float _AccretionOuterRadius;
float _AccretionInnerRadius;
float _AccretionWidth;
float _AccretionSlope;
float3 _AccretionDir;

sampler3D _AccretionNoiseTex;

int _NoiseLayerCount;
float _SampleScales[4];
float _ScrollRates[4];

float _GasCloudThreshold;
float _TransmittancePower;
float _DensityPower;

float _BlueShiftPower;

// Samples from a 3D noise texture with a layered approach of varying scale and scroll rates
float sampleNoiseTexture(float3 position, float densityFalloff = 0)
{
    float value = 0;
    for (int n = 0; n < _NoiseLayerCount; n++)
    {
        float3 offsetCoord = position;
        offsetCoord.y += _Time.x * _ScrollRates[n];
        float noiseValue = tex3Dlod(_AccretionNoiseTex, float4(offsetCoord / _SampleScales[n], 0.0f));
        value += max(0, noiseValue - densityFalloff) * _TransmittancePower;
    }
    return value;
}

// Simple radial color sampling
float3 discColor(float3 position, float falloff, float volRadius, float distFromDisc, float distFromCenter)
{
    float3 radialCoord = cartesianToRadial(position, distFromCenter, distFromDisc);
    float density = sampleNoiseTexture(radialCoord, falloff);

    falloff = pow(remap01(volRadius, 0, distFromCenter), _AccretionFalloff);
    float3 finalCol = lerp(_AccretionMainColor, 1 * _AccretionIntensity, falloff);
    return finalCol * density;
}

// Ray traces a disc shape
float rayTracedDisc(float3 discCenter, float3 rayDir, float3 rayPos)
{
    float3 p1 = discCenter - 0.5 * _AccretionWidth * _AccretionDir;
    float3 p2 = discCenter + 0.5 * _AccretionWidth * _AccretionDir;
    float discDst = intersectDisc(rayPos, rayDir, p1, p2, _AccretionDir, _AccretionOuterRadius, _AccretionInnerRadius);
    return discDst;
}

// Returns the density of a disc of gas at a given sample point
// Samples from layered 3D noise textures for performance
float gasDensity(float3 position, float falloff, float distFromDisc, float distFromCenter)
{
    float3 radialCoord = cartesianToRadial(position, distFromCenter, distFromDisc);
    float density = sampleNoiseTexture(radialCoord, falloff);
    density = lerp(density, _AccretionIntensity, pow(distFromCenter, -_DensityPower));
    return density;
}

// Accumulates the approximate density of gas at a given point
// Result is color graded for an adjustable outcome
void sampleGasVolume(inout float3 color, float3 position, float3 rayDir, float3 volCenter, float volRadius, float stepSize)
{
    float3 finalCol = 0;

    // If accretion disc disabled...
    if (_AccretionQuality == -1){
        return;
    }

    float distFromDisc = dot(_AccretionDir, position - volCenter);
    float distFromCenter = distance(position, volCenter);
    float densityFalloff = lerp(0, _GasCloudThreshold, pow(distFromCenter, 0.98f));

    // Full quality volumetrics
    if (_AccretionQuality == 0)
    {
        float transmittance = gasDensity(position - volCenter, densityFalloff, distFromDisc, distFromCenter);

        // Calculate disc volume shape
        float radialGradient = 1 - saturate((distFromCenter - _AccretionInnerRadius) / _AccretionOuterRadius);
        float slopeFactor = lerp(1, radialGradient, _AccretionSlope);

        // Power curve implementation to adjust near and far attenuation
        float density = pcurve(radialGradient, 4, 1);
        density *= saturate(1 - abs(distFromDisc) / (_AccretionWidth * slopeFactor));

        // If density is very thin or 0...
        if (density < 0.01f) {
            return;
        }

        // Base color
        float normalizedDist = remap01(volRadius, 0, distFromCenter);
        float falloff = pow(normalizedDist, _AccretionFalloff);
        float colorFalloff = pow(normalizedDist, _AccretionColorShift);
        
        float3 baseColor = lerp(_AccretionMainColor, _AccretionInnerColor, colorFalloff);
        finalCol = lerp(baseColor, _AccretionIntensity, falloff) * transmittance * stepSize;
    }
    else if(color.r == 0)
    {
        // Simple ray-traced disc effect
        // More performant as no volumetric sampling involved
        float discDst = rayTracedDisc(volCenter, rayDir, position);
        if (discDst < stepSize)
        {
            // If we detect the disc, sample from the texture
            float3 discSample = (position - volCenter) + rayDir * discDst;
            finalCol = discColor(discSample, densityFalloff, volRadius, distFromDisc, distFromCenter);
        }
    }

    // Modify the color
    color += finalCol;
}

// Gravitational shift source: https://astronomy.swin.edu.au/cosmos/g/Gravitational+Redshift
// As a photon travels toward/away from a mass, it may gain or lose energy causing it to change frequency
// The closer we are to a large mass, photons get blue shifted
float3 computeGravitationalShift(float3 position, float3 center, float gravConst, float mass)
{
    float dist = distance(position, center);
    if (_BlueShiftPower <= 0 || dist < 0.001f){
        return 1;
    }
    dist = pow(exp(dist) / 1.8f, _BlueShiftPower);
    float shift = (gravConst * mass) / (dist * pow(speedOfLight, 2));
    return lerp(1, float3(0, 0, 255), shift);
}
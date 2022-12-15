static const float maxFloat = 3.402823466e+38;
static const float speedOfLight = 299792458;
static const float gravitationalConst = 0.000000000066743;

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
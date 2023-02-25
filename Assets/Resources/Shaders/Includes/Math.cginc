static const float maxFloat = 3.402823466e+38;
static const float speedOfLight = 299792458;
static const float gravitationalConst = 0.000000000066743;
static const float PI = 3.14159265359;

// Returns dstToSphere, dstThroughSphere
// Implementation reference: jeancolasp @ scratchapixel & Sebastian Lague
// https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection.html
// https://www.youtube.com/watch?v=lctXaT9pxA0
float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir)
{
	float3 offset = rayOrigin - sphereCentre;
	float b = dot(offset, rayDir) * 2;
	float c = dot(offset, offset) - pow(sphereRadius, 2);
	float d = pow(b, 2) - 4 * c;

    // Intersected with sphere...
	if (d > 0)
	{
		d = sqrt(d);
		float distToSphere = max(0, (-b - d) / 2);
		float sphereFarDist = (d - b) / 2;
        float dstThroughSphere = sphereFarDist - distToSphere;

		if (sphereFarDist >= 0){
			return float2(distToSphere, dstThroughSphere);
		}
	}

	// No intersections...
	return float2(maxFloat, 0);
}

float remap01(float a, float b, float t) {
	return (t - a) / (b - a);
}

float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
	return minNew + (v - minOld) * (maxNew - minNew) / (maxOld - minOld);
}

// Referenced from https://iquilezles.org/articles/functions/
// Power curve function is a generalized parabola function
// Allows both sides of the curve to be independently manipulated
float pcurve(float x, float a, float b)
{
    float k = pow(a + b, a + b) / (pow(a, a) * pow(b, b));
    return k * pow(x, a) * pow(1.0 - x, b);
}

// Based upon http://www.vias.org/comp_geometry/math_coord_convert_3d.htm
float3 cartesianToRadial(float3 cartesian, float distFromCenter, float distFromDisc)
{
    float3 radialCoord;
    radialCoord.x = distFromCenter * 1.5f + 0.5f;
    radialCoord.y = atan2(-cartesian.x, -cartesian.z) * 1.5f;
    radialCoord.z = distFromDisc * 1.5f;
    return radialCoord;
}

// Disc, Plane & Cylinder intersection logic referenced from Kelvin van Hoorn
// Source: https://kelvinvanhoorn.com/

// Based upon https://mrl.cs.nyu.edu/~dzorin/rend05/lecture2.pdf
float2 intersectInfiniteCylinder(float3 rayOrigin, float3 rayDir, float3 cylinderOrigin, float3 cylinderDir, float cylinderRadius)
{
    float3 a0 = rayDir - dot(rayDir, cylinderDir) * cylinderDir;
    float a = dot(a0, a0);

    float3 dP = rayOrigin - cylinderOrigin;
    float3 c0 = dP - dot(dP, cylinderDir) * cylinderDir;
    float c = dot(c0, c0) - cylinderRadius * cylinderRadius;

    float b = 2 * dot(a0, c0);
    float discriminant = b * b - 4 * a * c;
    if (discriminant > 0) {
        float s = sqrt(discriminant);
        float dstToNear = max(0, (-b - s) / (2 * a));
        float dstToFar = (-b + s) / (2 * a);

        if (dstToFar >= 0) {
            return float2(dstToNear, dstToFar - dstToNear);
        }
    }
    return float2(maxFloat, 0);
}

// Based upon https://mrl.cs.nyu.edu/~dzorin/rend05/lecture2.pdf
float intersectInfinitePlane(float3 rayOrigin, float3 rayDir, float3 planeOrigin, float3 planeDir)
{
    float a = 0;
    float b = dot(rayDir, planeDir);
    float c = dot(rayOrigin, planeDir) - dot(planeDir, planeOrigin);
    float discriminant = b * b - 4 * a * c;
    return -c / b;
}

// Based upon https://mrl.cs.nyu.edu/~dzorin/rend05/lecture2.pdf
float intersectDisc(float3 rayOrigin, float3 rayDir, float3 p1, float3 p2, float3 discDir, float discRadius, float innerRadius)
{
    float discDst = maxFloat;
    float2 cylinderIntersection = intersectInfiniteCylinder(rayOrigin, rayDir, p1, discDir, discRadius);
    float cylinderDst = cylinderIntersection.x;

    if (cylinderDst < maxFloat)
    {
        float finiteC1 = dot(discDir, rayOrigin + rayDir * cylinderDst - p1);
        float finiteC2 = dot(discDir, rayOrigin + rayDir * cylinderDst - p2);

        // Ray intersects with edges of the cylinder/disc
        if (finiteC1 > 0 && finiteC2 < 0 && cylinderDst > 0){
            discDst = cylinderDst;
        }
        else
        {
            float radiusSqr = discRadius * discRadius;
            float innerRadiusSqr = innerRadius * innerRadius;

            float p1Dst = max(intersectInfinitePlane(rayOrigin, rayDir, p1, discDir), 0);
            float3 q1 = rayOrigin + rayDir * p1Dst;
            float p1q1DstSqr = dot(q1 - p1, q1 - p1);

            // Ray intersects with lower plane of cylinder/disc
            if (p1Dst > 0 && p1q1DstSqr < radiusSqr && p1q1DstSqr > innerRadiusSqr)
            {
                if (p1Dst < discDst){
                    discDst = p1Dst;
                }
            }

            float p2Dst = max(intersectInfinitePlane(rayOrigin, rayDir, p2, discDir), 0);
            float3 q2 = rayOrigin + rayDir * p2Dst;
            float p2q2DstSqr = dot(q2 - p2, q2 - p2);

            // Ray intersects with upper plane of cylinder/disc
            if (p2Dst > 0 && p2q2DstSqr < radiusSqr && p2q2DstSqr > innerRadiusSqr)
            {
                if (p2Dst < discDst){
                    discDst = p2Dst;
                }
            }
        }
    }
    return discDst;
}
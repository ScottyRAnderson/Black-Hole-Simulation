static const float maxFloat = 3.402823466e+38;
static const float speedOfLight = 299792458;
static const float gravitationalConst = 0.000000000066743;

// Returns vector (dstToSphere, dstThroughSphere)
// If ray origin is inside sphere, dstToSphere = 0
// If ray misses sphere, dstToSphere = maxValue; dstThroughSphere = 0
float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir)
{
	float3 offset = rayOrigin - sphereCentre;
	float a = 1; // Set to dot(rayDir, rayDir) if rayDir might not be normalized
	float b = 2 * dot(offset, rayDir);
	float c = dot(offset, offset) - sphereRadius * sphereRadius;
	float d = b * b - 4 * a * c; // Discriminant from quadratic formula

	// Number of intersections: 0 when d < 0; 1 when d = 0; 2 when d > 0
	if (d > 0)
	{
		float s = sqrt(d);
		float dstToSphereNear = max(0, (-b - s) / (2 * a));
		float dstToSphereFar = (-b + s) / (2 * a);

		// Ignore intersections that occur behind the ray
		if (dstToSphereFar >= 0){
			return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
		}
	}

	// Ray did not intersect sphere
	return float2(maxFloat, 0);
}

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
        if (finiteC1 > 0 && finiteC2 < 0 && cylinderDst > 0)
        {
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
                if (p1Dst < discDst)
                {
                    discDst = p1Dst;
                }
            }

            float p2Dst = max(intersectInfinitePlane(rayOrigin, rayDir, p2, discDir), 0);
            float3 q2 = rayOrigin + rayDir * p2Dst;
            float p2q2DstSqr = dot(q2 - p2, q2 - p2);

            // Ray intersects with upper plane of cylinder/disc
            if (p2Dst > 0 && p2q2DstSqr < radiusSqr && p2q2DstSqr > innerRadiusSqr)
            {
                if (p2Dst < discDst)
                {
                    discDst = p2Dst;
                }
            }
        }
    }

    return discDst;
}

float intersectSDF(float distA, float distB) {
	return max(distA, distB);
}

float unionSDF(float distA, float distB) {
	return min(distA, distB);
}

float differenceSDF(float distA, float distB) {
	return max(distA, -distB);
}

float remap01(float a, float b, float t) {
	return (t - a) / (b - a);
}

float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
	return minNew + (v - minOld) * (maxNew - minNew) / (maxOld - minOld);
}
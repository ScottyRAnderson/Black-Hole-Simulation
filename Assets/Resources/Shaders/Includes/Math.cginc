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

float intersectSDF(float distA, float distB) {
	return max(distA, distB);
}

float unionSDF(float distA, float distB) {
	return min(distA, distB);
}

float differenceSDF(float distA, float distB) {
	return max(distA, -distB);
}
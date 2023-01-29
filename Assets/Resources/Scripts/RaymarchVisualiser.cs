using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class RaymarchVisualiser : MonoBehaviour
{
    private const float gravitationalConst = 0.000000000066743f;

    private enum DisplayMode
    { 
        Single,
        Multiple
    }

    [SerializeField]
    private DisplayMode displayMode = DisplayMode.Single;
    [SerializeField]
    private Transform rayOrigin;
    [SerializeField]
    private Singularity singularity;

    [Space]

    [SerializeField]
    private float rayMass;
    [SerializeField]
    private float stepSize;
    [SerializeField]
    private int numSteps;
    [SerializeField]
    private bool blockAbsorbedRays;
    [SerializeField]
    private bool debugDirections;
    [SerializeField]
    private float directionDist;

    [Space]

    [SerializeField]
    private float displayWidth = 5f;
    [SerializeField]
    private Color displayColor = Color.white;

    private void OnDrawGizmos()
    {
        if (rayOrigin == null || singularity == null || stepSize <= 0f){
            return;
        }

        Handles.color = displayColor;
        List<Vector3> directions = new List<Vector3>();
        if (displayMode == DisplayMode.Single)
        {
            List<Vector3> positions = SimulateSingle(rayOrigin.position, rayOrigin.forward, stepSize, numSteps);
            directions.Add(rayOrigin.forward);
            for (int i = 0; i < positions.Count; i++)
            {
                if (i < positions.Count - 1){
                    Handles.DrawLine(positions[i], positions[i + 1], displayWidth);
                }
            }
        }
        else
        {
            List<List<Vector3>> positions = SimulateMultiple(rayOrigin.position, rayOrigin.forward, stepSize, numSteps);
            for (int i = 0; i < positions.Count; i++)
            {
                List<Vector3> poss = positions[i];
                directions.Add(poss[0]); // First position is reserved for the direction information
                for (int j = 1; j < poss.Count; j++)
                {
                    if (j < poss.Count - 1){
                        Handles.DrawLine(poss[j], poss[j + 1], displayWidth);
                    }
                }
            }
        }

        if(debugDirections)
        {
            Handles.color = Color.red;
            for (int i = 0; i < directions.Count; i++){
                Handles.DrawLine(rayOrigin.position, rayOrigin.position + directions[i] * directionDist, displayWidth);
            }
        }
    }

    //private List<Vector3> SimulateSingle(Vector3 rayPos, Vector3 rayDir, float stepSize, int numSteps)
    //{
    //    List<Vector3> positions = new List<Vector3>{
    //        rayPos
    //    };
    //
    //    for (int i = 0; i < numSteps; i++)
    //    {
    //        // Distort the ray according to Newton's law of universal gravitation
    //        Vector3 difference = singularity.transform.position - rayPos;
    //        float sqrLength = difference.sqrMagnitude;
    //
    //        Vector3 direction = difference.normalized;
    //        Vector3 acceleration = direction * gravitationalConst * ((singularity.Mass * rayMass) / sqrLength);
    //
    //        // Move the ray according to this force
    //        rayDir += acceleration * stepSize;
    //        rayPos += rayDir;
    //
    //        if(blockAbsorbedRays && Vector3.Distance(rayPos, singularity.transform.position) < singularity.SchwarzschildRadius){
    //            break;
    //        }
    //
    //        positions.Add(rayPos);
    //    }
    //    return positions;
    //}

    private List<Vector3> SimulateSingle(Vector3 rayPos, Vector3 rayDir, float stepSize, int numSteps)
    {
        List<Vector3> positions = new List<Vector3>{
            rayPos
        };

        for (int i = 0; i < numSteps; i++)
        {
            Vector3 dirToCentre = singularity.transform.position - rayPos;
            float dstToCentre = dirToCentre.magnitude;
            dirToCentre /= dstToCentre;

            float force = singularity.SchwarzschildRadius / (dstToCentre * dstToCentre);
            rayDir = Vector3.Normalize(rayDir + dirToCentre * force * stepSize);

            rayPos += rayDir * stepSize;

            if (blockAbsorbedRays && Vector3.Distance(rayPos, singularity.transform.position) < singularity.SchwarzschildRadius){
                break;
            }

            positions.Add(rayPos);
        }
        return positions;
    }

    // Simulates multiple rays at different uv coordinates
    private List<List<Vector3>> SimulateMultiple(Vector3 rayPos, Vector3 forward, float stepSize, int numSteps)
    {
        List<List<Vector3>> positions = new List<List<Vector3>>();

        float step = 0.2f;
        float offset = 0.5f;
        for (float x = 0; x < 1f; x += step)
        {
            for (float y = 0; y < 1f; y += step)
            {
                Vector2 uv = new Vector2(x, y) - new Vector2(offset, offset);
                Vector3 rayDir = transform.localToWorldMatrix * new Vector3(uv.x, uv.y, 0f);
                rayDir.Normalize();

                List<Vector3> singlePositions = SimulateSingle(rayPos, rayDir, stepSize, numSteps);
                singlePositions.Insert(0, rayDir);
                positions.Add(singlePositions);
            }
        }
        return positions;
    }
}
#endif
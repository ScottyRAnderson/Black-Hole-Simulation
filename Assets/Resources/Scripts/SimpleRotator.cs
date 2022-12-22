using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    [SerializeField]
    private Vector3 RotationDirection = Vector3.right;
    [SerializeField]
    private float RotationSpeed = 10f;
    [SerializeField]
    private Space RotationSpace = Space.Self;

    void Update(){
        transform.Rotate(RotationDirection * (Time.deltaTime * RotationSpeed), RotationSpace);
    }
}
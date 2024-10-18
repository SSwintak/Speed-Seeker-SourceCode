using Sirenix.OdinInspector;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public enum RotateDirection
    {   
        Clockwise,
        CounterClockwise
    }

    Transform _transform;

    public bool Rotate;

    public bool Revolve;

    [ShowIf("Revolve")] public GameObject RevolveTarget;

    public Vector3 Axis = new Vector3(0, 0, 1);

    [Range(0.1f, 100f)] public float Speed = 1f;

    public RotateDirection Direction;

    private void Awake()
    {
        _transform = transform;
    }
    private void Update()
    {
        if (Rotate)
        {
            RotateOrbit();
        }
        if (Revolve && RevolveTarget != null)
        {
            RevolveAround(RevolveTarget);
        }
    }

    private void RotateOrbit()
    {
        _transform.Rotate(Axis, Direction == RotateDirection.CounterClockwise ? (5f * Speed) * Time.deltaTime : ((5f * Speed) * Time.deltaTime) * -1f);
    }

    private void RevolveAround(GameObject target)
    {
        _transform.RotateAround(target.transform.position, Axis, Direction == RotateDirection.CounterClockwise ? (5f * Speed) * Time.deltaTime : ((5f * Speed) * Time.deltaTime) * -1f);
    }
}

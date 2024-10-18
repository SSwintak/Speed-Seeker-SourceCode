using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Attractor : MonoBehaviour
{
    [SerializeField] float _AttractorRadius;
    public float AttractorStrength;
    public Transform AttractorPoint;
    public LayerMask AffectedLayers;

    public float AttractorRadius
    { 
        get { return _AttractorRadius; } 
        set 
        { 
            _AttractorRadius = value;
            _CircleCollider.radius = _AttractorRadius;
        }
    }

    CircleCollider2D _CircleCollider;

    private void Awake()
    {
        if (!_CircleCollider && TryGetComponent(out _CircleCollider))
        {
            _CircleCollider.radius = _AttractorRadius;
            _CircleCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!MadWise.Utilities.Utility.IsLayerInMask(collision.gameObject.layer, AffectedLayers)) return;

        AttractToPoint(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!MadWise.Utilities.Utility.IsLayerInMask(collision.gameObject.layer, AffectedLayers)) return;
        AttractToPoint(collision);
    }

    public void AttractToPoint(Collider2D other)
    {
        if (other.attachedRigidbody)
        {
            Vector2 dir = (AttractorPoint.position - other.transform.position).normalized;
            other.attachedRigidbody.velocity = dir * AttractorStrength;
        }
    }
}

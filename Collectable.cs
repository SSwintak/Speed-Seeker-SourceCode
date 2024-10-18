using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour, ISpawnableObject
{
    [ReadOnly, SerializeField] bool _HasBeenScattered = false;

    [SerializeField] bool _ResetAfterTime = false;

    [Tooltip("How long (in seconds) until the object resets into the object pool.")]
    [SerializeField, ShowIf("_ResetAfterTime", true)] float _ResetTime = 15f;


    [SerializeField] float _MinSpeed = 50f, _MaxSpeed = 250f;

    public bool hasBeenScattered => _HasBeenScattered;

    Rigidbody2D _RigidBody;

    public enum CollectablesType
    {
        Health,
        SpeedBoost,
        Fuel,
        Cash,
        Shield,
        Magnet
    }

    [SerializeField] CollectablesType _CollectableType;
    public CollectablesType CollectableType => _CollectableType;

    [BoxGroup("Add")]
    [SerializeField, ShowIf("_CollectableType", CollectablesType.Health)] int _HealthIncrease;
    public int HealthIncrease => _HealthIncrease;

    [BoxGroup("Multiplier")]
    [SerializeField, ShowIf("_CollectableType", CollectablesType.SpeedBoost)] float _SpeedMultiplier, _SpeedIncreaseDuration;
    public float SpeedMultiplier => _SpeedMultiplier;
    public float SpeedIncreaseDuration => _SpeedIncreaseDuration;

    [BoxGroup("Add%"), Tooltip("Will be used as a percentage based on the total amount of fuel the ship has.")]
    [SerializeField, ShowIf("_CollectableType", CollectablesType.Fuel), MinValue(1f), MaxValue(100f)] float _FuelIncrease;
    public float FuelIncrease => _FuelIncrease;

    [BoxGroup("Add")]
    [SerializeField, ShowIf("_CollectableType", CollectablesType.Cash)] int _CashIncrease;
    public int CashIncrease => _CashIncrease;

    [BoxGroup("Add")]
    [SerializeField, ShowIf("_CollectableType", CollectablesType.Shield)] float _ShieldDuration;
    public float ShieldDuration => _ShieldDuration;

    [BoxGroup("Add")]
    [SerializeField, ShowIf("_CollectableType", CollectablesType.Magnet)] float _MagnetRadius, _MagnetDuration, _MagnetStrength;
    public float MagnetRadius => _MagnetRadius;
    public float MagnetDuration => _MagnetDuration;
    public float MagnetStrength => _MagnetStrength;

    Coroutine _ReturnToPoolRoutine;

    bool _IsInPool = false;

    private void OnBecameInvisible()
    {        
        ReturnToPool();
    }

    void Awake()
    {
        Init();
    }

    void OnEnable()
    {        
        if (_ResetAfterTime) _ReturnToPoolRoutine = StartCoroutine(ReturnToPoolAfterTime());
        Spawn(_RigidBody, _MinSpeed, _MaxSpeed);
    }

    public void Init()
    {        
        _RigidBody = GetComponent<Rigidbody2D>();
    }

    public void Spawn(Rigidbody2D _RigidBody, float _MinSpeed, float _MaxSpeed)
    {
        _IsInPool = false;

        if (_RigidBody.IsSleeping())
        {
            _RigidBody.WakeUp();// all collectables (should) start asleep 
            _RigidBody.AddForce(new Vector2(0f, -Random.Range(_MinSpeed, _MaxSpeed)));
            _RigidBody.SetRotation(Random.Range(0f, 359f));
        }
        // only apply force to objects that have been pulled from the pool, when they have the active will be true if not it's false/hidden
        else if (_RigidBody.IsAwake() && _RigidBody.gameObject.activeSelf)
        {
            _RigidBody.AddForce(new Vector2(0f, -Random.Range(_MinSpeed, _MaxSpeed)));
            _RigidBody.SetRotation(Random.Range(0f, 359f));
        }
    }

    public void ReturnToPool()
    {
        if (_IsInPool) return;

        _IsInPool = true;
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public IEnumerator ReturnToPoolAfterTime()
    {        
        float elapsedTime = 0f;
        while (elapsedTime < _ResetTime)
        {
            elapsedTime += 1f;
            yield return new WaitForSecondsRealtime(1f);
        }
        ReturnToPool();
    }
}

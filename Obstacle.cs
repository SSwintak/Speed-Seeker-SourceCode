using MadWise.Utilities;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class Obstacle : MonoBehaviour, ISpawnableObject
{
    [ReadOnly, SerializeField] bool _HasBeenScattered = false;

    [Tooltip("How long (in seconds) until the object resets into the object pool.")]
    [SerializeField] float _ResetTime = 15f;

    [SerializeField] float _MinSpeed = 50f, _MaxSpeed = 250f;
    [SerializeField] int _ObstacleDamage;
    public int ObstacleDamage => _ObstacleDamage;

    public bool HasBeenScattered => _HasBeenScattered;
    
    Rigidbody2D _RigidBody;

    public Vector2 LastVelocity { get; private set; }
    public float LastAngularVelocity { get; private set; }
    public int NumberOfImpactsOnPlayer { get; private set; }

    Coroutine _ReturnToPoolRoutine;

    float _DefaultMinSpeed;
    float _DefaultMaxSpeed;
    int _DefaultDamage;

    // prevents the ReturnToPool() from being called twice, which caused some issues with spawning
    bool _IsInPool = false;

    public delegate void DelegateObstacleDestroyed(GameObject obj);
    public event DelegateObstacleDestroyed OnObstacleDestroyed;

    private void OnBecameInvisible()
    {
        ReturnToPool();
    }

    private void OnDestroy()
    {
        OnObstacleDestroyed?.Invoke(gameObject);
    }

    void Awake()
    {
        Init();       

        // check if the difficulty changed, also helps with an obstacle is created after the change
        ApplyDifficultyMultiplier(GameManager.Instance.GameDifficulty.difficultyLevel);
    }

    public void Init()
    {
        _DefaultMinSpeed = _MinSpeed;
        _DefaultMaxSpeed = _MaxSpeed;
        _DefaultDamage = _ObstacleDamage;
        _RigidBody = GetComponent<Rigidbody2D>();
        Spawn(_RigidBody, _MinSpeed, _MaxSpeed);
    }

    void OnEnable()
    {
        GameManager.onDifficultyChange += ApplyDifficultyMultiplier;
        if (_ReturnToPoolRoutine != null) StopCoroutine(_ReturnToPoolRoutine); // stop the current routine, then restart it
        _ReturnToPoolRoutine = StartCoroutine(ReturnToPoolAfterTime());
        Spawn(_RigidBody, _MinSpeed, _MaxSpeed);        
    }

    private void OnDisable()
    {
        GameManager.onDifficultyChange -= ApplyDifficultyMultiplier;
    }

    void FixedUpdate()
    {
        LastVelocity = _RigidBody.velocity;
        LastAngularVelocity = _RigidBody.angularVelocity;
    }

    public void Spawn(Rigidbody2D rigidBody, float minSpeed, float maxSpeed)
    {
        _IsInPool = false;
        if (rigidBody.IsSleeping())
        {
            rigidBody.WakeUp();// all obstacles start asleep 
            rigidBody.AddForce(new Vector2(0f, -Random.Range(minSpeed, maxSpeed)));
            rigidBody.SetRotation(Random.Range(0f, 359f));
        }
        else if (rigidBody.IsAwake() && rigidBody.gameObject.activeSelf)
        {
            rigidBody.AddForce(new Vector2(0f, -Random.Range(minSpeed, maxSpeed)));
            rigidBody.SetRotation(Random.Range(0f, 359f));
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

    /// <summary>
    /// Will multiply the stats by the multiplier given by the difficulty level
    /// </summary>
    void ApplyDifficultyMultiplier(DifficultyLevel difficultyLevel)
    {
        float multiplier = Difficulty.GetMultiplierFromLevel(difficultyLevel);
        ApplyMultiplierToStats(multiplier);
    }

    /// <summary>
    /// Will multiply the stats by the multiplier given
    /// </summary>
    void ApplyMultiplierToStats(float multiplier)
    {
        multiplier = Mathf.Clamp(multiplier, Difficulty.MIN_DIFFICULTY_MULTIPLIER, Difficulty.MAX_DIFFICULTY_MULTIPLIER);

        // update attributes
        _MinSpeed = _DefaultMinSpeed * multiplier;
        _MaxSpeed = _DefaultMaxSpeed * multiplier;
        
        _ObstacleDamage = Mathf.RoundToInt(_DefaultDamage * multiplier);
    }
    
    public void IncreaseImpactsOnPlayer(int amount = 1)
    {
        NumberOfImpactsOnPlayer += amount;
    }

    // editor script buttons
#if UNITY_EDITOR

    [Button("Print IsSleeping")]
    void Button_IsSleeping()
    {
        Debug.Log(name + " IsSleeping: " + _RigidBody.IsSleeping());
    }

    [Button("Print Obstacle Velocity")]
    void Button_ObstacleVelocity()
    {
        Debug.Log(name + " Obstacle Velocity: " + _RigidBody.velocity);
    }
    
#endif
}

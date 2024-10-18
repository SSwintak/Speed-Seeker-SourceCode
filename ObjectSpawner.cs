using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableObject
    { 
        public GameObject Obj;
        public float SpawnRate;
        public DifficultyLevel SpawnOnDifficulty;
    }

    /// <summary>
    /// Minimum spawn rate in seconds
    /// </summary>
    public float MIN_SPAWN_RATE => 0.1f;

    /// <summary>
    /// Maximum spawn rate in seconds
    /// </summary>
    public float MAX_SPAWN_RATE => 86400f;// 24 hours


    [SerializeField] Camera playerCamera;      

    [Tooltip("If enabled, obstacles will start from random positions.")]
    [SerializeField] bool Scatter = true;

    [Tooltip("If true any collectable type objects while not be affected by the difficulty modifier. Only applies to 'Special Objects'")]
    [SerializeField] bool _IgnoreCollectablesForDifficulty = true;

    [Tooltip("Only for normal obstacles, spawn rate in seconds.")]
    [SerializeField, ShowIf("NormalObjects")] float SpawnRate = 0.3f;
    [SerializeField] List<GameObject> NormalObjects = new();

    [DictionaryDrawerSettings(KeyLabel = "Object To Spawn", ValueLabel = "Spawn Rate")]
    [SerializeField] SerializedDictionary<string, SpawnableObject> SpecialObjects = new();

    [SerializeField] Transform _SpecialObjectsParent;

    Coroutine _NormalObjectsSpawnRoutine;
    Dictionary<string, Coroutine> _SpecialObjectsSpawnRoutines;

    void Start()
    {
        GameManager.onDifficultyChange += ApplyDifficultyMultiplier;
        _SpecialObjectsSpawnRoutines = new (SpecialObjects.Count);

        StartSpawningObjects();
        StartSpawningSpecialObjects();
    }

    private void StartSpawningObjects()
    {
        _NormalObjectsSpawnRoutine = StartCoroutine(SpawnObjectsRoutine(SpawnRate));
    }

    private void StartSpawningSpecialObjects()
    {
        int i = 0;
        foreach (var sObject in SpecialObjects)
        {
            if ((int)GameManager.Instance.GameDifficulty.difficultyLevel < (int)sObject.Value.SpawnOnDifficulty) continue; // if it's not equal to or higher difficulty level then skip

            if (GameManager.Instance.AdventureMode 
            && (sObject.Key.Contains("Fuel", System.StringComparison.OrdinalIgnoreCase) 
            ||  sObject.Key.Contains("Coin", System.StringComparison.OrdinalIgnoreCase))) // skip fuel and coin spawning if it's in adventure mode
            {
                i++;
                continue;
            }
            if (!_SpecialObjectsSpawnRoutines.ContainsKey(sObject.Value.Obj.name))
            {
                _SpecialObjectsSpawnRoutines.Add(sObject.Value.Obj.name, StartCoroutine(SpawnSpecialObjectsRoutine(sObject.Value.Obj, sObject.Value.SpawnRate)));
            }
            i++;
        }
    }

    void OnDestroy()
    {
        GameManager.onDifficultyChange -= ApplyDifficultyMultiplier;
        StopAllCoroutines();
    }

    void ApplyDifficultyMultiplier(DifficultyLevel level)
    {
        float multiplier = Difficulty.GetMultiplierFromLevel(level);
        ApplyMultiplierToStats(multiplier);
    }

    void ApplyMultiplierToStats(float multiplier, bool ignoreCollectables = true, bool increaseNormalObstacleSpawnRate = false)
    {
        // Create a list of values to iterate over
        List<SpawnableObject> spawnableObjects = new List<SpawnableObject>(SpecialObjects.Values);

        // stop coroutines to change the spawn rate
        StopSpawningNormalObjects();
        foreach (var sObject in spawnableObjects)
        {
            if (ignoreCollectables && sObject.Obj.TryGetComponent(out Collectable c)) continue; // ignore the collectables spawn routines
            else StopSpawningSpecialObject(sObject.Obj.name);
        }

        // NORMAL OBJECTS
        float newRate = SpawnRate / (multiplier * 0.65f) + (increaseNormalObstacleSpawnRate ? 0 : 1);
        SpawnRate = Mathf.Clamp(newRate, MIN_SPAWN_RATE, MAX_SPAWN_RATE);

        // SPECIAL OBJECTS

        // Iterate over the values and modify the dictionary
        foreach (var obj in spawnableObjects)
        {
            // ignore any collectables
            if (ignoreCollectables && obj.Obj.TryGetComponent(out Collectable c)) continue;

            // The Value in the dictionary is the spawn rate for special obstacles
            newRate = obj.SpawnRate / multiplier;
            obj.SpawnRate = Mathf.RoundToInt(Mathf.Clamp(newRate, MIN_SPAWN_RATE, MAX_SPAWN_RATE));
        }

        // start the coroutines again
        StartSpawningObjects();
        StartSpawningSpecialObjects();
    }

    /// <summary>
    /// Randomizes the position within the screen space for the obstacle
    /// </summary>
    public void ScatterObstacle(Camera camera, GameObject obstacle)
    {
        if (obstacle == null) return;

        float randomX = Random.Range(0f, Screen.width);
        Vector3 scatteredPosition = camera.ScreenToWorldPoint(new Vector3(randomX, 0, 0));
        obstacle.transform.position = new Vector3(scatteredPosition.x, obstacle.transform.position.y, obstacle.transform.position.z);
    }

    public void SpawnObstacle(bool _Scatter)
    {
        foreach (var obstacle in NormalObjects)
        {
            var spawnedObject = ObjectPoolManager.SpawnObject(obstacle, transform.position, Quaternion.identity);
            spawnedObject.transform.parent = transform;
            if (_Scatter) ScatterObstacle(playerCamera, spawnedObject);
        }        
    }

    public void SpawnSpecialObstacle(GameObject obj, bool _Scatter)
    {
        Vector3 pos = transform.position;
        var spawnedObject = ObjectPoolManager.SpawnObject(obj, new Vector3(pos.x, pos.y + 10f, pos.z), Quaternion.identity);
        if (spawnedObject.TryGetComponent(out Collectable collectable)) spawnedObject.transform.parent = _SpecialObjectsParent;
        else spawnedObject.transform.parent = transform;

        if (_Scatter) ScatterObstacle(playerCamera, spawnedObject);
    }

    public void StopSpawningAllObjects()
    {
        if (_NormalObjectsSpawnRoutine != null) StopCoroutine(_NormalObjectsSpawnRoutine);

        foreach (var routine in _SpecialObjectsSpawnRoutines)
        {
            if (routine.Value != null)
            {
                StopCoroutine(routine.Value);
            }
        }

        _SpecialObjectsSpawnRoutines.Clear();
    }

    public void StopSpawningSpecialObject(string name)
    {
        _SpecialObjectsSpawnRoutines.TryGetValue(name, out var routine);
        if (routine != null)
        {
            StopCoroutine(routine);
            _SpecialObjectsSpawnRoutines.Remove(name);
        }
    }

    public void StopSpawningNormalObjects()
    {
        if (_NormalObjectsSpawnRoutine != null) StopCoroutine(_NormalObjectsSpawnRoutine);
    }

    private IEnumerator SpawnObjectsRoutine(float spawnRate)
    {
        while (true)
        {
            SpawnObstacle(Scatter);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private IEnumerator SpawnSpecialObjectsRoutine(GameObject obj, float spawnRate)
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnRate);
            SpawnSpecialObstacle(obj, Scatter);
        }
    }
}

using MadWise.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 position, Quaternion rotation)
    {
        if (!ObjectPools.BurstFind(p => p.ObjectName == objectToSpawn.name, out PooledObjectInfo pool)) // check if the pool exists
        {
            pool = new PooledObjectInfo() { ObjectName = objectToSpawn.name };
            ObjectPools.Add(pool);
        }
        
        GameObject spawnableObject = pool.InactiveObjects.FirstOrDefault();
        if (spawnableObject == null) // no inactive objects? create it dummy
        {
            //pool.InactiveObjects.RemoveAll(obj => obj == null); // can use this to make sure all null objects are removed, you need to reset the ObjectPools list when reloading a scene or it will have null entries
            spawnableObject = Instantiate(objectToSpawn, position, rotation);
        }
        else // ah there is an inactive object, reactivate it
        {
            spawnableObject.transform.SetPositionAndRotation(position, rotation);
            pool.InactiveObjects.Remove(spawnableObject);
            spawnableObject.SetActive(true);
        }
        
        if (spawnableObject.TryGetComponent(out Obstacle obstacle))
        {
            obstacle.OnObstacleDestroyed += RemoveObjectFromPool;
        }

        return spawnableObject;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string newName = obj.name.Substring(0, obj.name.Length - 7);// remove '(Clone)'

        if (ObjectPools.BurstFind(p => p.ObjectName == newName, out PooledObjectInfo pool))
        {
            obj.SetActive(false);
            if (!pool.InactiveObjects.Contains(obj)) pool.InactiveObjects.Add(obj);
        }
        else
        {
            Debug.LogWarning("Trying to reference a object that is not pooled: " + obj.name);
        }
    }

    static void RemoveObjectFromPool(GameObject obj)
    {
        if (ObjectPools.BurstFind(p => p.ObjectName == obj.name, out PooledObjectInfo pool) && pool.InactiveObjects.Contains(obj))
        {
            pool.InactiveObjects.Remove(obj);
        }
    }
}

public class PooledObjectInfo // TODO add pool name identifier
{
    public string ObjectName;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}

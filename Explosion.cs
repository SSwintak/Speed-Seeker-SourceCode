using UnityEngine;

public class Explosion : MonoBehaviour
{
    public static void CreateExplosion(GameObject explosionPrefab, Transform _parent)
    {
        var explosion = Instantiate(explosionPrefab, _parent);
        explosion.transform.localPosition = Vector3.zero;
        CheckForComponent(explosion);
    }

    

    public static void CreateExplosion(GameObject explosionPrefab, Transform _parent, Quaternion _rotation)
    {
        var explosion = Instantiate(explosionPrefab, _parent);
        explosion.transform.localPosition = Vector3.zero;
        explosion.transform.rotation = _rotation;
        CheckForComponent(explosion);
    }

    public void DestroyExplosion()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Will add the explosion component if the given object does NOT have it.
    /// </summary>
    private static void CheckForComponent(GameObject explosion)
    {
        if (!explosion.TryGetComponent<Explosion>(out Explosion comp))
        {
            explosion.AddComponent<Explosion>();
        }
    }
}

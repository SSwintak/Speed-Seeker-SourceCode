
using System.Collections;
using UnityEngine;

public interface ISpawnableObject
{
    void Spawn(Rigidbody2D rigidBody, float minSpeed, float maxSpeed);

    IEnumerator ReturnToPoolAfterTime();
}

public static class SpawnableUtility
{
    public static void Spawn(Rigidbody2D rigidBody, float minSpeed, float maxSpeed)
    {
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
}
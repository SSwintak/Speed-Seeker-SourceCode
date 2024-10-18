//using Infohazard.Core;
//using SickDev.CommandSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Collections;
using UnityEngine.UI;
using System.Text;

public enum AnchorPreset
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    StretchVerticalTop,
    StretchVerticalCenter,
    StretchVerticalBottom,
    StretchHorizontalTop,
    StretchHorizontalCenter,
    StretchHorizontalBottom,
    StretchAll
}

namespace MadWise.Utilities
{
    public static class Utility
    {
        public static bool IsNaN(Vector3 vec)
        {
            return double.IsNaN(vec.x) || double.IsNaN(vec.y) || double.IsNaN(vec.z);
        }

        public static bool Approximately(Vector3 vec1, Vector3 vec2)
        {
            return Mathf.Approximately(vec1.x, vec2.x) && Mathf.Approximately(vec1.y, vec2.y) && Mathf.Approximately(vec1.z, vec2.z);
        }

        /// <summary>
        /// Checks to see if the position is under the provided surface.
        /// </summary>
        public static bool IsUnderSurface(Vector3 position, Vector3 Surface)
        {
            bool result = position.y < Surface.y;
            return result;
        }

        /// <summary>
        /// Checks to see if the position is under the provided surface.
        /// </summary>
        public static bool IsUnderSurface(Vector3 position, float Surface)
        {
            bool result = position.y < Surface;
            return result;
        }

        //public static GameObject GetChildObjectWithTag(Transform parent, string tag)
        //{
        //    if (tag.CompareTo(GameTag.Tags) == 0) return null;
            
        //    Transform child;
        //    for (int i = 0; i < parent.childCount; i++)
        //    {
        //        child = parent.GetChild(i);
        //        if (child.CompareTag(tag)) return child.gameObject;
        //    }
        //    return null; // return null if no child found with that tag
        //}

        public static bool IsLayerInMask(int layer, LayerMask layerMask)
        {
            bool result = layerMask == (layerMask | 1 << layer); // this checks if the layer is included in the mask
            //Debug.Log($"Layer ({LayerMask.LayerToName(layer)}) in Mask: {result}");
            return result;
        }

        public static bool IsLayersInMask(int[] layers, LayerMask layerMask)
        {
            List<int> layersNotInMask = new();
            for (int i = 0; i < layers.Length; i++)
            {
                if ( !( layerMask == (layerMask | 1 << layers[i]) ) // if the layer isn't in the mask
                &&   !layersNotInMask.Contains(i) )
                {
                    layersNotInMask.Add(i);
                }
            }
            return layersNotInMask.Count == 0;
        }
        
        public static void SetAnchorPreset(AnchorPreset preset, RectTransform rt)
        {
            switch (preset)
            {
                // NON STRETCH PRESETS
                case AnchorPreset.TopLeft:
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(0, 1);
                    rt.pivot = new Vector2(0, 1);
                    break;
                case AnchorPreset.TopCenter:
                    rt.anchorMin = new Vector2(0.5f, 1);
                    rt.anchorMax = new Vector2(0.5f, 1);
                    rt.pivot = new Vector2(0.5f, 1);
                    break;
                case AnchorPreset.TopRight:
                    rt.anchorMin = new Vector2(1, 1);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.pivot = new Vector2(1, 1);
                    break;
                case AnchorPreset.BottomLeft:
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(0, 0);
                    rt.pivot = new Vector2(0, 0);
                    break;
                case AnchorPreset.BottomCenter:
                    rt.anchorMin = new Vector2(0.5f, 0);
                    rt.anchorMax = new Vector2(0.5f, 0);
                    rt.pivot = new Vector2(0.5f, 0);
                    break;
                case AnchorPreset.BottomRight:
                    rt.anchorMin = new Vector2(1, 0);
                    rt.anchorMax = new Vector2(1, 0);
                    rt.pivot = new Vector2(1, 0);
                    break;

                // STRETCH PRESETS
                case AnchorPreset.StretchVerticalTop:
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.pivot = new Vector2(0.5f, 1);
                    break;
                case AnchorPreset.StretchVerticalCenter:
                    rt.anchorMin = new Vector2(0, 0.5f);
                    rt.anchorMax = new Vector2(1, 0.5f);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.StretchVerticalBottom:
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(1, 0);
                    rt.pivot = new Vector2(0.5f, 0);
                    break;
                case AnchorPreset.StretchHorizontalTop:
                    rt.anchorMin = new Vector2(0, 1);
                    rt.anchorMax = new Vector2(0, 0);
                    rt.pivot = new Vector2(0, 0.5f);
                    break;
                case AnchorPreset.StretchHorizontalCenter:
                    rt.anchorMin = new Vector2(0.5f, 1);
                    rt.anchorMax = new Vector2(0.5f, 0);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.StretchHorizontalBottom:
                    rt.anchorMin = new Vector2(1, 1);
                    rt.anchorMax = new Vector2(1, 0);
                    rt.pivot = new Vector2(1, 0.5f);
                    break;
                case AnchorPreset.StretchAll:
                    rt.anchorMin = new Vector2(0, 0);
                    rt.anchorMax = new Vector2(1, 1);
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    break;
                default:
                    Debug.LogError("Unsupported AnchorPreset value.");
                    break;
            }
        }

        public static Scene[] GetLoadedScenes(bool includeSubScene)
        {
            Scene[] loadedScenes = new Scene[SceneManager.loadedSceneCount];
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.isLoaded || (includeSubScene && scene.isSubScene)) loadedScenes[i] = scene;
            }

            return loadedScenes;
        }

        public static float CalculateFPS()
        {
            float frameRate = Mathf.Round(1f / Time.deltaTime);
            return frameRate;
        }

        #region Type Extensions

        public static void SetAlpha(this Image image, float a)
        {
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, a);
        }

        public static void TrimZeroWidthSpace(this string text)
        {
            text = text.Replace("\u200B", ""); // replace zero width space with null char
            text = text.Trim();                // trim null char: the ASCII code for that is 0
        }

        /// <summary>
        /// Will only add the command if has not already been added
        /// </summary>
        //public static void AddNonExistingCommand(this SickDev.DevConsole.DevConsole singleton, Command command)
        //{
        //    if (!singleton.IsCommandAdded(command)) singleton.AddCommand(command);
        //}

        /// <summary>
        /// Will only add the command if has not already been added
        /// </summary>
        //public static void AddNonExistingCommands(this SickDev.DevConsole.DevConsole singleton, Command[] commands)
        //{
        //    foreach (Command command in commands)
        //    {
        //        if (!singleton.IsCommandAdded(command)) singleton.AddCommand(command);
        //    }
        //}

        public static float ToMilliseconds(this float num)
        {
            return num * 1000f;
        }

        public static bool IsA<T>(this Transform transform)
        {
            bool result = transform.TryGetComponent<T>(out var t);
            return result;
        }

        public static bool RemoveComponent<T>(this GameObject obj) where T : Component
        {
            bool result = obj.TryGetComponent<T>(out var t);
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && t) Component.DestroyImmediate(t); // destroy immediate if in edit mode
            else if (t) Component.Destroy(t);
#else
            if (t) Component.Destroy(t);
#endif
            return result;
        }

        public static bool RemoveComponent<T>(this GameObject obj, T comp) where T : Component
        {
            bool result = comp != null;
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying && comp) Component.DestroyImmediate(comp); // destroy immediate if in edit mode
            else if (comp) Component.Destroy(comp);
#else
            if (comp) Component.Destroy(comp);
#endif
            return result;
        }

        public static bool RemoveComponent<T>(this Transform trans) where T : Component
        {
            bool result = trans.TryGetComponent<T>(out var t);
            if (t) Component.Destroy(t);

            return result;
        }

        public static bool IsColliderInFrustum(this Camera camera, Collider collider)
        {
            if (!collider) throw new System.ArgumentNullException("collider");

            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            bool result = GeometryUtility.TestPlanesAABB(frustumPlanes, collider.bounds);
            return result;
        }

        /// <summary>
        /// Get the bounds of a cameras frustum.
        /// </summary>
        /// <returns>The frustum of the camera as bounds.</returns>
        public static Bounds BoundsFromFrustum(this Camera camera, float distance, Rect viewport = new Rect())
        {
            Vector3[] frustumCorners = new Vector3[4];

            if (viewport == Rect.zero) viewport = new Rect(0, 0, Screen.width, Screen.height);
            camera.CalculateFrustumCorners(viewport, distance, Camera.MonoOrStereoscopicEye.Mono, frustumCorners);

            for (int i = 0; i < frustumCorners.Length; i++)// convert to world space, CalculateFrustrumCorners gives you local coords by default
            {
                frustumCorners[i] = camera.transform.TransformPoint(frustumCorners[i]);
            }

            // Calculate the center and size of the bounds
            Vector3 center = (frustumCorners[0] + frustumCorners[1] + frustumCorners[2] + frustumCorners[3]) / 4.0f;
            Bounds bounds = new Bounds(center, Vector3.zero);

            for (int i = 0; i < frustumCorners.Length; i++)
            {
                bounds.Encapsulate(frustumCorners[i]); // ensure all corners are within the bounds
            }

            return bounds;
        }

        public static LayerMask Combine(this LayerMask layerMask1, LayerMask layerMask2)
        {
            return layerMask1 | layerMask2;
        }

        public static LayerMask Combine(this LayerMask layerMask, LayerMask[] maskArray)
        {
            int combinedLayerMask = 0;

            // Combine each layer mask using bitwise OR
            foreach (LayerMask mask in maskArray)
            {
                combinedLayerMask |= mask;
            }

            return combinedLayerMask;
        }

        public static Vector3 Clamp(this Vector3 value, float min, float max)
        {
            Vector3 clampedVec = new Vector3(Mathf.Clamp(value.x, min, max), Mathf.Clamp(value.y, min, max), Mathf.Clamp(value.z, min, max));
            return clampedVec;
        }

        public static Vector3 GetWorldPosition(this Camera value)
        {
            return value.transform.position;
        }

        public static bool CompareLayer(this Component value, int layer)
        {
            return value.gameObject.layer == layer;
        }

        #endregion
    }

    public abstract class Timer
    {
        protected float initialTime;
        public float Time { get; set; }
        public bool IsRunning { get; protected set; }

        public float Progress => Time / initialTime;

        public System.Action OnTimerStart = delegate { };
        public System.Action OnTimerStop = delegate { };

        protected Timer(float value)
        {
            initialTime = value;
            IsRunning = false;
        }

        public void Start()
        {
            Time = initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                OnTimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                OnTimerStop.Invoke();
            }
        }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public abstract void Tick(float deltaTime);
    }

    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= (float)deltaTime;
            }
            
            if (IsRunning && Time <= 0)
            {
                Stop();
            }
        }

        public bool IsFinished => Time <= 0;

        public void Reset() => Time = initialTime;

        public void Reset(float newTime)
        {
            initialTime = newTime;
            Reset();
        }
    }

    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) { }

        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += deltaTime;
            }
        }

        public void Reset() => Time = 0;

        public float GetTime() => Time;
    }

    public static class RandomGenerator
    {
        /// <summary>
        /// Is deterministic so to generate random numbers reseed the generator. If you don't, eventually, it won't seem to be random.
        /// </summary>
        [BurstCompile]
        public struct BurstRandom
        {
            Unity.Mathematics.Random _random;
            

            public BurstRandom(uint seed)
            {
                _random = new(seed);
            }

            public float RandomRange(float min, float max)
            {
                return _random.NextFloat(min, max);
            }

            public float2 RandomRange(float2 min, float2 max)
            {
                return _random.NextFloat2(max, min);
            }

            public float3 RandomRange(float3 min, float3 max)
            {
                return _random.NextFloat3(max, min);
            }

            public int RandomRange(int min, int max)
            {
                return _random.NextInt(min, max);
            }

            public int2 RandomRange(int2 min, int2 max)
            {
                return _random.NextInt2(min, max);
            }

            public int3 RandomRange(int3 min, int3 max)
            {
                return _random.NextInt3(min, max);
            }

            public float RandomFloat01()
            {
                return _random.NextFloat();
            }

            public int RandomInt01()
            {
                return _random.NextInt();
            }
        }

    }

    [BurstCompile]
    public static class BurstUtility
    {
        [BurstCompile]
        public struct BurstDistance: IJob
        {
            public float3 pointA;
            public float3 pointB;
            public NativeArray<float> result;

            public void Execute()
            {
                result[0] = math.distance(pointA, pointB);
            }
        }

        [BurstCompile]
        public static bool BurstFind<T>(this List<T> list, System.Predicate<T> predicate, out T result)
        {
            result = default;

            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    result = list[i];
                    return true;
                }
            }

            return false;
        }
    }

    public static class UtilityStringBuilder
    { 
        static StringBuilder builder = new();
        public static string CurrentValue => builder.ToString();

        /// <summary>
        /// Use this to create the builder, will only execute if the builder is null
        /// </summary>
        public static void CreateBuilder()
        {
            if (builder == null) builder = new();
        }

        public static void Clear()
        {
            builder.Clear();
        }

        /// <summary>
        /// Will clear the builder then appends the value to it.
        /// </summary>
        /// <returns>The string value of the builder after it appends the value</returns>
        public static string Create(string value)
        {
            builder.Clear();
            builder.Append(value);

            return builder.ToString();
        }

        /// <summary>
        /// Will clear the builder then appends the value to it.
        /// </summary>
        /// <returns>The string value of the builder after it appends the value</returns>
        public static string Create(float value)
        {
            builder.Clear();
            builder.Append(value);

            return builder.ToString();
        }

        /// <summary>
        /// Will clear the builder then appends the value to it.
        /// </summary>
        /// <returns>The string value of the builder after it appends the value</returns>
        public static string Create(string format, string value)
        {
            builder.Clear();
            builder.AppendFormat(format, value);

            return builder.ToString();
        }

        /// <summary>
        /// Will clear the builder then appends the value to it.
        /// </summary>
        /// <returns>The string value of the builder after it appends the value</returns>
        public static string Create(string format, string value1, string value2)
        {
            builder.Clear();
            builder.AppendFormat(format, value1, value2);

            return builder.ToString();
        }

        /// <summary>
        /// Will clear the builder then appends the value to it.
        /// </summary>
        /// <returns>The string value of the builder after it appends the value</returns>
        public static string Create(string format, float value)
        {
            builder.Clear();
            builder.AppendFormat(value % 1 != 0 ? format : "{0}", value); // if remainder means it's a float so use the provided format incase they want a decimal format

            return builder.ToString();
        }
    }
}
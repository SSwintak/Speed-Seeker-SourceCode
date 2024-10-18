using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


    public static class PhysicsLayer {
        public const int Default = 0;
        public const int TransparentFX = 1;
        public const int IgnoreRaycast = 2;
        public const int Water = 3;
        public const int UI = 4;
        public const int NormalObstacle = 5;
        public const int SpecialObstacle = 6;
        public const int Collectable = 7;


        public static readonly int[] Layers = {
            Default, TransparentFX, IgnoreRaycast, Water, UI, NormalObstacle, SpecialObstacle, Collectable, 
        };

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {
            Layer.PhysicsOverrideLayers = Layers;
        }
    }


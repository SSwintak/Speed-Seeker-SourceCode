
public static class Layer
{
    public const int Default = 0;
    public const int TransparentFX = 1;
    public const int IgnoreRaycast = 2;
    public const int Water = 4;
    public const int UI = 5;

    /// <summary>
    /// Array of default layers provided by Unity.
    /// </summary>
    public static readonly int[] DefaultLayers = 
    {
        Default, TransparentFX, IgnoreRaycast, Water, UI
    };

    /// <summary>
    /// Set by the generated <c>PhysicsLayer</c> script.
    /// </summary>
    public static int[] PhysicsOverrideLayers = null;

    /// <summary>
    /// Array of all default and custom tags in the project. 
    /// </summary>
    public static int[] Layers => PhysicsOverrideLayers ?? DefaultLayers;
}

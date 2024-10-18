
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
//using Infohazard.Core.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[InitializeOnLoad]
public static class PhysicsLayerGenerator
{
    private const string CheckLayersPref = "CheckLayers";

    private const string Newline = @"
";
    private const string LayerTemplate = "        public const int {0} = {1};" + Newline;

    private const string TagArrayTemplate = "{0}, ";

    private const string Template = @"using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


    public static class PhysicsLayer {{
{0}
{1}
        public static readonly int[] Layers = {{
            {2}
        }};

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() {{
            Layer.PhysicsOverrideLayers = Layers;
        }}
    }}

";

    private static bool _didGenerate = false;

    static PhysicsLayerGenerator()
    {
        EditorApplication.update += CheckLayers;
    }

    private static void CheckLayers()
    {
        if (_didGenerate || !EditorPrefs.GetBool(CheckLayersPref, true))
        {
            return;
        }
        string[] layerNames = InternalEditorUtility.layers;
        List<int> layers = new();

        foreach (string layerName in layerNames)
        {
            int layer = LayerMask.NameToLayer(layerName);
            if (layers.Contains(layer)) continue;

            layers.Add(layer);
        }

        bool needsRegen = false;
        for (int i = 0; i < 32; i++)
        {
            if (i == layers.Count && i == Layer.Layers.Length) break;

            if (i >= layers.Count || i >= Layer.Layers.Length || layers[i] != Layer.Layers[i])
            {
                needsRegen = true;
                break;
            }
        }

        if (needsRegen)
        {
            if (EditorUtility.DisplayDialog("Generate Layers", "Do you want to generate a PhysicsLayer.cs file?", "OK", "No"))
            {
                DoGenerate();
            }
            else
            {
                EditorPrefs.SetBool(CheckLayersPref, false);
            }
        }
    }

    /// <summary>
    /// Generate the PhysicsLayer file.
    /// </summary>
    [MenuItem("Tools/Madwise/Generate/Update PhysicsLayer.cs", priority = 0)]
    public static void Generate()
    {
        if (EditorUtility.DisplayDialog("Update PhysicsLayer.cs", "This will create or overwrite the file Assets/Scripts/PhysicsLayer.cs.", "OK", "Cancel"))
        {
            DoGenerate();
        }
    }

    /// <summary>
    /// Remove the PhysicsLayer file.
    /// </summary>
    [MenuItem("Tools/Madwise/Generate/Remove PhysicsLayer.cs", priority = 0)]
    public static void Remove()
    {
        if (EditorUtility.DisplayDialog("Remove PhysicsLayer.cs", "This will delete the generated PhysicsLayer.cs file, and revert to using only the builtin layers.", "OK", "Cancel"))
        {
            DoRemove();
        }
    }

    private static void DoGenerate()
    {
        EditorPrefs.SetBool(CheckLayersPref, true);
        _didGenerate = true;

        string layerDecls = string.Empty;
        string layerMasks = string.Empty;
        string layerArray = string.Empty;

        string[] layers = InternalEditorUtility.layers;

        HashSet<string> layerVars = new HashSet<string>();
        for (int i = 0; i < layers.Length; i++)
        {
            string layer = layers[i];

            // Remove all characters except letters, numbers, and underscore.
            string varName = Regex.Replace(layer, "\\W", "");

            // Strings are generated as verbatim, so replace single quotes with double quotes.
            int layerIndex = i;

            // Dont create empty variables or duplicate variables.
            if (!string.IsNullOrEmpty(varName) && layerVars.Add(varName))
            {
                // Ensure first character is not a digit.
                if (char.IsDigit(varName[0]))
                {
                    varName = '_' + varName;
                }

                layerDecls += string.Format(LayerTemplate, varName, layerIndex);
            }

            layerArray += string.Format(TagArrayTemplate, varName);
        }

        string generated = string.Format(Template, layerDecls, layerMasks, layerArray);

        //CoreEditorUtility.EnsureDataFolderExists();
        string defPath = Path.Combine("Assets/Scripts", "PhysicsLayer.cs");

        StreamWriter outStream = new StreamWriter(defPath);
        outStream.Write(generated);
        outStream.Close();
        AssetDatabase.Refresh();
    }

    private static void DoRemove()
    {
        EditorPrefs.SetBool(CheckLayersPref, false);
        string defPath = Path.Combine("Assets/Scripts", "PhysicsLayer.cs");
        if (File.Exists(defPath))
        {
            AssetDatabase.DeleteAsset(defPath);
            AssetDatabase.Refresh();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class CreateFiberProperties 
{
    [MenuItem("AICS/Create/FiberProperties")]
    public static void Create ()
    {
        FiberProperties asset = ScriptableObject.CreateInstance<FiberProperties>();

        AssetDatabase.CreateAsset(asset, "Assets/Data/FiberProperties/newFiberProperties.asset");
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
        EditorUtility.FocusProjectWindow();
    }
}
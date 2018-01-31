using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    public class CreateModel 
    {
        [MenuItem("Assets/Create/Model")]
        public static void Create ()
        {
            Model asset = ScriptableObject.CreateInstance<Model>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/newModel.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }
    }
}
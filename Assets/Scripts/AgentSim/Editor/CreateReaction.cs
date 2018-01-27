using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    public class CreateReaction 
    {
        [MenuItem("Assets/Create/Reaction")]
        public static void Create ()
        {
            Reaction asset = ScriptableObject.CreateInstance<Reaction>();

            AssetDatabase.CreateAsset(asset, "Assets/Reactions/newReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }
    }
}
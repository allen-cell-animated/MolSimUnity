using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    public static class CreateDataObjects 
    {
        [MenuItem("Assets/Create/Model")]
        public static void CreateModel ()
        {
            Model asset = ScriptableObject.CreateInstance<Model>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/newModel.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("Assets/Create/Molecule")]
        public static void CreateMolecule ()
        {
            Molecule asset = ScriptableObject.CreateInstance<Molecule>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/Molecules/newMolecule.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("Assets/Create/Reaction")]
        public static void CreateReaction ()
        {
            Reaction asset = ScriptableObject.CreateInstance<Reaction>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/Reactions/newReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }
    }
}
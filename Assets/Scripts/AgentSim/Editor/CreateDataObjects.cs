using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    public static class CreateDataObjects 
    {
        [MenuItem("AICS/Create/Model")]
        public static void CreateModel ()
        {
            ModelDef asset = ScriptableObject.CreateInstance<ModelDef>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Models/newModel.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Molecule")]
        public static void CreateMolecule ()
        {
            MoleculeDef asset = ScriptableObject.CreateInstance<MoleculeDef>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Molecules/newMolecule.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Reaction")]
        public static void CreateReaction ()
        {
            ReactionDef asset = ScriptableObject.CreateInstance<ReactionDef>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Reactions/newReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }
    }
}
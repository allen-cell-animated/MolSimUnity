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

        [MenuItem("AICS/Create/Reaction/StateChange")]
        public static void CreateStateChangeReaction ()
        {
            StateChangeReactionDef asset = ScriptableObject.CreateInstance<StateChangeReactionDef>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Reactions/newStateChangeReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Reaction/Bind")]
        public static void CreateBindReaction ()
        {
            BindReactionDef asset = ScriptableObject.CreateInstance<BindReactionDef>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Reactions/newBindReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Reaction/Release")]
        public static void CreateReleaseReaction ()
        {
            ReleaseReactionDef asset = ScriptableObject.CreateInstance<ReleaseReactionDef>();

            AssetDatabase.CreateAsset(asset, "Assets/Data/Reactions/newReleaseReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }
    }
}
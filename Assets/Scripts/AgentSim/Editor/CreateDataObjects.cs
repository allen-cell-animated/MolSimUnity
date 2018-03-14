using UnityEngine;
using UnityEditor;

namespace AICS.AgentSim
{
    public static class CreateDataObjects 
    {
        [MenuItem("AICS/Create/Model")]
        public static void CreateModel ()
        {
            Model asset = ScriptableObject.CreateInstance<Model>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/newModel.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Molecule")]
        public static void CreateMolecule ()
        {
            Molecule asset = ScriptableObject.CreateInstance<Molecule>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/Molecules/newMolecule.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Reaction/StateChange")]
        public static void CreateStateChangeReaction ()
        {
            StateChangeReaction asset = ScriptableObject.CreateInstance<StateChangeReaction>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/Reactions/newStateChangeReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Reaction/Bind")]
        public static void CreateBindReaction ()
        {
            BindReaction asset = ScriptableObject.CreateInstance<BindReaction>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/Reactions/newBindReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }

        [MenuItem("AICS/Create/Reaction/Release")]
        public static void CreateReleaseReaction ()
        {
            ReleaseReaction asset = ScriptableObject.CreateInstance<ReleaseReaction>();

            AssetDatabase.CreateAsset(asset, "Assets/Models/Reactions/newReleaseReaction.asset");
            AssetDatabase.SaveAssets();

            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ModelDef : ScriptableObject
    {
        [Tooltip( "Meters per scene unit" )] 
        public float scale = 1e-9f;
        [Tooltip( "Liters" )] 
        public float containerVolume;
        public ComplexConcentration[] complexes;
        public ReactionDef[] reactionDefs;
        public ReactionRateParameter[] adjustableParameters;

        public void Init ()
        {
            #region for prototyping in inspector without writing custom property drawer etc
            foreach (ComplexConcentration complex in complexes)
            {
                complex.complexPattern.Init();
            }
            #endregion

            foreach (ReactionDef reactionDef in reactionDefs)
            {
                reactionDef.Init();
            }
        }
    }

    [System.Serializable]
    public class ComplexConcentration
    {
        [Tooltip( "Molar" )] 
        public float concentration;
        public ComplexPattern complexPattern;

        public int moleculeCount
        {
            get
            {
                return complexPattern.GetNumberOfMolecules();
            }
        }

        public ComplexConcentration (ComplexPattern _complexPattern, float _concentration)
        {
            complexPattern = _complexPattern;
            complexPattern.Init();
            concentration = _concentration;
        }
    }

    [System.Serializable]
    public class ReactionRateParameter
    {
        public string label;
        public ReactionDef reactionDef;
        public Vector2 range;
    }
}
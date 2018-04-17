using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ModelDef : ScriptableObject
    {
        [Tooltip( "Meters" )] 
        public float scale = 1e-6f;
        [Tooltip( "Liters" )] 
        public float containerVolume;
        public ComplexConcentration[] complexes;
        public ReactionDef[] reactionDefs;

        public void Init ()
        {
            #region for prototyping in inspector without writing custom property drawer etc
            foreach (ComplexConcentration complex in complexes)
            {
                complex.complexSnapshot.Init();
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
        public ComplexSnapshot complexSnapshot;

        public int moleculeCount
        {
            get
            {
                return complexSnapshot.moleculeSnapshots.Length;
            }
        }

        public ComplexConcentration (ComplexSnapshot _complexSnapshot, float _concentration)
        {
            complexSnapshot = _complexSnapshot;
            complexSnapshot.Init();
            concentration = _concentration;
        }
    }
}
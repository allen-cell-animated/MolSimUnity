using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Model : ScriptableObject
    {
        [Tooltip( "L" )] 
        public float containerVolume;
        public MoleculeConcentration[] molecules;
        public Reaction[] reactions;

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (MoleculeConcentration molecule in molecules)
            {
                molecule.moleculeStateSet.Init();
            }
            foreach (Reaction reaction in reactions)
            {
                reaction.Init();
            }
        }
        #endregion
    }

    [System.Serializable]
    public class MoleculeConcentration
    {
        [Tooltip( "M" )] 
        public float concentration;
        public MoleculeStateSet moleculeStateSet;

        public string species
        {
            get
            {
                return moleculeStateSet.species;
            }
        }

        public int moleculeCount
        {
            get
            {
                return moleculeStateSet.moleculeStates.Length;
            }
        }

        public MoleculeConcentration (MoleculeStateSet _moleculeStateSet, float _concentration)
        {
            moleculeStateSet = _moleculeStateSet;
            concentration = _concentration;
        }
    }
}
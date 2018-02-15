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

        public Model (float _containerVolume, MoleculeConcentration[] _molecules, Reaction[] _reactions)
        {
            containerVolume = _containerVolume;
            molecules = _molecules;
            reactions = _reactions;
        }
    }

    [System.Serializable]
    public class MoleculeConcentration
    {
        public MoleculeState moleculeState;
        [Tooltip( "M" )] 
        public float concentration;

        public MoleculeConcentration (MoleculeState _moleculeState, float _concentration)
        {
            moleculeState = _moleculeState;
            concentration = _concentration;
        }
    }
}
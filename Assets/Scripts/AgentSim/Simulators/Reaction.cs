using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class SpeciesAmount
    {
        public string species;
        public int amount;

        public SpeciesAmount (string _species, int _amount)
        {
            species = _species;
            amount = _amount;
        }
    }

    [System.Serializable]
    public class Reaction : ScriptableObject
    {
        public string description;
        public SpeciesAmount[] reactants;
        public SpeciesAmount[] products;
        [Tooltip( "per second" )] public float rate;
    }
}
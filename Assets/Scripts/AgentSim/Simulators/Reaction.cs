using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reaction : ScriptableObject
    {
        public string description;
        [Tooltip( "max of 2 reactants" )] 
        public string[] reactantSpecies;
        [Tooltip( "max of 2 products" )] 
        public string[] productSpecies;
        [Tooltip( "per second" )] 
        public float rate;

        public Reaction (string[] _reactantSpecies, string[] _productSpecies, float _rate, string _description = "")
        {
            description = _description;
            reactantSpecies = _reactantSpecies;
            productSpecies = _productSpecies;
            rate = _rate;
        }

        public bool ReactantsEqual (string species1, string species2)
        {
            return (reactantSpecies.Length == 0 && species1 == null && species2 == null)
                || (reactantSpecies.Length == 1 && ((species1 == reactantSpecies[0] && species2 == null) 
                                                 || (species2 == reactantSpecies[0] && species1 == null)))
                || (reactantSpecies.Length == 2 && ((species1 == reactantSpecies[0] && species2 == reactantSpecies[1])
                                                 || (species2 == reactantSpecies[0] && species1 == reactantSpecies[1])));
        }
    }
}
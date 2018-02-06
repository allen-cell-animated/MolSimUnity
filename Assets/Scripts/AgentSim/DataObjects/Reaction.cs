using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reaction : ScriptableObject
    {
        public string description;
        [Tooltip( "max of 2 reactants" )] 
        public MoleculeState[] reactants;
        [Tooltip( "max of 2 products" )] 
        public MoleculeState[] products;
        [Tooltip( "per second" )] 
        public float rate;
        public Bind bind;

        public Reaction (MoleculeState[] _reactants, MoleculeState[] _products, float _rate, Bind _bind, string _description = "")
        {
            description = _description;
            reactants = _reactants;
            products = _products;
            rate = _rate;
            bind = _bind;
        }

        public bool ReactantsEqual (MoleculeState species1, MoleculeState species2)
        {
            return (reactants.Length == 0 && species1 == null && species2 == null)
                || (reactants.Length == 1 && ((species1.Matches( reactants[0] ) && species2 == null) 
                                           || (species2.Matches( reactants[0] ) && species1 == null)))
                || (reactants.Length == 2 && ((species1.Matches( reactants[0] ) && species2.Matches( reactants[1] ))
                                           || (species2.Matches( reactants[0] ) && species1.Matches( reactants[1] ) )));
        }
    }

    [System.Serializable]
    public class MoleculeState
    {
        public string species;
        public ComponentState state;

        public bool Matches (MoleculeState other)
        {
            return other.species == species && other.state.Matches( state );
        }
    }

    [System.Serializable]
    public class ComponentState
    {
        public string componentID;
        public string state;

        public bool Matches (ComponentState other)
        {
            return other.componentID == componentID && other.state == state;
        }
    }

    [System.Serializable]
    public class Bind 
    {
        public string parentSpecies;
        public string childSpecies;
        public Vector3 relativePosition;
        public Vector3 relativeRotation;

        public Bind (string _parentSpecies, string _childSpecies, Vector3 _relativePosition, Vector3 _relativeRotation)
        {
            parentSpecies = _parentSpecies;
            childSpecies = _childSpecies;
            relativePosition = _relativePosition;
            relativeRotation = _relativeRotation;
        }
    }
}
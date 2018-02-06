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

        public bool ReactantsEqual (string species1, string species2)
        {
            return (reactants.Length == 0 && species1 == null && species2 == null)
                || (reactants.Length == 1 && ((species1 == reactants[0].species && species2 == null) 
                                           || (species2 == reactants[0].species && species1 == null)))
                || (reactants.Length == 2 && ((species1 == reactants[0].species && species2 == reactants[1].species)
                                           || (species2 == reactants[0].species && species1 == reactants[1].species)));
        }

        public bool ReactantsEqual (MoleculeState species1, MoleculeState species2)
        {
            return (reactants.Length == 0 && species1 == null && species2 == null)
                || (reactants.Length == 1 && ((species1.Satisfies( reactants[0] ) && species2 == null) 
                                           || (species2.Satisfies( reactants[0] ) && species1 == null)))
                || (reactants.Length == 2 && ((species1.Satisfies( reactants[0] ) && species2.Satisfies( reactants[1] ))
                                           || (species2.Satisfies( reactants[0] ) && species1.Satisfies( reactants[1] ) )));
        }
    }

    [System.Serializable]
    public class MoleculeState
    {
        public string species;
        [SerializeField] ComponentState[] _components;

        Dictionary<string,string> _componentStates;
        public Dictionary<string,string> componentStates
        {
            get
            {
                if (_componentStates == null)
                {
                    foreach (ComponentState component in _components)
                    {
                        _componentStates.Add( component.id, component.state );
                    }
                }
                return _componentStates;
            }
        }

        public bool Satisfies (MoleculeState other)
        {
            if (other.species == species)
            {
                foreach (KeyValuePair<string,string> component in other.componentStates)
                {
                    if (componentStates[component.Key] != component.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    [System.Serializable]
    public class ComponentState
    {
        public string id;
        public string state;

        public bool Matches (ComponentState other)
        {
            return other.id == id && other.state == state;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Model : ScriptableObject
    {
        [Tooltip( "L" )] 
        public float containerVolume;
        public Molecule[] molecules;
        public Reaction[] reactions;
        public Bind[] binds;

        public Model (float _containerVolume, Molecule[] _molecules, Reaction[] _reactions)
        {
            containerVolume = _containerVolume;
            molecules = _molecules;
            reactions = _reactions;
        }

        public Bind GetBindForSpecies (string species1, string species2)
        {
            foreach (Bind bind in binds)
            {
                if ((bind.parentSpecies == species1 && bind.childSpecies == species2)
                 || (bind.parentSpecies == species2 && bind.childSpecies == species1))
                {
                    return bind;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class Molecule
    {
        public string species;
        [Tooltip( "[scale] meters" )] 
        public float radius;
        [Tooltip( "conversion factor to meters" )] 
        public float scale;
        [Tooltip( "([scale] meters)^2 / s" )]
        public float diffusionCoefficient = 3e5f;
        [Tooltip( "M" )] 
        public float initialConcentration;

        public GameObject _visualizationPrefab;
        public GameObject visualizationPrefab
        {
            get
            {
                if (_visualizationPrefab == null)
                {
                    _visualizationPrefab = Resources.Load( "DefaultMolecule" ) as GameObject;
                }
                return _visualizationPrefab;
            }
        }

        public Molecule (string _species, float _radius, float _scale, float _initialConcentration, GameObject _prefab = null)
        {
            species = _species;
            radius = _radius;
            scale = _scale;
            initialConcentration = _initialConcentration;
            _visualizationPrefab = _prefab;
        }
    }

    [System.Serializable]
    public class Reaction
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
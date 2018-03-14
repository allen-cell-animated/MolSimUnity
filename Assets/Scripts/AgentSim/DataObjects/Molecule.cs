using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule : ScriptableObject
    {
        public string species;
        [Tooltip( "[scale] meters" )] 
        public float radius;
        [Tooltip( "conversion factor to meters" )] 
        public float scale;
        [Tooltip( "([scale] meters)^2 / s" )]
        public float diffusionCoefficient = 3e5f;
        public BindingSite[] sites = new BindingSite[0];

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

        public BindingSite GetSiteByID (string id)
        {
            foreach (BindingSite site in sites)
            {
                if (site.id == id)
                {
                    return site;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class BindingSite
    {
        public string id;
        public string[] states;
        public string[] activeStates;
        public RelativeTransform transformOnMolecule;
        public float radius;
    }

    [System.Serializable]
    public class RelativeTransform
    {
        public Vector3 position;
        public Vector3 rotation;

        public RelativeTransform (Vector3 _position, Vector3 _rotation)
        {
            position = _position;
            rotation = _rotation;
        }

        public void Apply (Transform parent, Transform child)
        {
            child.position = parent.TransformPoint( position );
            child.rotation = parent.rotation * Quaternion.Euler( rotation );
        }
    }
}
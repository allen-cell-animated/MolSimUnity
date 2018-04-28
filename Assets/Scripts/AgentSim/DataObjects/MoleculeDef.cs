using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeDef : ScriptableObject
    {
        [SerializeField] string _moleculeName = "";
        public string moleculeName
        {
            get
            {
                return _moleculeName;
            }
        }

        [Tooltip( "[scale] meters" )] 
        public float radius;
        [Tooltip( "conversion factor to meters" )] 
        public float scale;
        [Tooltip( "([scale] meters)^2 / s" )]
        public float diffusionCoefficient = 3e5f;
        public Dictionary<string,List<ComponentDef>> componentDefs;

        #region for prototyping in inspector without writing custom property drawer etc
        [SerializeField] ComponentDef[] componentDefsTemp = new ComponentDef[0];
        public MoleculePatternColor[] colors;

        public void Init ()
        {
            componentDefs = new Dictionary<string, List<ComponentDef>>();
            foreach (ComponentDef componentDef in componentDefsTemp)
            {
                if (!componentDefs.ContainsKey( componentDef.componentName ))
                {
                    componentDefs.Add( componentDef.componentName, new List<ComponentDef>() );
                }
                componentDefs[componentDef.componentName].Add( componentDef );
            }
            if (colors != null)
            {
                foreach (MoleculePatternColor color in colors)
                {
                    color.pattern.InitSiteStates();
                }
            }
        }
        #endregion

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

        public override bool Equals (object obj)
        {
            MoleculeDef other = obj as MoleculeDef;
            if (other != null)
            {
                return other.moleculeName == moleculeName;
            }
            return false;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                return 16777619 * (moleculeName == null ? 0 : moleculeName.GetHashCode());
            }
        }

		public override string ToString()
		{
            return "molecule " + moleculeName;
		}
	}

    [System.Serializable]
    public class ComponentDef
    {
        public string componentName;
        public string[] states;
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

    [System.Serializable]
    public class MoleculePatternColor
    {
        public MoleculePattern pattern;
        public Color color;
    }
}
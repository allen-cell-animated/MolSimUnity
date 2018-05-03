using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ComponentPattern : IComponent
    {
        [SerializeField] string _componentName;
        public string componentName
        {
            get
            {
                return _componentName;
            }
        }

        [SerializeField] string _state;
        public string state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public ComponentPattern (string _theComponentName, string _theState)
        {
            _componentName = _theComponentName;
            state = _theState;
        }

        public ComponentPattern (MoleculeComponent moleculeComponent)
        {
            _componentName = moleculeComponent.componentName;
            state = moleculeComponent.state;
        }

        public bool MatchesID (IComponent other)
        {
            return other.componentName == componentName;
        }

        public override bool Equals (object obj)
        {
            IComponent other = obj as IComponent;
            if (other != null)
            {
                return other.Matches( this );
            }
            return false;
        }

        public bool Matches (IComponent other)
        {
            return other.componentName == componentName && (other.state.Contains( "!" ) ? state.Contains( "!" ) : other.state == state);
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                return 16777619 * ((componentName == null) ? 1 : componentName.GetHashCode()) + 7237 * ((state == null) ? 1 : (state.Contains( "!" ) ? "!".GetHashCode() : state.GetHashCode()));
            }
        }

        public override string ToString()
        {
            return "(" + componentName + " state = " + state + ")";
        }
    }
}

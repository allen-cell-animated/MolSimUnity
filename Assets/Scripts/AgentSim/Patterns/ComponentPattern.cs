﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ComponentPattern : IComponent
    {
        #region IComponent
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

        [SerializeField] bool _bound;
        public bool bound
        {
            get
            {
                return _bound;
            }
        }

        public bool Matches (IComponent other)
        {
            return other.componentName == componentName && other.state == state && other.bound == bound;
        }
        #endregion

        public string bondName;

        public ComponentPattern (string _theComponentName, string _theState, bool _isBound)
        {
            _componentName = _theComponentName;
            state = _theState;
            _bound = _isBound;
        }

        public ComponentPattern (MoleculeComponent moleculeComponent)
        {
            _componentName = moleculeComponent.componentName;
            state = moleculeComponent.state;
            _bound = moleculeComponent.bound;
        }

        public virtual void SetStateOfComponent (MoleculeComponent component)
        {
            //UnityEngine.Profiling.Profiler.BeginSample("Set State");
            component.state = state;
            component.lastBondName = bound ? bondName : string.Empty;
            //UnityEngine.Profiling.Profiler.EndSample();
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

        public override int GetHashCode ()
        {
            unchecked
            {
                return 16777619 * ((componentName == null) ? 1 : componentName.GetHashCode()) + 7237 * ((state == null) ? 1 : state.GetHashCode()) + 7237 * bound.GetHashCode();
            }
        }

        public override string ToString()
        {
            return "(" + componentName + "~" + state + (bound ? "!" + (string.IsNullOrEmpty( bondName ) ? "+" : bondName ) : "") + ")";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class Agent : MonoBehaviour 
	{
        public string species;
        [Tooltip( "conversion factor to meters" )] 
		public float scale = 1e-9f;

		Agent _parent;
		public Agent parent
		{
			get
			{
				if (_parent == null)
				{
					_parent = GetComponentInParent<Agent>();
				}
				return _parent;
			}
            set
            {
                _parent = value;
            }
		}

		List<Agent> _children;
		public List<Agent> children
		{
			get
			{
				if (_children == null)
				{
					Agent _agent;
					_children = new List<Agent>();
					foreach (Transform child in transform)
					{
						_agent = child.GetComponent<Agent>();
						if (_agent != null)
						{
							_children.Add( _agent );
						}
					}
				}
				return _children;
			}
        }

		Simulator[] _simulators;
		public Simulator[] simulators
		{
			get 
			{
				if (_simulators == null)
				{
					_simulators = GetComponents<Simulator>();
				}
				return _simulators;
			}
		}

        public void Init (string _species, float _scale)
        {
            species = _species;
            scale = _scale;
        }

		public void UpdateBy (float dTime)
		{
			foreach (Agent child in children)
			{
                child.UpdateBy( dTime );
			}
            UpdateSelfBy( dTime );
		}

        void UpdateSelfBy (float dTime)
		{
			foreach (Simulator simulator in simulators)
			{
                simulator.SimulateFor( dTime );
			}
        }

        public void SetParent (Agent newParent)
        {
            parent.RemoveChild( this );
            newParent.AddChild( this );
            parent = newParent;
        }

        void AddChild (Agent child)
        {
            if (!children.Contains( child ))
            {
                children.Add( child );
                child.transform.SetParent( transform );
            }
            else
            {
                Debug.LogWarning( "Add: " + name + " already has child " + child.name );
            }
        }

        void RemoveChild (Agent child)
        {
            if (children.Contains( child ))
            {
                children.Remove( child );
            }
            else
            {
                Debug.LogWarning( "Remove: " + name + " doesn't have child " + child.name );
            }
        }
	}
}
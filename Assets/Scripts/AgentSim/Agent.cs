using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class Agent : MonoBehaviour 
	{
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

		ISimulator[] _simulators;
		public ISimulator[] simulators
		{
			get 
			{
				if (_simulators == null)
				{
					_simulators = GetComponents<ISimulator>();
				}
				return _simulators;
			}
		}

		public void UpdateTo (float time)
		{
			foreach (Agent child in children)
			{
				child.UpdateTo( time );
			}
			UpdateSelf( time );
		}

		void UpdateSelf (float time)
		{
			foreach (ISimulator simulator in simulators)
			{
				simulator.SimulateTo( time );
			}
		}
	}
}
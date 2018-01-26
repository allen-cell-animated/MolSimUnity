using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public abstract class Simulator : MonoBehaviour
	{
		Agent _agent;
		public Agent agent
		{
			get
			{
				if (_agent == null)
				{
					_agent = GetComponentInParent<Agent>();
				}
				return _agent;
			}
		}

		public abstract void SimulateTo (float time);
	}
}
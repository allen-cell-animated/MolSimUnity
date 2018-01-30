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
					_agent = GetComponent<Agent>();
				}
				return _agent;
			}
		}

        void Start ()
        {
            Setup();
        }

        protected abstract void Setup ();

        public abstract void SimulateFor (float dTime);
	}
}
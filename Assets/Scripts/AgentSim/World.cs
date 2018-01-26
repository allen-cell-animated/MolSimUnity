using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class World : MonoBehaviour 
	{
		[Tooltip("current time in seconds")] 
		public float time = 0; 
		public Agent[] highestAgents;
		public Observer observer;

		static World _Instance;
		public static World Instance
		{
			get 
			{
				if (_Instance == null)
				{
					_Instance = GameObject.FindObjectOfType<World>();
				}
				return _Instance;
			}
		}

		float dT
		{
			get
			{
				return observer.currentScale;
			}
		}

		void Update ()
		{
			time += dT;
			foreach (Agent agent in highestAgents)
			{
				agent.UpdateTo( time );
			}
		}
	}
}
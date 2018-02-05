using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class World : MonoBehaviour 
	{
		[Tooltip("current time in seconds")] 
		public float time = 0; 
        [Tooltip("frames since start")] 
        public int steps = 0; 
        public Agent[] rootAgents = new Agent[0];
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

		public float dT
		{
			get
			{
				return observer.currentScale;
			}
		}

		void Update ()
		{
            if (observer != null)
            {
    			time += dT;
                steps++;

                foreach (Agent agent in rootAgents)
    			{
                    agent.UpdateBy( dT );
    			}
            }
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public class World : MonoBehaviour 
	{
		[Tooltip("current time in seconds")] 
		public float time; 
        [Tooltip("frames since start")] 
        public int steps; 
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
            }
		}
	}
}
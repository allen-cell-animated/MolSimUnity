﻿using System.Collections;
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

        public bool paused;

		void Update ()
		{
            if (observer != null && !paused)
            {
    			time += dT;
                steps++;
            }
        }

        static GameObject _flashPrefab;
        static GameObject flashPrefab
        {
            get
            {
                if (_flashPrefab == null)
                {
                    _flashPrefab = Resources.Load( "Flash" ) as GameObject;
                }
                return _flashPrefab;
            }
        }

        public static void ShowFlash (Transform parent)
        {
            if (flashPrefab != null)
            {
                Instantiate( flashPrefab, parent.position, Quaternion.identity, parent );
            }
        }

        public void Restart ()
        {
            time = 0;
            steps = 0;
        }
	}
}
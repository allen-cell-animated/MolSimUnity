using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.UI
{
	public class StateIndicator : MonoBehaviour 
	{
		public Transform[] states;

		public void GoToState (int newState)
		{
			if (newState < states.Length && states[newState] != null)
			{
				transform.position = states[newState].position;
			}
		}
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [RequireComponent( typeof(Collider) )]
    public class Focuser : MonoBehaviour 
    {
		void OnMouseUp()
		{
            World.Instance.observer.FocusOn( transform );
		}
	}
}
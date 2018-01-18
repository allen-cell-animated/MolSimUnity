using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
	public interface ISimulator 
	{
		void SimulateTo (float time);
	}
}
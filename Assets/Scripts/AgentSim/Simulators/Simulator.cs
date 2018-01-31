using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class Simulator : AgentComponent
	{
        public abstract void SimulateFor (float dTime);
	}
}
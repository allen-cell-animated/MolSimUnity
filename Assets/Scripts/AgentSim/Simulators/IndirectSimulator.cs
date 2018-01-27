using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class IndirectSimulator : Simulator
    {
        public Reaction[] reactions;

        public override void SimulateTo (float time)
        {
            foreach (Reaction reaction in reactions)
            {

            }
        }
    }
}
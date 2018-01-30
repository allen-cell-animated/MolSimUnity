using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class IndirectSimulator : Simulator
    {
        public Reaction[] reactions;

        protected override void Setup () { }

        public override void SimulateFor (float dTime)
        {
            foreach (Reaction reaction in reactions)
            {

            }
        }
    }
}
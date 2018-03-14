using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReaction : Reaction 
    {
        public override void React (BindingSiteSimulator reactant1, BindingSiteSimulator reactant2 = null)
        {
            if (reactant1 != null && reactant2 != null)
            {
                // TODO position relative to each other and parent, move to new population
            }
        }
    }
}
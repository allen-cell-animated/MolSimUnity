﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class StateChangeReaction : Reaction 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantStates.Length == 1 && productStates.Length == 1;
        }

        public override void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null)
        {
            if (bindingSiteSimulator1 != null)
            {
                SetComplexToFinalState( bindingSiteSimulator1.complex, productStates[0] );

                Reactor.ShowFlash( bindingSiteSimulator1.theTransform );
            }
        }
    }
}
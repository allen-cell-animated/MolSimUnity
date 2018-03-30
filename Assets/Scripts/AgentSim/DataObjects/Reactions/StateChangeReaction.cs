using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class StateChangeReaction : Reaction 
    {
        public override void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null)
        {
            if (bindingSiteSimulator1 != null)
            {
                SetFinalStateOfComplex( bindingSiteSimulator1 );

                ParticlePopulation productParticlePopulation = bindingSiteSimulator1.reactor.GetPopulationForComplex( productStates[0] );
                MoleculeSimulator[] moleculeSimulators = new MoleculeSimulator[bindingSiteSimulator1.complex.Length];
                bindingSiteSimulator1.complex.CopyTo( moleculeSimulators, 0 );
                productParticlePopulation.CreateComplexWithMoleculeSimulators( bindingSiteSimulator1.theTransform, moleculeSimulators );

                Reactor.ShowFlash( bindingSiteSimulator1.theTransform );
            }
        }
    }
}
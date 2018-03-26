using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReaction : Reaction 
    {
        public override void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null)
        {
            if (bindingSiteSimulator1 != null && bindingSiteSimulator2 != null)
            {
                bindingSiteSimulator1.boundSite = bindingSiteSimulator2;
                bindingSiteSimulator2.boundSite = bindingSiteSimulator1;

                RelativelyPosition( bindingSiteSimulator1.transform, bindingSiteSimulator2.transform );

                ParticlePopulation productParticlePopulation = bindingSiteSimulator1.reactor.GetPopulationForComplex( productStates[0] );
                List<MoleculeSimulator> moleculeSimulators = new List<MoleculeSimulator>( bindingSiteSimulator1.particleSimulator.moleculeSimulators );
                moleculeSimulators.AddRange( bindingSiteSimulator2.particleSimulator.moleculeSimulators );
                productParticlePopulation.CreateComplexWithMoleculeSimulators( bindingSiteSimulator1.transform, moleculeSimulators );

                SetFinalSiteState( bindingSiteSimulator1 );
                SetFinalSiteState( bindingSiteSimulator2 );
            }
        }

        void RelativelyPosition (Transform parentBindingSite, Transform childBindingSite)
        {
            childBindingSite.parent.position = parentBindingSite.TransformPoint( childBindingSite.InverseTransformPoint( childBindingSite.parent.position ) );
            childBindingSite.parent.rotation = childBindingSite.parent.rotation * Quaternion.Inverse( childBindingSite.rotation ) * parentBindingSite.rotation;
        }
    }
}
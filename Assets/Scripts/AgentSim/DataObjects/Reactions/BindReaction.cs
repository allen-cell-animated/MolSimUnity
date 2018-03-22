using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReaction : Reaction 
    {
        public override void React (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2 = null)
        {
            if (bindingSite1 != null && bindingSite2 != null)
            {
                bindingSite1.boundSite = bindingSite2;
                bindingSite2.boundSite = bindingSite1;

                RelativelyPosition( bindingSite1.transform, bindingSite2.transform );

                ParticlePopulation productParticlePopulation = bindingSite1.reactor.GetPopulationForComplex( productStates[0] );
                List<MoleculeSimulator> moleculeSimulators = new List<MoleculeSimulator>( bindingSite1.particleSimulator.moleculeSimulators );
                moleculeSimulators.AddRange( bindingSite2.particleSimulator.moleculeSimulators );
                productParticlePopulation.CreateComplexWithMoleculeSimulators( bindingSite1.transform, moleculeSimulators );

                SetFinalSiteState( bindingSite1 );
                SetFinalSiteState( bindingSite2 );
            }
        }

        void RelativelyPosition (Transform parentBindingSite, Transform childBindingSite)
        {
            childBindingSite.parent.position = parentBindingSite.TransformPoint( childBindingSite.InverseTransformPoint( childBindingSite.parent.position ) );
            childBindingSite.parent.rotation = childBindingSite.parent.rotation * Quaternion.Inverse( childBindingSite.rotation ) * parentBindingSite.rotation;
        }
    }
}
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

                RelativelyPosition( bindingSiteSimulator1.theTransform, bindingSiteSimulator2.theTransform );

                ParticlePopulation productParticlePopulation = bindingSiteSimulator1.reactor.GetPopulationForComplex( productStates[0] );
                MoleculeSimulator[] moleculeSimulators = new MoleculeSimulator[bindingSiteSimulator1.complex.Length + bindingSiteSimulator2.complex.Length];
                bindingSiteSimulator1.complex.CopyTo( moleculeSimulators, 0 );
                bindingSiteSimulator2.complex.CopyTo( moleculeSimulators, bindingSiteSimulator1.complex.Length );
                productParticlePopulation.CreateComplexWithMoleculeSimulators( bindingSiteSimulator1.theTransform, moleculeSimulators );

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
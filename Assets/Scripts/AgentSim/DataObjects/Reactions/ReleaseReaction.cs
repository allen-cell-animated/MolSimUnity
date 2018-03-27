using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ReleaseReaction : Reaction 
    {
        public override void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null)
        {
            if (bindingSiteSimulator1 != null)
            {
                bindingSiteSimulator2 = bindingSiteSimulator1.boundSite;

                SetFinalSiteState( bindingSiteSimulator1 );
                SetFinalSiteState( bindingSiteSimulator2 );

                BindingSiteSimulator bindingSiteSimulator;
                ParticlePopulation productPopulation;
                foreach (ComplexState productState in productStates)
                {
                    bindingSiteSimulator = GetBindingSiteForProductState( productState, bindingSiteSimulator1, bindingSiteSimulator2 );
                    productPopulation = bindingSiteSimulator1.reactor.GetPopulationForComplex( productState );
                    productPopulation.CreateComplexWithMoleculeSimulators( bindingSiteSimulator.moleculeSimulator.theTransform, 
                                                                           bindingSiteSimulator.particleSimulator.GetComplexAtEndOfBond( bindingSiteSimulator ) );
                }

                bindingSiteSimulator1.boundSite = null;
                bindingSiteSimulator2.boundSite = null;
            }
        }

        BindingSiteSimulator GetBindingSiteForProductState (ComplexState productState, BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2)
        {
            foreach (MoleculeState moleculeState in productState.moleculeStates)
            {
                if (moleculeState.molecule.species == bindingSiteSimulator1.species && moleculeState.ContainsBindingSite( bindingSiteSimulator1.id ) )
                {
                    return bindingSiteSimulator1;
                }
                if (moleculeState.molecule.species == bindingSiteSimulator2.species && moleculeState.ContainsBindingSite( bindingSiteSimulator2.id ) )
                {
                    return bindingSiteSimulator2;
                }
            }
            return null;
        }
    }
}
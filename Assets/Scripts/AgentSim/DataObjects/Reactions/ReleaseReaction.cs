using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ReleaseReaction : Reaction 
    {
        public override void React (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2 = null)
        {
            if (bindingSite1 != null)
            {
                bindingSite2 = bindingSite1.boundSite;

                SetFinalSiteState( bindingSite1 );
                SetFinalSiteState( bindingSite2 );

                ParticlePopulation productPopulation;
                BindingSiteSimulator bindingSite;
                foreach (ComplexState productState in productStates)
                {
                    bindingSite = GetBindingSiteForProductState( productState, bindingSite1, bindingSite2 );
                    productPopulation = bindingSite1.reactor.GetPopulationForComplex( productState );
                    productPopulation.CreateComplexWithMolecules( bindingSite.molecule.transform, new List<MoleculeSimulator>( new MoleculeSimulator[]{bindingSite.molecule} ) );
                }

                bindingSite1.boundSite = null;
                bindingSite2.boundSite = null;
            }
        }

        BindingSiteSimulator GetBindingSiteForProductState (ComplexState productState, BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2)
        {
            foreach (MoleculeState moleculeState in productState.moleculeStates)
            {
                if (moleculeState.species == bindingSite1.molecule.species && moleculeState.ContainsBindingSite( bindingSite1.id ) )
                {
                    return bindingSite1;
                }
                if (moleculeState.species == bindingSite2.molecule.species && moleculeState.ContainsBindingSite( bindingSite2.id ) )
                {
                    return bindingSite2;
                }
            }
            return null;
        }
    }
}
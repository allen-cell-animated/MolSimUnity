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
                if (bindingSite1.boundSite == null)
                {
                    Debug.Log( bindingSite1.name );
                    UnityEditor.EditorApplication.isPaused = true;
                }
                else 
                {
                    bindingSite2 = bindingSite1.boundSite;

                    SetFinalSiteState( bindingSite1 );
                    SetFinalSiteState( bindingSite2 );

                    BindingSiteSimulator bindingSite;
                    ParticlePopulation productPopulation;
                    foreach (ComplexState productState in productStates)
                    {
                        bindingSite = GetBindingSiteForProductState( productState, bindingSite1, bindingSite2 );
                        productPopulation = bindingSite1.reactor.GetPopulationForComplex( productState );
                        productPopulation.CreateComplexWithMoleculeSimulators( bindingSite.moleculeSimulator.transform, 
                                                                               new List<MoleculeSimulator>( bindingSite.particleSimulator.GetMoleculesAtEndOfBond( bindingSite ) ) );
                    }

                    bindingSite1.boundSite = null;
                    bindingSite2.boundSite = null;
                }
            }
        }

        BindingSiteSimulator GetBindingSiteForProductState (ComplexState productState, BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2)
        {
            foreach (MoleculeState moleculeState in productState.moleculeStates)
            {
                if (moleculeState.molecule.species == bindingSite1.species && moleculeState.ContainsBindingSite( bindingSite1.id ) )
                {
                    return bindingSite1;
                }
                if (moleculeState.molecule.species == bindingSite2.species && moleculeState.ContainsBindingSite( bindingSite2.id ) )
                {
                    return bindingSite2;
                }
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ReleaseReaction : Reaction 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantStates.Length == 1 && productStates.Length == 2;
        }

        public override void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null)
        {
            if (bindingSiteSimulator1 != null)
            {
                bindingSiteSimulator2 = bindingSiteSimulator1.boundSite;

                bindingSiteSimulator1.boundSite = null;
                bindingSiteSimulator2.boundSite = null;

                BindingSiteSimulator bindingSiteSimulator;
                MoleculeSimulator[] complex;
                foreach (ComplexState productState in productStates)
                {
                    bindingSiteSimulator = GetBindingSiteForProductState( productState, bindingSiteSimulator1, bindingSiteSimulator2 );
                    complex = bindingSiteSimulator.complexSimulator.GetComplexAtEndOfBond( bindingSiteSimulator );
                    SetComplexToFinalState( complex, productState );
                    bindingSiteSimulator.reactor.spawner.CreateComplex( bindingSiteSimulator.moleculeSimulator.theTransform, complex );

                    SetProductColor( complex );
                }

                World.ShowFlash( bindingSiteSimulator1.theTransform );
            }
        }

        BindingSiteSimulator GetBindingSiteForProductState (ComplexState productState, BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2)
        {
            foreach (MoleculeState moleculeState in productState.moleculeStates)
            {
                if (moleculeState.molecule.species == bindingSiteSimulator1.molecule.species && moleculeState.ContainsBindingSite( bindingSiteSimulator1.id ) )
                {
                    return bindingSiteSimulator1;
                }
                if (moleculeState.molecule.species == bindingSiteSimulator2.molecule.species && moleculeState.ContainsBindingSite( bindingSiteSimulator2.id ) )
                {
                    return bindingSiteSimulator2;
                }
            }
            return null;
        }
    }
}
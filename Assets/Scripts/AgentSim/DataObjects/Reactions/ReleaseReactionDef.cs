using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ReleaseReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantPatterns.Length == 1 && productPatterns.Length == 2;
        }

        public override bool React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null && component1.boundComponent != null)
            {
                component2 = component1.boundComponent;

                MoleculeComponent[] components = {component1, component2};
                ComplexPattern productPattern;
                Molecule[] molecules;
                foreach (MoleculeComponent component in components)
                {
                    productPattern = GetProductPatternForComponent( component );
                    if (productPattern == null) { Debug.Log( component ); }
                    molecules = component.complex.GetMoleculesAtEndOfBond( component );

                    component.boundComponent = null;
                    productPattern.SetStateOfComplex( molecules );
                    reactor.MoveMoleculesToNewComplex( molecules, component.molecule.theTransform );

                    SetProductColor( molecules );
                    AnimateReaction( molecules );
                }
                World.ShowFlash( component1.theTransform );

                return true;
            }
            return false;
        }
    }
}
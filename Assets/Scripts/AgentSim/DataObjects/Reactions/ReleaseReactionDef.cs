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

        public override void React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null)
            {
                Debug.Log( "Reaction happened: " + description );
                component2 = component1.boundComponent;

                component1.boundComponent = null;
                component2.boundComponent = null;

                MoleculeComponent component;
                Molecule[] molecules;
                foreach (ComplexPattern productPattern in productPatterns)
                {
                    component = GetComponentForProductState( productPattern, component1, component2 );
                    molecules = component.complex.GetMoleculesAtEndOfBond( component );

                    productPattern.SetStateOfComplex( molecules );
                    reactor.MoveMoleculesToNewComplex( molecules, component.molecule.theTransform );

                    SetProductColor( molecules );
                    AnimateReaction( molecules );
                }
                World.ShowFlash( component1.theTransform );
            }
        }

        protected MoleculeComponent GetComponentForProductState (ComplexPattern productPattern, MoleculeComponent component1, MoleculeComponent component2)
        {
            //TODO this won't work anymore...
            foreach (MoleculePattern moleculePattern in productPattern.moleculePatterns)
            {
                if (moleculePattern.moleculeDef.Equals( component1.moleculeDef ) && moleculePattern.ContainsComponent( component1.componentName ) )
                {
                    return component1;
                }
                if (moleculePattern.moleculeDef.Equals( component2.moleculeDef ) && moleculePattern.ContainsComponent( component2.componentName ) )
                {
                    return component2;
                }
            }
            return null;
        }
    }
}
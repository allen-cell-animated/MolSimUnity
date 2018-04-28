using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantPatterns.Length == 2 && productPatterns.Length == 1;
        }

        public override void React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null && component2 != null)
            {
                Debug.Log( "Reaction happened: " + description );
                component1.boundComponent = component2;
                component2.boundComponent = component1;

                RelativelyPosition( component1.theTransform, component2.theTransform );

                Molecule[] molecules = new Molecule[component1.molecules.Length + component2.molecules.Length];
                component1.molecules.CopyTo( molecules, 0 );
                component2.molecules.CopyTo( molecules, component1.molecules.Length );

                productPatterns[0].SetStateOfComplex( molecules );
                reactor.MoveMoleculesToNewComplex( molecules, component1.theTransform );

                SetProductColor( molecules );
                AnimateReaction( molecules );
                World.ShowFlash( component1.theTransform );
            }
        }

        protected void RelativelyPosition (Transform parentComponent, Transform childComponent)
        {
            childComponent.parent.position = parentComponent.TransformPoint( childComponent.InverseTransformPoint( childComponent.parent.position ) );
            childComponent.parent.rotation = childComponent.parent.rotation * Quaternion.Inverse( childComponent.rotation ) * parentComponent.rotation;
        }
    }
}
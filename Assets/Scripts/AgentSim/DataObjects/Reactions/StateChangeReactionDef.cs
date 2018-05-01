using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class StateChangeReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantPatterns.Length == 1 && productPatterns.Length == 1;
        }

        public override bool React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null)
            {
                productPatterns[0].SetStateOfComplex( component1.molecules );
                component1.complex.UpdateReactions();

                SetProductColor( component1.molecules );
                AnimateReaction( component1.molecules );
                World.ShowFlash( component1.theTransform );

                return true;
            }
            return false;
        }
    }
}
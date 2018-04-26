using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class StateChangeReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantSnapshots.Length == 1 && productSnapshots.Length == 1;
        }

        public override void React (Reactor reactor, BindingSite bindingSite1, BindingSite bindingSite2 = null)
        {
            if (bindingSite1 != null)
            {
                Debug.Log( "Reaction happened: " + description );
                productSnapshots[0].SetStateOfComplex( bindingSite1.molecules );
                bindingSite1.complex.UpdateReactions();

                SetProductColor( bindingSite1.molecules );
                AnimateReaction( bindingSite1.molecules );
                World.ShowFlash( bindingSite1.theTransform );
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class StateChangeReaction : CollisionFreeReaction
    {
        public StateChangeReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public override bool React (MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null)
            {
                SetReactantsToProductState( new MoleculeComponent[]{component1} );
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
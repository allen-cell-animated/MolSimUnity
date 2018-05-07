using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class StateChangeReaction : CollisionFreeReaction
    {
        public StateChangeReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public override bool React (MoleculeComponent[] components, ReactionCenter[] matchingReactionCenters)
        {
            if (components.Length > 0 && components[0] != null &&
                matchingReactionCenters.Length > 0 && matchingReactionCenters[0] != null)
            {
                components[0].SetToProductState( matchingReactionCenters[0] );
                components[0].complex.UpdateReactions();

                SetProductColor( components[0].molecules );
                AnimateReaction( components[0].molecules );
                World.ShowFlash( components[0].theTransform );

                return true;
            }
            return false;
        }
    }
}
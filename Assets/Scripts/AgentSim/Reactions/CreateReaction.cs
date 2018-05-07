using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class CreateReaction : CollisionFreeReaction
    {
        public CreateReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public override bool React (MoleculeComponent[] components, ReactionCenter[] matchingReactionCenters)
        {
            // TODO implement create reaction
            return false;
        }
    }
}
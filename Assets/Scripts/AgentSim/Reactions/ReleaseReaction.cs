using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ReleaseReaction : CollisionFreeReaction
    {
        public ReleaseReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public override bool React (MoleculeComponent[] components, ReactionCenter[] matchingReactionCenters)
        {
            if (components.Length > 0 && components[0] != null &&
                matchingReactionCenters.Length > 0 && matchingReactionCenters[0] != null)
            {
                components = new MoleculeComponent[]{components[0], components[0].boundComponent};
                matchingReactionCenters = new ReactionCenter[]{matchingReactionCenters[0], GetOtherReactionCenter( matchingReactionCenters[0] )};
                Dictionary<string,List<Molecule>> molecules;
                Complex newComplex;
                for (int i = 0; i < components.Length; i++)
                {
                    molecules = components[i].complex.GetMoleculesAtEndOfBond( components[i] );
                    newComplex = reactor.MoveMoleculesToNewComplex( molecules );

                    components[i].SetToProductState( definition.reactionCenters[i] );
                    components[i].boundComponent = null;
                    newComplex.SetToProductState( matchingReactionCenters[i] );
                    newComplex.UpdateReactions();

                    SetProductColor( molecules );
                }

                return true;
            }
            return false;
        }

        ReactionCenter GetOtherReactionCenter (ReactionCenter reactionCenter)
        {
            foreach (ReactionCenter rc in definition.reactionCenters)
            {
                if (rc != reactionCenter)
                {
                    return rc;
                }
            }
            return null;
        }
    }
}
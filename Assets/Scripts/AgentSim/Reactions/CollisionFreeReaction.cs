using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public abstract class CollisionFreeReaction : Reaction
    {
        public List<MoleculeComponent> components = new List<MoleculeComponent>();

        public CollisionFreeReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public bool RegisterComponent (MoleculeComponent component)
        {
            if (!components.Contains( component ))
            {
                if (ComponentIsInReactionCenter( component ))
                {
                    components.Add( component );
                    return true;
                }
            }
            return false;
        }

        protected bool ComponentIsInReactionCenter (MoleculeComponent component)
        {
            return definition.reactionCenters[0].reactantMolecule.Matches( component.molecule ) && 
                   definition.reactionCenters[0].reactantComponent.Matches( component );
        }

        public void UnregisterComponent (MoleculeComponent component)
        {
            if (components.Contains( component ))
            {
                components.Remove( component );
            }
        }

        public virtual bool TryReact ()
        {
            if (components.Count > 0 && ShouldHappen())
            {
                components.Shuffle();
                return React( new MoleculeComponent[]{components[0]}, new ReactionCenter[]{definition.reactionCenters[0]} );
            }
            return false;
        }
    }
}
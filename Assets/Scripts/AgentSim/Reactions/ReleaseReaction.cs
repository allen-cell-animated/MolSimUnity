using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ReleaseReaction : CollisionFreeReaction
    {
        public ReleaseReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public override bool React (MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null && component1.boundComponent != null)
            {
                component2 = component1.boundComponent;

                MoleculeComponent[] components = {component1, component2};
                Dictionary<string,List<Molecule>> molecules;
                foreach (MoleculeComponent component in components)
                {
                    component.boundComponent = null;
                    SetReactantsToProductState( new MoleculeComponent[]{component} );

                    molecules = component.complex.GetMoleculesAtEndOfBond( component );
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
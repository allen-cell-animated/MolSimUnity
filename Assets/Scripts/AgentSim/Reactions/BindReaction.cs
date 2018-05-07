using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class BindReaction : Reaction
    {
        public BindReaction (ReactionDef _reactionDef, Reactor _reactor) : base (_reactionDef, _reactor) { }

        public override bool React (MoleculeComponent[] components, ReactionCenter[] matchingReactionCenters)
        {
            if (components.Length > 1 && components[0] != null && components[1] != null &&
                matchingReactionCenters.Length > 1 && matchingReactionCenters[0] != null && matchingReactionCenters[1] != null)
            {
                Dictionary<string,List<Molecule>> molecules = MergeMolecules( components[0].molecules, components[1].molecules );
                Complex newComplex = reactor.MoveMoleculesToNewComplex( molecules, components[0].theTransform );

                for (int i = 0; i < components.Length; i++)
                {
                    components[i].SetToProductState( matchingReactionCenters[i] );
                    components[i].boundComponent = components[1 - i];
                }
                newComplex.UpdateReactions();

                RelativelyPosition( components[0].theTransform, components[1].theTransform );

                SetProductColor( molecules );
                AnimateReaction( molecules );
                World.ShowFlash( components[0].theTransform );

                return true;
            }
            return false;
        }

        protected void RelativelyPosition (Transform parentComponent, Transform childComponent)
        {
            childComponent.parent.position = parentComponent.TransformPoint( childComponent.InverseTransformPoint( childComponent.parent.position ) );
            childComponent.parent.rotation = childComponent.parent.rotation * Quaternion.Inverse( childComponent.rotation ) * parentComponent.rotation;
        }

        protected Dictionary<string,List<Molecule>> MergeMolecules (Dictionary<string,List<Molecule>> molecules1, Dictionary<string,List<Molecule>> molecules2)
        {
            Dictionary<string,List<Molecule>> newMolecules = new Dictionary<string,List<Molecule>>();
            foreach (string moleculeName in molecules1.Keys)
            {
                newMolecules.Add( moleculeName, new List<Molecule>() );
                newMolecules[moleculeName].AddRange( molecules1[moleculeName] );
            }
            foreach (string moleculeName in molecules2.Keys)
            {
                if (!newMolecules.ContainsKey( moleculeName ))
                {
                    newMolecules.Add( moleculeName, new List<Molecule>() );
                }
                newMolecules[moleculeName].AddRange( molecules2[moleculeName] );
            }
            return newMolecules;
        }
    }
}
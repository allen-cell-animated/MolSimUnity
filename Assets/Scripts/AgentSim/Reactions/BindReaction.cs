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
                RelativelyPosition( components[0], components[1] );

                Dictionary<string,List<Molecule>> molecules = MergeMolecules( components[0].complex.molecules, components[1].complex.molecules );
                Complex newComplex = reactor.MoveMoleculesToNewComplex( molecules );

                for (int i = 0; i < components.Length; i++)
                {
                    components[i].SetToProductState( matchingReactionCenters[i] );
                    components[i].boundComponent = components[1 - i];
                }

                if (newComplex.GetNumberOfMolecules() > 2)
                {
                    newComplex.SetToProductState( matchingReactionCenters[0].productComplex, components[0].molecule, matchingReactionCenters[0].productMolecule,
                                                  components[1].molecule, matchingReactionCenters[1].productMolecule);
                }
                else
                {
                    newComplex.SetToProductState( matchingReactionCenters[0] );
                }
                newComplex.UpdateReactions();

                SetProductColor( molecules );

                return true;
            }
            return false;
        }

        protected void RelativelyPosition (MoleculeComponent parentComponent, MoleculeComponent childComponent)
        {
            // move the complex with the fewest molecules
            int childMoleculeCount = childComponent.complex.GetNumberOfMolecules();
            int parentMoleculeCount = parentComponent.complex.GetNumberOfMolecules();
            if (parentMoleculeCount < childMoleculeCount)
            {
                MoleculeComponent tempComponent = childComponent;
                childComponent = parentComponent;
                parentComponent = tempComponent;

                int tempCount = childMoleculeCount;
                childMoleculeCount = parentMoleculeCount;
                parentMoleculeCount = tempCount;
            }

            childComponent.molecule.SetWorldTransform( reactor.GetWorldTransformForBindingMolecule( childComponent.molecule.position, childComponent.molecule.rotation, childComponent.position,
                                                                                                    childComponent.rotation, parentComponent.position, parentComponent.rotation) );
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
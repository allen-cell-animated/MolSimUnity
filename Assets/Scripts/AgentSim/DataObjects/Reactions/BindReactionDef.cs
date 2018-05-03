using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantPatterns.Length == 2 && productPatterns.Length == 1;
        }

        public override bool React (Reactor reactor, MoleculeComponent component1, MoleculeComponent component2 = null)
        {
            if (component1 != null && component2 != null)
            {
                component1.boundComponent = component2;
                component2.boundComponent = component1;

                RelativelyPosition( component1.theTransform, component2.theTransform );

                Dictionary<string,List<Molecule>> molecules = MergeMolecules( component1.molecules, component2.molecules );

                productPatterns[0].SetStateOfComplex( molecules );
                reactor.MoveMoleculesToNewComplex( molecules, component1.theTransform );

                SetProductColor( molecules );
                AnimateReaction( molecules );
                World.ShowFlash( component1.theTransform );

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
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ReleaseReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantSnapshots.Length == 1 && productSnapshots.Length == 2;
        }

        public override void React (Reactor reactor, BindingSite bindingSite1, BindingSite bindingSite2 = null)
        {
            if (bindingSite1 != null)
            {
                bindingSite2 = bindingSite1.boundSite;

                bindingSite1.boundSite = null;
                bindingSite2.boundSite = null;

                BindingSite bindingSite;
                Molecule[] molecules;
                foreach (ComplexSnapshot productSnapshot in productSnapshots)
                {
                    bindingSite = GetBindingSiteForProductState( productSnapshot, bindingSite1, bindingSite2 );
                    molecules = bindingSite.complex.GetMoleculesAtEndOfBond( bindingSite );

                    SetMoleculesToFinalState( molecules, productSnapshot );
                    reactor.MoveMoleculesToNewComplex( molecules, bindingSite.molecule.theTransform );

                    SetProductColor( molecules );
                    AnimateReaction( molecules );
                }
                World.ShowFlash( bindingSite1.theTransform );
            }
        }

        protected BindingSite GetBindingSiteForProductState (ComplexSnapshot productSnapshot, BindingSite bindingSite1, BindingSite bindingSite2)
        {
            //TODO this won't work for binding sites with same ID that are in different states
            foreach (MoleculeSnapshot moleculeSnapshot in productSnapshot.moleculeSnapshots)
            {
                if (moleculeSnapshot.moleculeDef.Equals( bindingSite1.moleculeDef ) && moleculeSnapshot.ContainsBindingSite( bindingSite1.definition.bindingSiteRef ) )
                {
                    return bindingSite1;
                }
                if (moleculeSnapshot.moleculeDef.Equals( bindingSite2.moleculeDef ) && moleculeSnapshot.ContainsBindingSite( bindingSite2.definition.bindingSiteRef ) )
                {
                    return bindingSite2;
                }
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReactionDef : ReactionDef 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantSnapshots.Length == 2 && productSnapshots.Length == 1;
        }

        public override void React (Reactor reactor, BindingSite bindingSite1, BindingSite bindingSite2 = null)
        {
            if (bindingSite1 != null && bindingSite2 != null)
            {
                bindingSite1.boundSite = bindingSite2;
                bindingSite2.boundSite = bindingSite1;

                RelativelyPosition( bindingSite1.theTransform, bindingSite2.theTransform );

                Molecule[] molecules = new Molecule[bindingSite1.molecules.Length + bindingSite2.molecules.Length];
                bindingSite1.molecules.CopyTo( molecules, 0 );
                bindingSite2.molecules.CopyTo( molecules, bindingSite1.molecules.Length );

                SetComplexToFinalState( molecules, productSnapshots[0] );
                reactor.spawner.CreateComplex( bindingSite1.theTransform, molecules, reactor );

                SetProductColor( molecules );
                World.ShowFlash( bindingSite1.theTransform );
            }
        }

        void RelativelyPosition (Transform parentBindingSite, Transform childBindingSite)
        {
            childBindingSite.parent.position = parentBindingSite.TransformPoint( childBindingSite.InverseTransformPoint( childBindingSite.parent.position ) );
            childBindingSite.parent.rotation = childBindingSite.parent.rotation * Quaternion.Inverse( childBindingSite.rotation ) * parentBindingSite.rotation;
        }
    }
}
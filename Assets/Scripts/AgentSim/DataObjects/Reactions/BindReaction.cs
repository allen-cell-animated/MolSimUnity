using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReaction : Reaction 
    {
        public override void React (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2 = null)
        {
            if (bindingSite1 != null && bindingSite2 != null)
            {
                bindingSite1.molecule.ToggleMotion( false );
                bindingSite2.molecule.ToggleMotion( false );
                RelativelyPositionAndParent( bindingSite1.transform, bindingSite2.transform );
                MoleculePopulation productPopulation = bindingSite1.reactor.GetPopulationForMoleculeSet( products[0] );
                productPopulation.SpawnMoleculeComplex( bindingSite1.transform.position, new MoleculeSimulator[]{bindingSite1.molecule, bindingSite2.molecule});

                //bindingSite1.state = "";
            }
        }

        void RelativelyPositionAndParent (Transform parentBindingSite, Transform childBindingSite)
        {
            childBindingSite.parent.position = parentBindingSite.TransformPoint( childBindingSite.InverseTransformPoint( childBindingSite.parent.position ) );
            childBindingSite.parent.rotation = childBindingSite.parent.rotation * Quaternion.Inverse( childBindingSite.rotation ) * parentBindingSite.rotation;
            childBindingSite.parent.SetParent( parentBindingSite.parent );
        }
    }
}
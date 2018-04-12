using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindReaction : Reaction 
    {
        protected override bool ReactantAndProductAmountsAreCorrect ()
        {
            return reactantStates.Length == 2 && productStates.Length == 1;
        }

        public override void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null)
        {
            if (bindingSiteSimulator1 != null && bindingSiteSimulator2 != null)
            {
                Population productPopulation = bindingSiteSimulator1.reactor.GetPopulation( productStates[0] );
                MoleculeSimulator[] complex = new MoleculeSimulator[bindingSiteSimulator1.complex.Length + bindingSiteSimulator2.complex.Length];
                bindingSiteSimulator1.complex.CopyTo( complex, 0 );
                bindingSiteSimulator2.complex.CopyTo( complex, bindingSiteSimulator1.complex.Length );

                CreateBond( bindingSiteSimulator1, bindingSiteSimulator2, complex );
                SetComplexToFinalState( complex, productStates[0] );
                productPopulation.CreateComplex( bindingSiteSimulator1.theTransform, complex );

                SetProductColor( complex );
                World.ShowFlash( bindingSiteSimulator1.theTransform );
            }
        }

        void CreateBond (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2, MoleculeSimulator[] complex)
        {
            bindingSiteSimulator1.boundSite = bindingSiteSimulator2;
            bindingSiteSimulator2.boundSite = bindingSiteSimulator1;
            bindingSiteSimulator1.state = bindingSiteSimulator2.state = "!" + GetUniqueBondIndex( complex );
            RelativelyPosition( bindingSiteSimulator1.theTransform, bindingSiteSimulator2.theTransform );
        }

        string GetUniqueBondIndex (MoleculeSimulator[] complex)
        {
            List<string> bondIndices = new List<string>();
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
                {
                    if (bindingSiteSimulator.state.Contains( "!" ))
                    {
                        bondIndices.Add( bindingSiteSimulator.state.Split( '!' )[1] );
                    }
                }
            }

            if (bondIndices.Count > 0)
            {
                // TODO get an index that's not already present
            }
            return "1";
        }

        void RelativelyPosition (Transform parentBindingSite, Transform childBindingSite)
        {
            childBindingSite.parent.position = parentBindingSite.TransformPoint( childBindingSite.InverseTransformPoint( childBindingSite.parent.position ) );
            childBindingSite.parent.rotation = childBindingSite.parent.rotation * Quaternion.Inverse( childBindingSite.rotation ) * parentBindingSite.rotation;
        }
    }
}
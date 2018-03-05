using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSitePopulation : MonoBehaviour 
    {
        public BindingSite bindingSite;
        public float radius;

        public float maxExtentFromMoleculeCenter
        {
            get
            {
                return Vector3.Magnitude( bindingSite.transformOnMolecule.position ) + radius;
            }
        }

        int[] _relevantReactionIndices;
        public int[] relevantReactionIndices
        {
            get
            {
                if (_relevantReactionIndices == null)
                {
                    //foreach (Reaction reaction in reactor.model.reactions)
                    //{
                    //    foreach (MoleculeBindingSite bindingSite in reaction.relevantSites)
                    //    {
                    //        if (molecule == bindingSite.molecule)
                    //        {

                    //        }
                    //    }
                    //}
                }
                return _relevantReactionIndices;
            }
        }

        public virtual void Init (BindingSite _bindingSite)
        {
            bindingSite = _bindingSite;
            radius = bindingSite.radius;
        }

        public bool StateIsActive (string state)
        {
            foreach (string activeState in bindingSite.activeStates)
            {
                if (state == activeState)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
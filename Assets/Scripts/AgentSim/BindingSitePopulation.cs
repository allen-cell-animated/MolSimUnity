using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSitePopulation : MonoBehaviour 
    {
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
    }
}
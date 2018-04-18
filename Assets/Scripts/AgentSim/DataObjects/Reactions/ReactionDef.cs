using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class ReactionDef : ScriptableObject
    {
        public string description;
        [Tooltip( "per second" )] 
        public float rate;
        public ComplexSnapshot[] reactantSnapshots;
        [Tooltip( "for now, molecules should be in same order in products" )]
        public ComplexSnapshot[] productSnapshots;
        //public BindingSiteReference[] relevantSiteReferences;
        public MoleculeBindingSite[] relevantSites;

        public bool isBimolecular
        {
            get
            {
                return reactantSnapshots.Length > 1;
            }
        }

        public void Init ()
        {
            if (!ReactantAndProductAmountsAreCorrect())
            {
                Debug.LogWarning( "Reaction " + name + " doesn't have the correct amount of reactant and product species for a " + GetType() );
            }

            #region for prototyping in inspector without writing custom property drawer etc
            foreach (ComplexSnapshot reactantState in reactantSnapshots)
            {
                reactantState.Init();
            }
            foreach (ComplexSnapshot productState in productSnapshots)
            {
                productState.Init();
            }
            #endregion
        }

        protected abstract bool ReactantAndProductAmountsAreCorrect ();

        public abstract void React (Reactor reactor, BindingSite bindingSite1, BindingSite bindingSite2 = null);

        protected virtual void SetMoleculesToFinalState (Molecule[] molecules, ComplexSnapshot finalSnapshot)
        {
            foreach (MoleculeSnapshot moleculeSnapshot in finalSnapshot.moleculeSnapshots) 
            {
                foreach (Molecule molecule in molecules)
                {
                    if (molecule.definition.Equals( moleculeSnapshot.moleculeDef ))
                    {
                        foreach (KeyValuePair<string,string> bindingSiteState in moleculeSnapshot.bindingSiteStates)
                        {
                            molecule.bindingSites[bindingSiteState.Key].state = bindingSiteState.Value.Contains( "!" ) ? "!" : bindingSiteState.Value;
                        }
                    }
                }
            }
        }

        protected void SetProductColor (Molecule[] molecules)
        {
            foreach (Molecule molecule in molecules)
            {
                molecule.SetColorForCurrentState();
            }
        }
    }

    //[System.Serializable]
    //public class BindingSiteReference
    //{
    //    public int complexIndex;
    //    public int moleculeIndex;
    //    public string siteID;
    //}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public abstract class Reaction : ScriptableObject
    {
        public string description;
        [Tooltip( "per second" )] 
        public float rate;
        public ComplexState[] reactantStates;
        public ComplexState[] productStates;
        public MoleculeBindingSite[] relevantSites;

        public bool isBimolecular
        {
            get
            {
                return reactantStates.Length > 1;
            }
        }

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (ComplexState reactantState in reactantStates)
            {
                reactantState.Init();
            }
            foreach (ComplexState productState in productStates)
            {
                productState.Init();
            }
        }
        #endregion

        public string GetInitialStateOfSite (ComplexState complexState, MoleculeBindingSite moleculeBindingSite)
        {
            foreach (ComplexState reactantState in reactantStates)
            {
                if (reactantState.Matches( complexState ))
                {
                    foreach (MoleculeState moleculeState in reactantState.moleculeStates)
                    {
                        if (moleculeState.molecule == moleculeBindingSite.molecule)
                        {
                            if (moleculeState.bindingSiteStates.ContainsKey( moleculeBindingSite.bindingSiteID ))
                            {
                                return moleculeState.bindingSiteStates[moleculeBindingSite.bindingSiteID];
                            }
                        }
                    }
                }
            }
            return "";
        }

        public abstract void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null);

        protected virtual void SetFinalSiteState (BindingSiteSimulator bindingSiteSimulator)
        {
            foreach (ComplexState productState in productStates)
            {
                foreach (MoleculeState moleculeState in productState.moleculeStates)
                {
                    if (moleculeState.molecule.species == bindingSiteSimulator.species)
                    {
                        if (moleculeState.bindingSiteStates.ContainsKey( bindingSiteSimulator.id ))
                        {
                            bindingSiteSimulator.state = moleculeState.bindingSiteStates[bindingSiteSimulator.id];
                            return;
                        }
                    }
                }
            }
            Debug.LogWarning( "reacting binding site " + bindingSiteSimulator.name + " doesn't match any site on product of reaction " + description );
        }
    }
}
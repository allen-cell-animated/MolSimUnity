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

        public void Init ()
        {
            if (!ReactantAndProductAmountsAreCorrect())
            {
                Debug.LogWarning( "Reaction " + name + " doesn't have the correct amount of reactant and product species for a " + GetType() );
            }

            #region for prototyping in inspector without writing custom property drawer etc
            foreach (ComplexState reactantState in reactantStates)
            {
                reactantState.Init();
            }
            foreach (ComplexState productState in productStates)
            {
                productState.Init();
            }
            #endregion
        }

        protected abstract bool ReactantAndProductAmountsAreCorrect ();

        public string GetInitialStateOfSite (ComplexState complexState, MoleculeBindingSite moleculeBindingSite)
        {
            foreach (ComplexState reactantState in reactantStates)
            {
                if (reactantState.IsSatisfiedBy( complexState ))
                {
                    foreach (MoleculeState moleculeState in reactantState.moleculeStates)
                    {
                        if (moleculeState.molecule.species == moleculeBindingSite.molecule.species)
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

        protected virtual void SetComplexToFinalState (MoleculeSimulator[] complex, ComplexState finalState)
        {
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                foreach (MoleculeState moleculeState in finalState.moleculeStates)
                {
                    if (moleculeSimulator.molecule.species == moleculeState.molecule.species)
                    {
                        foreach (KeyValuePair<string,string> bindingSiteState in moleculeState.bindingSiteStates)
                        {
                            moleculeSimulator.bindingSiteSimulators[bindingSiteState.Key].state = bindingSiteState.Value;
                        }
                    }
                }
            }
        }

        public abstract void React (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2 = null);
    }
}
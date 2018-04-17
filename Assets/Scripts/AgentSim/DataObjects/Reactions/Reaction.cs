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
        public BindingSiteReference[] relevantSiteReferences;
        public MoleculeBindingSite[] relevantSites;

        public MoleculeColor[] productmoleculeColors;

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

        public string GetInitialStateOfSite (ComplexSnapshot complexSnapshot, MoleculeBindingSite moleculeBindingSite)
        {
            foreach (ComplexSnapshot reactantSnapshot in reactantSnapshots)
            {
                if (reactantSnapshot.IsSatisfiedBy( complexSnapshot ))
                {
                    foreach (MoleculeSnapshot moleculeSnapshot in reactantSnapshot.moleculeSnapshots)
                    {
                        if (moleculeSnapshot.moleculeDef.species == moleculeBindingSite.moleculeDef.species)
                        {
                            if (moleculeSnapshot.bindingSiteStates.ContainsKey( moleculeBindingSite.bindingSiteID ))
                            {
                                return moleculeSnapshot.bindingSiteStates[moleculeBindingSite.bindingSiteID];
                            }
                        }
                    }
                }
            }
            return "";
        }

        protected virtual void SetComplexToFinalState (Molecule[] molecules, ComplexSnapshot finalSnapshot)
        {
            foreach (MoleculeSnapshot moleculeSnapshot in finalSnapshot.moleculeSnapshots) 
            {
                foreach (Molecule molecule in molecules)
                {
                    if (molecule.definition.species == moleculeSnapshot.moleculeDef.species)
                    {
                        foreach (KeyValuePair<string,string> bindingSiteState in moleculeSnapshot.bindingSiteStates)
                        {
                            molecule.bindingSites[bindingSiteState.Key].state = bindingSiteState.Value.Contains( "!" ) ? "!" : bindingSiteState.Value;
                        }
                    }
                }
            }
        }

        public abstract void React (Reactor reactor, BindingSite bindingSite1, BindingSite bindingSite2 = null);

        protected void SetProductColor (Molecule[] molecules)
        {
            if (productmoleculeColors != null)
            {
                foreach (MoleculeColor moleculeColor in productmoleculeColors)
                {
                    foreach (Molecule molecule in molecules)
                    {
                        if (molecule.definition.species == moleculeColor.definition.species)
                        {
                            molecule.SetColor( moleculeColor.color );
                            break;
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class MoleculeColor
    {
        public MoleculeDef definition;
        public Color color;
    }

    [System.Serializable]
    public class BindingSiteReference
    {
        public int complexIndex;
        public int moleculeIndex;
        public string siteID;
    }
}
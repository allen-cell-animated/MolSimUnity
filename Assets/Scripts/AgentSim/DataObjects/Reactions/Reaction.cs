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
        public MoleculeStateSet[] reactants;
        public MoleculeStateSet[] products;

        #region for prototyping in inspector without writing custom property drawer etc
        public MoleculeBindingSite[] relevantSites;

        public void Init ()
        {
            foreach (MoleculeStateSet reactant in reactants)
            {
                reactant.Init();
            }
            foreach (MoleculeStateSet product in products)
            {
                product.Init();
            }
        }
        #endregion

        public abstract void React (BindingSiteSimulator reactant1, BindingSiteSimulator reactant2 = null);
    }

    [System.Serializable]
    public class MoleculeBindingSite
    {
        public Molecule molecule;
        public string bindingSiteID;

        public MoleculeBindingSite (Molecule _molecule, string _bindingSiteID)
        {
            molecule = _molecule;
            bindingSiteID = _bindingSiteID;
        }
    }

    [System.Serializable]
    public class MoleculeStateSet
    {
        public MoleculeState[] moleculeStates;

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (MoleculeState molecule in moleculeStates)
            {
                molecule.Init();
            }
        }
        #endregion

        public string species 
        {
            get
            {
                string s = "";
                for (int i = 0; i < moleculeStates.Length; i++)
                {
                    s += moleculeStates[i].species;
                    if (i < moleculeStates.Length - 1)
                    {
                        s += ".";
                    }
                }
                return s;
            }
        }

        public bool Matches (MoleculeSimulator[] molecules)
        {
            foreach (MoleculeState moleculeState in moleculeStates)
            {
                bool stateFound = false;
                foreach (MoleculeSimulator molecule in molecules)
                {
                    if (moleculeState.Matches( molecule ))
                    {
                        stateFound = true;
                        break;
                    }
                }
                if (!stateFound)
                {
                    return false;
                }
            }
            return true;
        }
    }

    [System.Serializable]
    public class MoleculeState
    {
        public Molecule molecule;
        public Dictionary<string,string> bindingSiteStates;

        #region for prototyping in inspector without writing custom property drawer etc
        [SerializeField] SiteState[] siteStates;

        public void Init ()
        {
            bindingSiteStates = new Dictionary<string,string>();
            foreach (SiteState siteState in siteStates)
            {
                bindingSiteStates.Add( siteState.id, siteState.state );
            }
        }
        #endregion

        public string species 
        {
            get
            {
                return molecule.species;
            }
        }

        public bool Matches (MoleculeSimulator _molecule)
        {
            if (_molecule.species == molecule.species)
            {
                foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
                {
                    if (!_molecule.SiteIsInState( siteState.Key, siteState.Value ))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }

    //for prototyping in inspector without writing custom property drawer etc
    [System.Serializable]
    public class SiteState
    {
        public string id;
        public string state;
    }
}
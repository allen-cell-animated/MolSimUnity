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

        #region for prototyping in inspector without writing custom property drawer etc
        public MoleculeBindingSite[] relevantSites;

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

        public abstract void React (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2 = null);

        protected virtual string GetFinalSiteState (BindingSiteSimulator bindingSite)
        {
            foreach (ComplexState productState in productStates)
            {
                foreach (MoleculeState moleculeState in productState.moleculeStates)
                {
                    if (moleculeState.species == bindingSite.molecule.species)
                    {
                        foreach (KeyValuePair<string,string> bindingSiteState in moleculeState.bindingSiteStates)
                        {
                            if (bindingSiteState.Key == bindingSite.id)
                            {
                                return bindingSiteState.Value;
                            }
                        }
                    }
                }
            }
            Debug.LogWarning( "reacting binding site " + bindingSite.name + " doesn't match any site on product of reaction " + description );
            return "";
        }
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
    public class ComplexState
    {
        public MoleculeState[] moleculeStates;

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            foreach (MoleculeState moleculeState in moleculeStates)
            {
                moleculeState.Init();
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

        public float radius
        {
            get
            {
                if (moleculeStates.Length == 1)
                {
                    return moleculeStates[0].molecule.radius;
                }
                if (moleculeStates.Length > 1)
                {
                    float r = 0;
                    foreach (MoleculeState moleculeState in moleculeStates)
                    {
                        r += moleculeState.molecule.radius;
                    }
                    return r * 1.5f; //hack for now
                }
                return 0;
            }
        }

        public float diffusionCoefficient
        {
            get
            {
                if (moleculeStates.Length == 1)
                {
                    return moleculeStates[0].molecule.diffusionCoefficient;
                }
                if (moleculeStates.Length > 1)
                {
                    float d = 0;
                    foreach (MoleculeState moleculeState in moleculeStates)
                    {
                        d += moleculeState.molecule.diffusionCoefficient;
                    }
                    return d / (0.8f * Mathf.Pow( moleculeStates.Length, 2f )); //hack for now
                }
                return 0;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
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

        public bool Matches (MoleculeBindingSite other)
        {
            return other.molecule.species == molecule.species && other.bindingSiteID == bindingSiteID;
        }

        public bool Matches (BindingSiteSimulator bindingSiteSimulator)
        {
            return bindingSiteSimulator.molecule.species == molecule.species && bindingSiteSimulator.id == bindingSiteID;
        }

        public override string ToString ()
        {
            return "site " + bindingSiteID + " on molecule " + molecule.species;
        }
    }

    [System.Serializable]
    public class ComplexState
    {
        [SerializeField] MoleculeState[] _moleculeStates;
        public MoleculeState[] moleculeStates
        {
            get
            {
                return _moleculeStates;
            }
        }

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
                    s += moleculeStates[i].molecule.species;
                    if (i < moleculeStates.Length - 1)
                    {
                        s += ".";
                    }
                }
                return s;
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

        public ComplexState (MoleculeState[] theMoleculeStates)
        {
            _moleculeStates = theMoleculeStates;
        }

        public bool IsSatisfiedBy (ComplexState other)
        {
            foreach (MoleculeState moleculeState in moleculeStates)
            {
                bool stateFound = false;
                foreach (MoleculeState otherMoleculeState in other.moleculeStates)
                {
                    if (moleculeState.IsSatisfiedBy( otherMoleculeState ))
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

        public bool IsSatisfiedBy (MoleculeSimulator[] complex)
        {
            foreach (MoleculeState moleculeState in moleculeStates)
            {
                bool stateFound = false;
                foreach (MoleculeSimulator molecule in complex)
                {
                    if (moleculeState.IsSatisfiedBy( molecule ))
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

		public override bool Equals (object obj)
		{
            ComplexState other = obj as ComplexState;
            if (other != null)
            {
                if (other.moleculeStates.Length != moleculeStates.Length)
                {
                    return false;
                }
                foreach (MoleculeState moleculeState in moleculeStates)
                {
                    bool foundState = false;
                    if (moleculeState != null && moleculeState.molecule != null)
                    {
                        foreach (MoleculeState otherMoleculeState in other.moleculeStates)
                        {
                            if (otherMoleculeState != null && otherMoleculeState.molecule != null
                                && otherMoleculeState.molecule.species == moleculeState.molecule.species)
                            {
                                foundState = true;
                                break;
                            }
                        }
                    }
                    if (!foundState)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                int hash = 0;
                if (moleculeStates != null)
                {
                    foreach (MoleculeState moleculeState in moleculeStates)
                    {
                        hash += 7919 * (moleculeState == null ? 1 : moleculeState.GetHashCode());
                    }
                }
                return hash;
            }
        }

        public override string ToString ()
        {
            string s = "";
            int i = 0;
            foreach (MoleculeState moleculeState in moleculeStates)
            {
                s += moleculeState;
                if (i < moleculeStates.Length - 1)
                {
                    s += " | ";
                }
                i++;
            }
            return s;
        }
    }

    [System.Serializable]
    public class MoleculeState
    {
        [SerializeField] Molecule _molecule;
        public Molecule molecule 
        {
            get 
            {
                return _molecule;
            }
        }

        public Dictionary<string,string> bindingSiteStates;

        #region for prototyping in inspector without writing custom property drawer etc
        [SerializeField] SiteState[] siteStates;

        public void Init ()
        {
            molecule.Init();
            bindingSiteStates = new Dictionary<string,string>();
            foreach (SiteState siteState in siteStates)
            {
                bindingSiteStates.Add( siteState.id, siteState.state );
            }
        }
        #endregion

        public MoleculeState (Molecule theMolecule, Dictionary<string,string> _bindingSiteStates)
        {
            _molecule = theMolecule;
            molecule.Init();

            bindingSiteStates = new Dictionary<string, string>( _bindingSiteStates );
        }

        public bool IsSatisfiedBy (MoleculeState other)
        {
            if (other.molecule.species != molecule.species)
            {
                return false;
            }
            foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
            {
                if (!other.bindingSiteStates.ContainsKey( siteState.Key ))
                {
                    return false;
                }
                if (siteState.Value.Contains( "!" ))
                {
                    if (!other.bindingSiteStates[siteState.Key].Contains( "!" ))
                    {
                        return false;
                    }
                }
                else
                {
                    if (other.bindingSiteStates[siteState.Key] != siteState.Value)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsSatisfiedBy (MoleculeSimulator _moleculeSimulator)
        {
            if (_moleculeSimulator.species == molecule.species)
            {
                foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
                {
                    if (siteState.Value.Contains( "!" ))
                    {
                        if (_moleculeSimulator.bindingSiteSimulators[siteState.Key].boundSite == null)
                        {
                            return false;
                        }
                    }
                    else 
                    {
                        if (_moleculeSimulator.bindingSiteSimulators[siteState.Key].state != siteState.Value)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public override bool Equals (object obj)
        {
            MoleculeState other = obj as MoleculeState;
            if (other != null)
            {
                return other.molecule.species == molecule.species;
            }
            return false;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                return 16777619 * ((molecule == null || molecule.species == null) ? 1 : molecule.species.GetHashCode());
            }
        }

        public bool ContainsBindingSite (string bindingSiteID)
        {
            foreach (KeyValuePair<string,string> bindingSiteState in bindingSiteStates)
            {
                if (bindingSiteState.Key == bindingSiteID)
                {
                    return true;
                }
            }
            return false;
        }

		public override string ToString ()
		{
            string s = molecule.species + ":";
            int i = 0;
            foreach (KeyValuePair<string,string> bindingSiteState in bindingSiteStates)
            {
                s += "[" + bindingSiteState.Key + "]=" + bindingSiteState.Value;
                if (i < bindingSiteStates.Count - 1)
                {
                    s += ",";
                }
                i++;
            }
            return s;
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

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
            return other.molecule == molecule && other.bindingSiteID == bindingSiteID;
        }

        public override string ToString ()
        {
            return "site " + bindingSiteID + " on molecule " + molecule.species;
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
                    s += moleculeStates[i].molecule.species;
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

		public override bool Equals (object obj)
		{
            ComplexState other = obj as ComplexState;
            if (other != null)
            {
                return Matches( other );
            }
            return false;
		}

		public bool Matches (ComplexState other)
        {
            foreach (MoleculeState moleculeState in moleculeStates)
            {
                bool stateFound = false;
                foreach (MoleculeState otherMoleculeState in other.moleculeStates)
                {
                    if (moleculeState.Matches( otherMoleculeState ))
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
            molecule.Init();
            bindingSiteStates = new Dictionary<string,string>();
            foreach (SiteState siteState in siteStates)
            {
                bindingSiteStates.Add( siteState.id, siteState.state );
            }
        }
        #endregion

        public bool Matches (MoleculeSimulator _moleculeSimulator)
        {
            if (_moleculeSimulator.species == molecule.species)
            {
                foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
                {
                    if (_moleculeSimulator.bindingSiteSimulators[siteState.Key].state != siteState.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool Matches (MoleculeState other)
        {
            if (other.molecule.species == molecule.species)
            {
                foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
                {
                    if (!other.bindingSiteStates.ContainsKey(siteState.Key) || other.bindingSiteStates[siteState.Key] != siteState.Value)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
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
    }

    //for prototyping in inspector without writing custom property drawer etc
    [System.Serializable]
    public class SiteState
    {
        public string id;
        public string state;
    }
}

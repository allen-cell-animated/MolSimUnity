using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class MoleculeSnapshot
    {
        [SerializeField] MoleculeDef _moleculeDef;
        public MoleculeDef moleculeDef 
        {
            get 
            {
                return _moleculeDef;
            }
        }

        public Dictionary<string,string> bindingSiteStates;

        #region for prototyping in inspector without writing custom property drawer etc
        [SerializeField] SiteState[] siteStates;

        public void Init ()
        {
            moleculeDef.Init();
            InitSites();
        }

        public void InitSites ()
        {
            bindingSiteStates = new Dictionary<string,string>();
            foreach (SiteState siteState in siteStates)
            {
                bindingSiteStates.Add( siteState.id, siteState.state );
            }
        }
        #endregion

        public MoleculeSnapshot (MoleculeDef theMoleculeDef, Dictionary<string,string> _bindingSiteStates)
        {
            _moleculeDef = theMoleculeDef;
            moleculeDef.Init();

            bindingSiteStates = new Dictionary<string, string>( _bindingSiteStates );
        }

        public bool IsSatisfiedBy (MoleculeSnapshot other)
        {
            if (other.moleculeDef.species != moleculeDef.species)
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

        public bool IsSatisfiedBy (Molecule _molecule)
        {
            if (_molecule.species == moleculeDef.species)
            {
                foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
                {
                    if (siteState.Value.Contains( "!" ))
                    {
                        if (_molecule.bindingSites[siteState.Key].boundSite == null)
                        {
                            return false;
                        }
                    }
                    else 
                    {
                        if (_molecule.bindingSites[siteState.Key].state != siteState.Value)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        public bool ContainsBindingSite (string bindingSiteID)
        {
            return bindingSiteStates.ContainsKey( bindingSiteID );
        }

        public override bool Equals (object obj)
        {
            MoleculeSnapshot other = obj as MoleculeSnapshot;
            if (other != null)
            {
                return other.moleculeDef.species == moleculeDef.species;
            }
            return false;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                return 16777619 * ((moleculeDef == null || moleculeDef.species == null) ? 1 : moleculeDef.species.GetHashCode());
            }
        }

		public override string ToString ()
		{
            string s = moleculeDef.species + ":";
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

    [System.Serializable]
    public class MoleculeBindingSite
    {
        public MoleculeDef moleculeDef;
        public string bindingSiteID;

        public MoleculeBindingSite (MoleculeDef _moleculeDef, string _bindingSiteID)
        {
            moleculeDef = _moleculeDef;
            bindingSiteID = _bindingSiteID;
        }

        public bool Matches (BindingSite bindingSite)
        {
            return bindingSite.moleculeDef.species == moleculeDef.species && bindingSite.id == bindingSiteID;
        }

        public override string ToString ()
        {
            return "site " + bindingSiteID + " on molecule " + moleculeDef.species;
        }
    }
}

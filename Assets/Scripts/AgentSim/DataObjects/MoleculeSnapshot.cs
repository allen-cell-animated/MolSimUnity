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

        [SerializeField] SiteState[] siteStates;
        public Dictionary<string,List<SiteState>> bindingSiteStates;


        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            moleculeDef.Init();
            InitSiteStates();
        }

        public void InitSiteStates ()
        {
            bindingSiteStates = new Dictionary<string,List<SiteState>>();
            foreach (SiteState site in siteStates)
            {
                if (!bindingSiteStates.ContainsKey( site.id ))
                {
                    bindingSiteStates.Add( site.id, new List<SiteState>() );
                }
                bindingSiteStates[site.id].Add( site );
            }
        }
        #endregion

        public MoleculeSnapshot (MoleculeDef theMoleculeDef, SiteState[] _siteStates)
        {
            _moleculeDef = theMoleculeDef;
            moleculeDef.Init();

            siteStates = _siteStates;
            InitSiteStates();
        }

        public virtual void SetStateOfMolecule (Molecule molecule)
        {
            //TODO match BindingSites to SiteStates
        }

        public bool IsSatisfiedBy (MoleculeSnapshot other)
        {
            if (!other.moleculeDef.Equals( moleculeDef ))
            {
                //Debug.Log( "molecules don't match" );
                return false;
            }

            string state;
            Dictionary<string,int> sitesInStateThis = new Dictionary<string,int>();
            Dictionary<string,int> sitesInStateOther = new Dictionary<string,int>();
            foreach (string bindingSiteID in bindingSiteStates.Keys)
            {
                if (!other.bindingSiteStates.ContainsKey( bindingSiteID )) //does this type of site exist in the other?
                {
                    //Debug.Log( "site " + bindingSiteID + " doesn't exist in other" );
                    return false;
                }

                foreach (SiteState site in bindingSiteStates[bindingSiteID]) //how many of our sites are in each state?
                {
                    state = site.state.Contains( "!" ) ? "!" : site.state;
                    if (!sitesInStateThis.ContainsKey( state ))
                    {
                        sitesInStateThis[state] = 1;
                    }
                    else
                    {
                        sitesInStateThis[state]++;
                    }
                }
                foreach (SiteState otherSite in other.bindingSiteStates[bindingSiteID]) //how many of the other's sites are in each state?
                {
                    state = otherSite.state.Contains( "!" ) ? "!" : otherSite.state;
                    if (!sitesInStateOther.ContainsKey( state ))
                    {
                        sitesInStateOther[state] = 1;
                    }
                    else
                    {
                        sitesInStateOther[state]++;
                    }
                }

                foreach (string s in sitesInStateThis.Keys) //does the other at least have as many sites in each state as we do?
                {
                    if (!sitesInStateOther.ContainsKey( s ))
                    {
                        //Debug.Log( "there are no " + bindingSiteID + " sites in state " + s );
                        return false;
                    }
                    if (sitesInStateOther[s] < sitesInStateThis[s])
                    {
                        //Debug.Log( "number of " + bindingSiteID + " sites in state " + s + " is " + sitesInStateOther[s] + " when it should be at least " + sitesInStateThis[s] );
                        return false;
                    }
                }

                sitesInStateThis.Clear();
                sitesInStateOther.Clear();
            }
            return true;
        }

        public bool IsSatisfiedBy (Molecule _molecule)
        {
            if (!_molecule.definition.Equals( moleculeDef ))
            {
                //Debug.Log( "molecules don't match" );
                return false;
            }

            string state;
            Dictionary<string,int> sitesInStateThis = new Dictionary<string,int>();
            Dictionary<string,int> sitesInStateOther = new Dictionary<string,int>();
            foreach (string bindingSiteID in bindingSiteStates.Keys)
            {
                if (!_molecule.bindingSites.ContainsKey( bindingSiteID )) //does this type of site exist in the other?
                {
                    //Debug.Log( "site " + bindingSiteID + " doesn't exist in other" );
                    return false;
                }

                foreach (SiteState site in bindingSiteStates[bindingSiteID]) //how many of our sites are in each state?
                {
                    state = site.state.Contains( "!" ) ? "!" : site.state;
                    if (!sitesInStateThis.ContainsKey( state ))
                    {
                        sitesInStateThis[state] = 1;
                    }
                    else
                    {
                        sitesInStateThis[state]++;
                    }
                }
                foreach (BindingSite otherSite in _molecule.bindingSites[bindingSiteID]) //how many of the other's sites are in each state?
                {
                    state = otherSite.state.Contains( "!" ) ? "!" : otherSite.state;
                    if (!sitesInStateOther.ContainsKey( state ))
                    {
                        sitesInStateOther[state] = 1;
                    }
                    else
                    {
                        sitesInStateOther[state]++;
                    }
                }

                foreach (string s in sitesInStateThis.Keys) //does the other at least have as many sites in each state as we do?
                {
                    if (!sitesInStateOther.ContainsKey( s ))
                    {
                        //Debug.Log( "there are no " + bindingSiteID + " sites in state " + s );
                        return false;
                    }
                    if (sitesInStateOther[s] < sitesInStateThis[s])
                    {
                        //Debug.Log( "number of " + bindingSiteID + " sites in state " + s + " is " + sitesInStateOther[s] + " when it should be at least " + sitesInStateThis[s] );
                        return false;
                    }
                }

                sitesInStateThis.Clear();
                sitesInStateOther.Clear();
            }
            return true;
        }

        public virtual bool ContainsBindingSite (string bindingSiteID)
        {
            return bindingSiteStates.ContainsKey( bindingSiteID );
        }

        public override bool Equals (object obj)
        {
            MoleculeSnapshot other = obj as MoleculeSnapshot;
            if (other != null)
            {
                return other.moleculeDef.Equals( moleculeDef );
            }
            return false;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                return 16777619 * ((moleculeDef == null) ? 1 : moleculeDef.GetHashCode());
            }
        }

		public override string ToString ()
		{
            string s = moleculeDef.species + ":";
            int i = 0;
            foreach (SiteState site in siteStates)
            {
                s += site;
                if (i < siteStates.Length - 1)
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

        public SiteState (string _id, string _state)
        {
            id = _id;
            state = _state;
        }

        public bool MatchesIDAndState (SiteState other)
        {
            return other.id == id && other.state == state;
        }

        public bool MatchesIDAndState (BindingSite bindingSite)
        {
            return bindingSite.definition.id == id && bindingSite.state == state;
        }

		public override string ToString()
		{
            return "(" + id + " state = " + state + ")";
		}
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

        public bool IsSatisfiedBy (BindingSite bindingSite)
        {
            return bindingSite.moleculeDef.Equals( moleculeDef ) && bindingSite.id == bindingSiteID;
        }

        public override string ToString ()
        {
            return "site " + bindingSiteID + " on " + moleculeDef;
        }
    }
}

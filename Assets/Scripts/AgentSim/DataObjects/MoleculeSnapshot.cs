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

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            moleculeDef.Init();
        }
        #endregion

        public MoleculeSnapshot (MoleculeDef theMoleculeDef, SiteState[] _siteStates)
        {
            _moleculeDef = theMoleculeDef;
            moleculeDef.Init();

            siteStates = _siteStates;
        }

        public bool IsSatisfiedBy (MoleculeSnapshot other)
        {
            if (!other.moleculeDef.Equals( moleculeDef ))
            {
                Debug.Log( "molecules are not the same" );
                return false;
            }

            //find all sites that satisfy each other
            Dictionary<SiteState,List<SiteState>> foundSiteStatesThis = new Dictionary<SiteState,List<SiteState>>();
            foreach (SiteState site in siteStates)
            {
                if (!foundSiteStatesThis.ContainsKey( site ))
                {
                    foundSiteStatesThis.Add( site, new List<SiteState>() );
                }
                foreach (SiteState otherSite in other.siteStates)
                {
                    if (site.IsEquivalentTo( otherSite ))
                    {
                        foundSiteStatesThis[site].Add( otherSite );
                    }
                }
                if (foundSiteStatesThis[site].Count < 1)
                {
                    Debug.Log( site + " not found" );
                    return false; //the other doesn't have any sites that satisfy one of our sites
                }
            }
            foreach (KeyValuePair<SiteState,List<SiteState>> matchToThis in foundSiteStatesThis)
            {
                foreach (SiteState match in matchToThis.Value)
                {
                    Debug.Log( "this: " + matchToThis.Key + " --> " + match );
                }
            }
            Dictionary<SiteState,List<SiteState>> foundSiteStatesOther = new Dictionary<SiteState,List<SiteState>>();
            foreach (SiteState otherSite in other.siteStates)
            {
                if (!foundSiteStatesOther.ContainsKey( otherSite ))
                {
                    foundSiteStatesOther.Add( otherSite, new List<SiteState>() );
                }
                foreach (SiteState site in siteStates)
                {
                    if (otherSite.IsEquivalentTo( site ))
                    {
                        foundSiteStatesOther[otherSite].Add( site );
                    }
                }
                //we don't care if the other has a site that we don't satisfy since this is a one-way check
            }
            foreach (KeyValuePair<SiteState,List<SiteState>> matchToOther in foundSiteStatesOther)
            {
                foreach (SiteState match in matchToOther.Value)
                {
                    Debug.Log( "other: " + matchToOther.Key + " --> " + match );
                }
            }

            //match pairs of sites
            List<SiteState> sortedSitesThis = new List<SiteState>();
            List<SiteState> sortedSitesOther = new List<SiteState>();

            //make unique matches
            foreach (SiteState site in foundSiteStatesThis.Keys)
            {
                if (foundSiteStatesThis[site].Count == 1 && !sortedSitesOther.Contains( foundSiteStatesThis[site][0] ))
                {
                    Debug.Log( "unique this: " + site + " --> " + foundSiteStatesThis[site][0] );
                    sortedSitesThis.Add( site );
                    sortedSitesOther.Add( foundSiteStatesThis[site][0] );

                    foundSiteStatesOther.Remove( foundSiteStatesThis[site][0] );
                }
            }
            foreach (SiteState otherSite in foundSiteStatesOther.Keys)
            {
                if (foundSiteStatesOther[otherSite].Count == 1 && !sortedSitesThis.Contains( foundSiteStatesOther[otherSite][0] ))
                {
                    Debug.Log( "unique other: " + otherSite + " --> " + foundSiteStatesOther[otherSite][0] );
                    sortedSitesThis.Add( foundSiteStatesOther[otherSite][0] );
                    sortedSitesOther.Add( otherSite );

                    foundSiteStatesThis.Remove( foundSiteStatesOther[otherSite][0] );
                }
            }

            //match everything else
            foreach (SiteState site in foundSiteStatesThis.Keys)
            {
                if (!sortedSitesThis.Contains( site ))
                {
                    bool foundMatch = false;
                    foreach (SiteState otherSite in foundSiteStatesThis[site])
                    {
                        if (!sortedSitesOther.Contains( otherSite ))
                        {
                            Debug.Log( "everything else: " + site + " --> " + otherSite );
                            sortedSitesThis.Add( site );
                            sortedSitesOther.Add( otherSite );
                            foundMatch = true;
                            break;
                        }
                    }
                    if (!foundMatch)
                    {
                        Debug.Log( site + " NOT FOUND!!!" );
                        return false; //this site's match is matched to something else now so it no longer has a match
                    }
                }
            }

            //compare states
            for (int i = 0; i < sortedSitesThis.Count; i++)
            {
                Debug.Log( "COMPARING: " + sortedSitesThis[i] + " --> " + sortedSitesOther[i] );
                if (sortedSitesThis[i].state.Contains( "!" ))
                {
                    if (!sortedSitesOther[i].state.Contains( "!" ))
                    {
                        return false;
                    }
                }
                else
                {
                    if (sortedSitesOther[i].state != sortedSitesThis[i].state)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool IsSatisfiedBy (Molecule _molecule)
        {
            // TODO
            //if (_molecule.definition.Equals( moleculeDef ))
            //{
            //    foreach (KeyValuePair<string,string> siteState in bindingSiteStates)
            //    {
            //        if (siteState.Value.Contains( "!" ))
            //        {
            //            if (_molecule.bindingSites[siteState.Key].boundSite == null)
            //            {
            //                return false;
            //            }
            //        }
            //        else 
            //        {
            //            if (_molecule.bindingSites[siteState.Key].state != siteState.Value)
            //            {
            //                return false;
            //            }
            //        }
            //    }
            //    return true;
            //}
            return false;
        }

        public bool ContainsBindingSite (string bindingSiteID)
        {
            foreach (SiteState site in siteStates)
            {
                if (site.siteRef.id == bindingSiteID)
                {
                    return true;
                }
            }
            return false;
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
                s += "[" + site.siteRef.id + ":" + site.siteRef.index + "]=" + site.state;
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
        public BindingSiteRef siteRef;
        public string state;

        public SiteState (BindingSiteRef _siteRef, string _state)
        {
            siteRef = _siteRef;
            state = _state;
        }

        public bool IsEquivalentTo (SiteState other)
        {
            return siteRef.IsEquivalentTo( other.siteRef ) && other.state == state;
        }

		public override string ToString()
		{
            return "(" + siteRef + " state = " + state + ")";
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

        public bool Matches (BindingSite bindingSite)
        {
            return bindingSite.moleculeDef.Equals( moleculeDef ) && bindingSite.id == bindingSiteID;
        }

        public override string ToString ()
        {
            return "site " + bindingSiteID + " on molecule " + moleculeDef.species;
        }
    }
}

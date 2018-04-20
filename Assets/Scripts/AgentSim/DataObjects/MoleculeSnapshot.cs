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
                    if (site.siteRef.IsSatisfiedBy( otherSite.siteRef ))
                    {
                        foundSiteStatesThis[site].Add( otherSite );
                    }
                }
                if (foundSiteStatesThis[site].Count < 1)
                {
                    return false; //the other doesn't have any sites that satisfy one of our sites
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
                    if (otherSite.siteRef.IsSatisfiedBy( site.siteRef ))
                    {
                        foundSiteStatesOther[otherSite].Add( site );
                    }
                }
                //we don't care if the other has a site that we don't satisfy since this is a one-way check
            }

            //match pairs of sites
            List<SiteState> sortedSitesThis = new List<SiteState>();
            List<SiteState> sortedSitesOther = new List<SiteState>();

            //make unique matches
            foreach (SiteState site in foundSiteStatesThis.Keys)
            {
                if (foundSiteStatesThis[site].Count == 1)
                {
                    sortedSitesThis.Add( site );
                    sortedSitesOther.Add( foundSiteStatesThis[site][0] );

                    foundSiteStatesOther.Remove( foundSiteStatesThis[site][0] );
                }
            }
            foreach (SiteState otherSite in foundSiteStatesOther.Keys)
            {
                if (foundSiteStatesOther[otherSite].Count == 1)
                {
                    sortedSitesThis.Add( foundSiteStatesOther[otherSite][0] );
                    sortedSitesOther.Add( otherSite );

                    foundSiteStatesThis.Remove( foundSiteStatesOther[otherSite][0] );
                }
            }

            //match everything else
            foreach (SiteState site in foundSiteStatesThis.Keys)
            {
                if (foundSiteStatesThis[site].Count > 1)
                {
                    foreach (SiteState otherSite in foundSiteStatesThis[site])
                    {
                        if (foundSiteStatesOther.ContainsKey( otherSite ))
                        {
                            sortedSitesThis.Add( site );
                            sortedSitesOther.Add( otherSite );
                        }
                    }
                }
            }
            //TODO cover more cases

            //compare states
            for (int i = 0; i < sortedSitesThis.Count; i++)
            {
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

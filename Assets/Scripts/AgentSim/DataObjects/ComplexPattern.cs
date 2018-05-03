using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class ComplexPattern
    {
        [SerializeField] MoleculePattern[] _molecules;
        Dictionary<string,List<MoleculePattern>> _moleculePatterns;
        public Dictionary<string,List<MoleculePattern>> moleculePatterns
        {
            get
            {
                return _moleculePatterns;
            }
        }

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            InitMoleculePatterns();
        }

        protected void InitMoleculePatterns ()
        {
            _moleculePatterns = new Dictionary<string,List<MoleculePattern>>();
            foreach (MoleculePattern moleculePattern in _molecules)
            {
                if (!moleculePatterns.ContainsKey( moleculePattern.moleculeDef.moleculeName ))
                {
                    moleculePatterns.Add( moleculePattern.moleculeDef.moleculeName, new List<MoleculePattern>() );
                }
                moleculePatterns[moleculePattern.moleculeDef.moleculeName].Add( moleculePattern );
                moleculePattern.Init();
            }
        }
        #endregion

        public ComplexPattern (Dictionary<string,List<Molecule>> molecules)
        {
            _moleculePatterns = new Dictionary<string,List<MoleculePattern>>();
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    if (!moleculePatterns.ContainsKey( molecule.definition.moleculeName ))
                    {
                        moleculePatterns.Add( molecule.definition.moleculeName, new List<MoleculePattern>() );
                    }
                    moleculePatterns[molecule.definition.moleculeName].Add( new MoleculePattern( molecule ) );
                }
            }
        }

        public ComplexPattern (MoleculePattern[] molecules)
        {
            _molecules = molecules;
            InitMoleculePatterns();
        }

        public virtual void SetStateOfComplex (Dictionary<string,List<Molecule>> molecules)
        {
            List<Molecule> matchedMolecules = new List<Molecule>();
            foreach (string moleculeName in moleculePatterns.Keys)
            {
                if (molecules.ContainsKey( moleculeName ))
                {
                    foreach (MoleculePattern moleculePattern in moleculePatterns[moleculeName])
                    {
                        foreach (Molecule molecule in molecules[moleculeName])
                        {
                            if (!matchedMolecules.Contains( molecule ) && moleculePattern.MatchesID( molecule ))
                            {
                                moleculePattern.SetStateOfMoleculeComponents( molecule );
                                matchedMolecules.Add( molecule );
                                break;
                            }
                        }
                    }
                }
            }
        }

        public override bool Equals (object obj)
        {
            ComplexPattern other = obj as ComplexPattern;
            if (other != null)
            {
                return Matches( other );
            }
            Dictionary<string,List<Molecule>> molecules = obj as Dictionary<string,List<Molecule>>;
            if (molecules != null)
            {
                return Matches( molecules );
            }
            return false;
        }

        public bool Matches (ComplexPattern other)
        {
            Dictionary<MoleculePattern,int> thisComponentsInState = new Dictionary<MoleculePattern,int>();
            Dictionary<MoleculePattern,int> otherComponentsInState = new Dictionary<MoleculePattern,int>();
            foreach (string moleculeName in moleculePatterns.Keys)
            {
                if (!other.moleculePatterns.ContainsKey( moleculeName )) //does this type of molecule exist in the other?
                {
                    //Debug.Log( "molecule " + moleculeName + " doesn't exist in other" );
                    return false;
                }

                foreach (MoleculePattern thisMoleculePattern in moleculePatterns[moleculeName]) //how many of our molecules are in each state?
                {
                    if (!thisComponentsInState.ContainsKey( thisMoleculePattern ))
                    {
                        thisComponentsInState[thisMoleculePattern] = 1;
                    }
                    else
                    {
                        thisComponentsInState[thisMoleculePattern]++;
                    }
                }
                foreach (MoleculePattern otherMoleculePattern in other.moleculePatterns[moleculeName]) //how many of the other's molecules are in each state?
                {
                    if (!otherComponentsInState.ContainsKey( otherMoleculePattern ))
                    {
                        otherComponentsInState[otherMoleculePattern] = 1;
                    }
                    else
                    {
                        otherComponentsInState[otherMoleculePattern]++;
                    }
                }

                foreach (MoleculePattern thisMoleculePattern in thisComponentsInState.Keys) //does the other at least have as many molecules in each state as we do?
                {
                    if (!otherComponentsInState.ContainsKey( thisMoleculePattern ))
                    {
                        //Debug.Log( "there are no " + moleculeName + " molecuels that match pattern " + thisMoleculePattern );
                        return false;
                    }
                    if (otherComponentsInState[thisMoleculePattern] < thisComponentsInState[thisMoleculePattern])
                    {
                        //Debug.Log( "number of " + moleculeName + " molecules that match pattern " + thisMoleculePattern + " is " 
                        //+ otherComponentsInState[thisMoleculePattern] + " when it should be at least " + thisComponentsInState[thisMoleculePattern] );
                        return false;
                    }
                }

                thisComponentsInState.Clear();
                otherComponentsInState.Clear();
            }
            return true;
        }

        public bool Matches (Dictionary<string,List<Molecule>> molecules)
        {
            MoleculePattern otherMoleculePattern;
            Dictionary<MoleculePattern,int> thisComponentsInState = new Dictionary<MoleculePattern,int>();
            Dictionary<MoleculePattern,int> otherComponentsInState = new Dictionary<MoleculePattern,int>();
            foreach (string moleculeName in moleculePatterns.Keys)
            {
                if (!molecules.ContainsKey( moleculeName )) //does this type of molecule exist in the other?
                {
                    //Debug.Log( "molecule " + moleculeName + " doesn't exist in molecules" );
                    return false;
                }

                foreach (MoleculePattern thisMoleculePattern in moleculePatterns[moleculeName]) //how many of our molecules are in each state?
                {
                    if (!thisComponentsInState.ContainsKey( thisMoleculePattern ))
                    {
                        thisComponentsInState[thisMoleculePattern] = 1;
                    }
                    else
                    {
                        thisComponentsInState[thisMoleculePattern]++;
                    }
                }
                foreach (Molecule otherMolecule in molecules[moleculeName]) //how many of the other's molecules are in each state?
                {
                    otherMoleculePattern = new MoleculePattern( otherMolecule );
                    if (!otherComponentsInState.ContainsKey( otherMoleculePattern ))
                    {
                        otherComponentsInState[otherMoleculePattern] = 1;
                    }
                    else
                    {
                        otherComponentsInState[otherMoleculePattern]++;
                    }
                }

                foreach (MoleculePattern thisMoleculePattern in thisComponentsInState.Keys) //does the other at least have as many molecules in each state as we do?
                {
                    if (!otherComponentsInState.ContainsKey( thisMoleculePattern ))
                    {
                        //Debug.Log( "there are no " + moleculeName + " molecuels that match pattern " + thisMoleculePattern );
                        return false;
                    }
                    if (otherComponentsInState[thisMoleculePattern] < thisComponentsInState[thisMoleculePattern])
                    {
                        //Debug.Log( "number of " + moleculeName + " molecules that match pattern " + thisMoleculePattern + " is " 
                        //+ otherComponentsInState[thisMoleculePattern] + " when it should be at least " + thisComponentsInState[thisMoleculePattern] );
                        return false;
                    }
                }

                thisComponentsInState.Clear();
                otherComponentsInState.Clear();
            }
            return true;
        }

        public override int GetHashCode ()
        {
            unchecked
            {
                int hash = 0;
                if (moleculePatterns != null)
                {
                    foreach (List<MoleculePattern> aTypeOfMoleculePattern in moleculePatterns.Values)
                    {
                        foreach (MoleculePattern moleculePattern in aTypeOfMoleculePattern)
                        {
                            hash += 7919 * (moleculePattern == null ? 1 : moleculePattern.GetHashCode());
                        }
                    }
                }
                return hash;
            }
        }

        public override string ToString ()
        {
            string s = "";
            int i = 0;
            int n = GetNumberOfMolecules();
            foreach (List<MoleculePattern> aTypeOfMoleculePattern in moleculePatterns.Values)
            {
                foreach (MoleculePattern moleculePattern in aTypeOfMoleculePattern)
                {
                    s += moleculePattern;
                    if (i < n - 1)
                    {
                        s += " | ";
                    }
                    i++;
                }
            }
            return s;
        }

        public int GetNumberOfMolecules ()
        {
            int n = 0;
            foreach (List<MoleculePattern> aTypeOfMoleculePattern in moleculePatterns.Values)
            {
                n += aTypeOfMoleculePattern.Count;
            }
            return n;
        }
    }
}

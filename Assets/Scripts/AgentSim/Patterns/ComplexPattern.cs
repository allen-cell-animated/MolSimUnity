using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public void InitMoleculePatterns ()
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

        //this is a HACKY MESS!!
        public virtual void SetStateOfComplex (Dictionary<string,List<Molecule>> molecules)
        {
            //get molecules at the reaction center
            List<Molecule> reactedMolecules = new List<Molecule>();
            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    if (molecule.stateWasUpdated)
                    {
                        reactedMolecules.Add( molecule );
                    }
                }
            }

            //trace their bonds to find candidate molecules


            //match the candidates to the product states














            //UnityEngine.Profiling.Profiler.BeginSample("Set State");
            Dictionary<string,List<MoleculePattern>> unusedMoleculePatterns = new Dictionary<string,List<MoleculePattern>>();
            foreach (string moleculeName in moleculePatterns.Keys)
            {
                if (!molecules.ContainsKey( moleculeName ))
                {
                    continue;
                }
                foreach (MoleculePattern moleculePattern in moleculePatterns[moleculeName])
                {
                    if (!MoleculePatternWasUsed( moleculePattern, molecules[moleculeName] ))
                    {
                        if (!unusedMoleculePatterns.ContainsKey( moleculeName ))
                        {
                            unusedMoleculePatterns.Add( moleculeName, new List<MoleculePattern>() );
                        }
                        unusedMoleculePatterns[moleculeName].Add( moleculePattern );
                    }
                }
            }

            List<Molecule> matchedMolecules = new List<Molecule>();
            foreach (string moleculeName in unusedMoleculePatterns.Keys)
            {
                if (!molecules.ContainsKey( moleculeName ))
                {
                    continue;
                }
                foreach (MoleculePattern moleculePattern in unusedMoleculePatterns[moleculeName])
                {
                    foreach (Molecule molecule in molecules[moleculeName])
                    {
                        if (!molecule.stateWasUpdated)
                        {
                            if (!matchedMolecules.Contains( molecule ) && moleculePattern.MatchesID( molecule ))
                            {
                                moleculePattern.SetStateOfMolecule( molecule );
                                matchedMolecules.Add( molecule );
                                break;
                            }
                        }
                        else
                        {
                            matchedMolecules.Add( molecule );
                            molecule.stateWasUpdated = false;
                        }
                    }
                }
            }

            foreach (string moleculeName in molecules.Keys)
            {
                foreach (Molecule molecule in molecules[moleculeName])
                {
                    molecule.stateWasUpdated = false;
                }
            }
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        bool MoleculePatternWasUsed (MoleculePattern moleculePattern, List<Molecule> molecules)
        {
            foreach (Molecule molecule in molecules)
            {
                if (molecule.stateWasUpdated && moleculePattern.Matches( molecule ))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Matches (ComplexPattern other)
        {
            //UnityEngine.Profiling.Profiler.BeginSample("Comparisons");
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
                    MoleculePattern matchingMoleculePattern = null;
                    foreach (MoleculePattern otherMoleculePattern in otherComponentsInState.Keys)
                    {
                        if (thisMoleculePattern.Matches( otherMoleculePattern ))
                        {
                            matchingMoleculePattern = otherMoleculePattern;
                            break;
                        }
                    }
                    if (matchingMoleculePattern == null)
                    {
                        //Debug.Log( "there are no " + moleculeName + " molecules that match pattern " + thisMoleculePattern );
                        return false;
                    }
                    if (otherComponentsInState[matchingMoleculePattern] < thisComponentsInState[thisMoleculePattern])
                    {
                        //Debug.Log( "number of " + moleculeName + " molecules that match pattern " + thisMoleculePattern + " is " 
                                  //+ otherComponentsInState[matchingMoleculePattern] + " when it should be at least " + thisComponentsInState[thisMoleculePattern] );
                        return false;
                    }
                }

                thisComponentsInState.Clear();
                otherComponentsInState.Clear();
            }
            //UnityEngine.Profiling.Profiler.EndSample();
            return true;
        }

        public bool Matches (Dictionary<string,List<Molecule>> molecules)
        {
            //UnityEngine.Profiling.Profiler.BeginSample("Comparisons");
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
                    MoleculePattern otherMoleculePattern = new MoleculePattern( otherMolecule );
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
                    MoleculePattern matchingMoleculePattern = null;
                    foreach (MoleculePattern otherMoleculePattern in otherComponentsInState.Keys)
                    {
                        if (thisMoleculePattern.Matches( otherMoleculePattern ))
                        {
                            matchingMoleculePattern = otherMoleculePattern;
                            break;
                        }
                    }
                    if (matchingMoleculePattern == null)
                    {
                        //Debug.Log( "there are no " + moleculeName + " molecules that match pattern " + thisMoleculePattern );
                        return false;
                    }
                    if (otherComponentsInState[matchingMoleculePattern] < thisComponentsInState[thisMoleculePattern])
                    {
                        //Debug.Log( "number of " + moleculeName + " molecules that match pattern " + thisMoleculePattern + " is " 
                        //+ otherComponentsInState[matchingMoleculePattern] + " when it should be at least " + thisComponentsInState[thisMoleculePattern] );
                        return false;
                    }
                }

                thisComponentsInState.Clear();
                otherComponentsInState.Clear();
            }
            //UnityEngine.Profiling.Profiler.EndSample();
            return true;
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

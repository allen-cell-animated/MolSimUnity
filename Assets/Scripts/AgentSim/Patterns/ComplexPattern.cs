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

        public virtual void SetStateOfComplex (Molecule molecule1, MoleculePattern productMolecule1, Molecule molecule2, MoleculePattern productMolecule2)
        {
            //get references to the reaction center molecules in this complex pattern
            MoleculePattern productMoleculeRef1 = null, productMoleculeRef2 = null;
            foreach (string moleculeName in moleculePatterns.Keys)
            {
                foreach (MoleculePattern moleculePattern in moleculePatterns[moleculeName])
                {
                    if (productMolecule1.Matches( moleculePattern ))
                    {
                        productMoleculeRef1 = moleculePattern;
                    }
                    else if (productMolecule2.Matches( moleculePattern ))
                    {
                        productMoleculeRef2 = moleculePattern;
                    }
                }
            }

            //set states of attached molecules
            SetProductStatesOfAttachedMolecules( molecule1, productMoleculeRef1 );
            SetProductStatesOfAttachedMolecules( molecule2, productMoleculeRef2 );
        }

        void SetProductStatesOfAttachedMolecules (Molecule molecule, MoleculePattern productMoleculeRef)
        {
            Dictionary<string,Bond> bonds = GetBonds( productMoleculeRef );
            foreach (string componentName in molecule.components.Keys)
            {
                foreach (MoleculeComponent component in molecule.components[componentName])
                {
                    if (component.boundComponent != null && !component.boundComponent.molecule.stateWasUpdated && bonds.ContainsKey( component.lastBondName ))
                    {
                        component.boundComponent.SetToProductState( bonds[component.lastBondName].moleculePattern2, bonds[component.lastBondName].componentPattern2 );
                    }
                }
            }
        }

        Dictionary<string,Bond> GetBonds (MoleculePattern moleculePattern)
        {
            Bond bond;
            Dictionary<string,Bond> bonds = new Dictionary<string,Bond>();
            foreach (string componentName in moleculePattern.componentPatterns.Keys)
            {
                foreach (ComponentPattern componentPattern in moleculePattern.componentPatterns[componentName])
                {
                    if (!componentPattern.bound || componentPattern.bondName.Contains( "+" ) || bonds.ContainsKey( componentPattern.bondName ))
                    {
                        continue;
                    }

                    bond = GetBondForComponent( moleculePattern, componentPattern );
                    if (bond != null)
                    {
                        bonds.Add( componentPattern.bondName, bond );
                    }
                }
            }
            return bonds;
        }

        Bond GetBondForComponent (MoleculePattern moleculePattern, ComponentPattern componentPattern)
        {
            foreach (string moleculeName in moleculePatterns.Keys)
            {
                foreach (MoleculePattern otherMoleculePattern in moleculePatterns[moleculeName])
                {
                    if (otherMoleculePattern == moleculePattern)
                    {
                        continue;
                    }
                    foreach (string otherComponentName in otherMoleculePattern.componentPatterns.Keys)
                    {
                        foreach (ComponentPattern otherComponentPattern in otherMoleculePattern.componentPatterns[otherComponentName])
                        {
                            if (otherComponentPattern.bound && otherComponentPattern.bondName == componentPattern.bondName)
                            {
                                return new Bond( moleculePattern, componentPattern, otherMoleculePattern, otherComponentPattern );
                            }
                        }
                    }
                }
            }
            return null;
        }

        //this is a HACKY MESS!!
        public virtual void SetStateOfComplex (Dictionary<string,List<Molecule>> molecules)
        {
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
                        }
                    }
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

    public class Bond
    {
        public MoleculePattern moleculePattern1;
        public ComponentPattern componentPattern1;
        public MoleculePattern moleculePattern2;
        public ComponentPattern componentPattern2;

        public Bond (MoleculePattern _moleculePattern1, ComponentPattern _componentPattern1, MoleculePattern _moleculePattern2, ComponentPattern _componentPattern2)
        {
            moleculePattern1 = _moleculePattern1;
            componentPattern1 = _componentPattern1;
            moleculePattern2 = _moleculePattern2;
            componentPattern2 = _componentPattern2;
        }
    }
}

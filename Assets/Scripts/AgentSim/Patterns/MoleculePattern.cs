using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    [System.Serializable]
    public class MoleculePattern
    {
        [SerializeField] MoleculeDef _moleculeDef;
        public MoleculeDef moleculeDef 
        {
            get 
            {
                return _moleculeDef;
            }
        }

        [SerializeField] ComponentPattern[] _components;
        Dictionary<string,List<ComponentPattern>> _componentPatterns;
        public Dictionary<string,List<ComponentPattern>> componentPatterns
        {
            get
            {
                return _componentPatterns;
            }
        }

        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            moleculeDef.Init();
            InitComponentPatterns();
        }

        public void InitComponentPatterns ()
        {
            _componentPatterns = new Dictionary<string,List<ComponentPattern>>();
            foreach (ComponentPattern componentPattern in _components)
            {
                if (!componentPatterns.ContainsKey( componentPattern.componentName ))
                {
                    componentPatterns.Add( componentPattern.componentName, new List<ComponentPattern>() );
                }
                componentPatterns[componentPattern.componentName].Add( componentPattern );
            }
        }
        #endregion

        public MoleculePattern (MoleculeDef theMoleculeDef, ComponentPattern[] components)
        {
            _moleculeDef = theMoleculeDef;
            moleculeDef.Init();

            _components = components;
            InitComponentPatterns();
        }

        public MoleculePattern (Molecule molecule)
        {
            _moleculeDef = molecule.definition;
            if (molecule.definition.componentDefs == null)
            {
                //since this will mostly be used at runtime, the moleculeDef is probably already initialized
                //and this step will go away anyway once BNGL files are parsed to unity objects correctly
                moleculeDef.Init();
            }

            _componentPatterns = new Dictionary<string,List<ComponentPattern>>();
            foreach (string componentName in molecule.components.Keys)
            {
                if (!componentPatterns.ContainsKey( componentName ))
                {
                    componentPatterns.Add( componentName, new List<ComponentPattern>() );
                }
                foreach (MoleculeComponent component in molecule.components[componentName])
                {
                    componentPatterns[componentName].Add( new ComponentPattern( component ) );
                }
            }
        }

        public virtual void SetStateOfMolecule (Molecule molecule)
        {
            List<MoleculeComponent> matchedComponents = new List<MoleculeComponent>();
            foreach (string componentName in componentPatterns.Keys)
            {
                if (molecule.components.ContainsKey( componentName ))
                {
                    foreach (ComponentPattern componentPattern in componentPatterns[componentName])
                    {
                        foreach (MoleculeComponent moleculeComponent in molecule.components[componentName])
                        {
                            if (!moleculeComponent.stateWasUpdated)
                            {
                                if (!matchedComponents.Contains( moleculeComponent ) && componentPattern.MatchesID( moleculeComponent ))
                                {
                                    componentPattern.SetStateOfComponent( moleculeComponent );
                                    matchedComponents.Add( moleculeComponent );
                                    break;
                                }
                            }
                            else
                            {
                                matchedComponents.Add( moleculeComponent );
                                moleculeComponent.stateWasUpdated = false;
                            }
                        }
                    }
                }
            }
        }

        public virtual bool ContainsComponent (string componentName)
        {
            return componentPatterns.ContainsKey( componentName );
        }

        public bool MatchesID (MoleculePattern other)
        {
            return other.moleculeDef.Equals( moleculeDef );
        }

        public bool MatchesID (Molecule molecule)
        {
            return molecule.definition.Equals( moleculeDef );
        }

        public override bool Equals (object obj)
        {
            UnityEngine.Profiling.Profiler.BeginSample("Comparisons");
            MoleculePattern other = obj as MoleculePattern;
            if (other != null)
            {
                return Matches( other );
            }
            Molecule molecule = obj as Molecule;
            if (molecule != null)
            {
                return Matches( molecule );
            }
            UnityEngine.Profiling.Profiler.EndSample();
            return false;
        }

        public bool Matches (MoleculePattern other)
        {
            if (!other.moleculeDef.Equals( moleculeDef ))
            {
                //Debug.Log( "molecules don't match" );
                return false;
            }

            Dictionary<ComponentPattern,int> thisComponentsInState = new Dictionary<ComponentPattern,int>();
            Dictionary<ComponentPattern,int> otherComponentsInState = new Dictionary<ComponentPattern,int>();
            foreach (string componentName in componentPatterns.Keys)
            {
                if (!other.componentPatterns.ContainsKey( componentName )) //does this type of component exist in the other?
                {
                    //Debug.Log( "component " + componentName + " doesn't exist in other" );
                    return false;
                }

                foreach (ComponentPattern thisComponentPattern in componentPatterns[componentName]) //how many of our components are in each state?
                {
                    if (!thisComponentsInState.ContainsKey( thisComponentPattern ))
                    {
                        thisComponentsInState[thisComponentPattern] = 1;
                    }
                    else
                    {
                        thisComponentsInState[thisComponentPattern]++;
                    }
                }
                foreach (ComponentPattern otherComponentPattern in other.componentPatterns[componentName]) //how many of the other's components are in each state?
                {
                    if (!otherComponentsInState.ContainsKey( otherComponentPattern ))
                    {
                        otherComponentsInState[otherComponentPattern] = 1;
                    }
                    else
                    {
                        otherComponentsInState[otherComponentPattern]++;
                    }
                }

                foreach (ComponentPattern thisComponentPattern in thisComponentsInState.Keys) //does the other at least have as many components in each state as we do?
                {
                    if (!otherComponentsInState.ContainsKey( thisComponentPattern ))
                    {
                        //Debug.Log( "there are no " + componentName + " components that match pattern " + thisComponentPattern );
                        return false;
                    }
                    if (otherComponentsInState[thisComponentPattern] < thisComponentsInState[thisComponentPattern])
                    {
                        //Debug.Log( "number of " + componentName + " components that match pattern " + thisComponentPattern + " is " 
                        //+ otherComponentsInState[thisComponentPattern] + " when it should be at least " + thisComponentsInState[thisComponentPattern] );
                        return false;
                    }
                }

                thisComponentsInState.Clear();
                otherComponentsInState.Clear();
            }
            return true;
        }

        public bool Matches (Molecule _molecule)
        {
            if (!_molecule.definition.Equals( moleculeDef ))
            {
                //Debug.Log( "molecules don't match" );
                return false;
            }

            ComponentPattern otherComponentPattern;
            Dictionary<ComponentPattern,int> thisComponentsInState = new Dictionary<ComponentPattern,int>();
            Dictionary<ComponentPattern,int> otherComponentsInState = new Dictionary<ComponentPattern,int>();
            foreach (string componentName in componentPatterns.Keys)
            {
                if (!_molecule.components.ContainsKey( componentName )) //does this type of component exist in the molecule?
                {
                    //Debug.Log( "component " + componentName + " doesn't exist in molecule" );
                    return false;
                }

                foreach (ComponentPattern thisComponentPattern in componentPatterns[componentName]) //how many of our components are in each state?
                {
                    if (!thisComponentsInState.ContainsKey( thisComponentPattern ))
                    {
                        thisComponentsInState[thisComponentPattern] = 1;
                    }
                    else
                    {
                        thisComponentsInState[thisComponentPattern]++;
                    }
                }
                foreach (MoleculeComponent otherComponent in _molecule.components[componentName]) //how many of the molecule's components are in each state?
                {
                    otherComponentPattern = new ComponentPattern( otherComponent );
                    if (!otherComponentsInState.ContainsKey( otherComponentPattern ))
                    {
                        otherComponentsInState[otherComponentPattern] = 1;
                    }
                    else
                    {
                        otherComponentsInState[otherComponentPattern]++;
                    }
                }

                foreach (ComponentPattern thisComponentPattern in thisComponentsInState.Keys) //does the molecule at least have as many components in each state as we do?
                {
                    if (!otherComponentsInState.ContainsKey( thisComponentPattern ))
                    {
                        //Debug.Log( "there are no " + componentName + " components that match pattern " + thisComponentPattern );
                        return false;
                    }
                    if (otherComponentsInState[thisComponentPattern] < thisComponentsInState[thisComponentPattern])
                    {
                        //Debug.Log( "number of " + componentName + " components that match pattern " + thisComponentPattern + " is " 
                        //+ otherComponentsInState[thisComponentPattern] + " when it should be at least " + thisComponentsInState[thisComponentPattern] );
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
                int hash = 16777619 * ((moleculeDef == null) ? 1 : moleculeDef.GetHashCode());
                foreach (List<ComponentPattern> aTypeOfComponentPattern in componentPatterns.Values)
                {
                    foreach (ComponentPattern componentPattern in aTypeOfComponentPattern)
                    {
                        hash += 1381 * (componentPattern == null ? 1 : componentPattern.GetHashCode());
                    }
                }
                return hash;
            }
        }

		public override string ToString ()
		{
            string s = moleculeDef.moleculeName + ":";
            int i = 0;
            int n = GetNumberOfComponents();
            foreach (List<ComponentPattern> aTypeOfComponentPattern in componentPatterns.Values)
            {
                foreach (ComponentPattern componentPattern in aTypeOfComponentPattern)
                {
                    s += componentPattern;
                    if (i < n - 1)
                    {
                        s += ",";
                    }
                    i++;
                }
            }
            return s;
		}

        int GetNumberOfComponents ()
        {
            int n = 0;
            foreach (List<ComponentPattern> aTypeOfComponentPattern in componentPatterns.Values)
            {
                n += aTypeOfComponentPattern.Count;
            }
            return n;
        }
    }
}

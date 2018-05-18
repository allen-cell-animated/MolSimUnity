using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public virtual void SetStateOfMolecule(Molecule molecule)
        {
            //UnityEngine.Profiling.Profiler.BeginSample("Set Component State");

            foreach (var entry in componentPatterns)
            {
                if (!molecule.components.ContainsKey(entry.Key)) return;
            }

            // matches each sub-entity to a list sub-entity patterns
            // after matching, changes are applied to the sub-entity
            // with the fewest matches, until all sub-entity patterns are used
            //
            // this prevent a sub-entity with many pattern matches from 'taking'
            // a sub-entity pattern from a sub-entity with fewer matches

            foreach (var entry in componentPatterns)
            {
                var candidates = molecule.components[entry.Key];
                var patterns = entry.Value;
                var matches = new Dictionary<MoleculeComponent, List<ComponentPattern>>();

                foreach (var candidate in candidates)
                {
                    foreach (var patternToMatch in patterns)
                    {
                        if (patternToMatch.Matches(candidate))
                        {
                            if (!matches.ContainsKey(candidate))
                            {
                                matches[candidate] = new List<ComponentPattern>();
                            }

                            matches[candidate].Add(patternToMatch);
                        }
                    }
                }

                List<ComponentPattern> usedPatterns = new List<ComponentPattern>();
                List<KeyValuePair<MoleculeComponent, List<ComponentPattern>>> matchList = matches.ToList();

                while (true)
                {
                    // find the component with the least number of component pattern matches
                    int minIndex = -1;
                    int minCount = int.MaxValue;
                    for (int i = 0; i < matchList.Count; ++i)
                    {
                        if (matchList[i].Value.Count == 0) continue;

                        if (matchList[i].Value.Count < minCount)
                        {
                            minIndex = i;
                            minCount = matchList[i].Value.Count;
                        }

                        if (matchList[i].Value.Count == 1)
                        {
                            break;
                        }
                    }

                    if (minIndex == -1)
                    {
                        break;
                    }

                    // apply the change to the component with the fewest options
                    ComponentPattern finalMatchPattern = matchList[minIndex].Value[0];
                    MoleculeComponent finalMatchComponent = matchList[minIndex].Key;
                    finalMatchPattern.SetStateOfComponent(finalMatchComponent);

                    // remove the matched component
                    matchList.RemoveAt(minIndex);
                    matchList.TrimExcess();

                    // remove the 'used' pattern
                    for (int i = 0; i < matchList.Count; ++i)
                    {
                        matchList[i].Value.Remove(finalMatchPattern);
                    }
                }
            }

            //UnityEngine.Profiling.Profiler.EndSample();
        }

        public bool MatchesID (Molecule molecule)
        {
            return molecule.definition.Equals( moleculeDef );
        }

        public override bool Equals (object obj)
        {
            //UnityEngine.Profiling.Profiler.BeginSample("Comparisons");
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
            //UnityEngine.Profiling.Profiler.EndSample();
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

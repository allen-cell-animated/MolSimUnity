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
        public Dictionary<string,List<ComponentPattern>> components;


        #region for prototyping in inspector without writing custom property drawer etc
        public void Init ()
        {
            moleculeDef.Init();
            InitSiteStates();
        }

        public void InitSiteStates ()
        {
            components = new Dictionary<string,List<ComponentPattern>>();
            foreach (ComponentPattern componentState in _components)
            {
                if (!components.ContainsKey( componentState.componentName ))
                {
                    components.Add( componentState.componentName, new List<ComponentPattern>() );
                }
                components[componentState.componentName].Add( componentState );
            }
        }
        #endregion

        public MoleculePattern (MoleculeDef theMoleculeDef, ComponentPattern[] _theComponents)
        {
            _moleculeDef = theMoleculeDef;
            moleculeDef.Init();

            _components = _theComponents;
            InitSiteStates();
        }

        public virtual void SetStateOfMoleculeComponents (Molecule molecule)
        {
            List<MoleculeComponent> matchedComponents = new List<MoleculeComponent>();
            foreach (string componentName in components.Keys)
            {
                if (molecule.components.ContainsKey( componentName ))
                {
                    foreach (ComponentPattern thisComponent in components[componentName])
                    {
                        foreach (MoleculeComponent moleculeComponent in molecule.components[componentName])
                        {
                            if (!matchedComponents.Contains( moleculeComponent ) && thisComponent.MatchesID( moleculeComponent ))
                            {
                                moleculeComponent.state = thisComponent.state;
                                matchedComponents.Add( moleculeComponent );
                                break;
                            }
                        }
                    }
                }
            }
        }

        public bool Matches (MoleculePattern other)
        {
            if (!other.moleculeDef.Equals( moleculeDef ))
            {
                //Debug.Log( "molecules don't match" );
                return false;
            }

            string state;
            Dictionary<string,int> componentsInStateThis = new Dictionary<string,int>();
            Dictionary<string,int> componentsInStateOther = new Dictionary<string,int>();
            foreach (string componentName in components.Keys)
            {
                if (!other.components.ContainsKey( componentName )) //does this type of component exist in the other?
                {
                    //Debug.Log( "component " + componentName + " doesn't exist in other" );
                    return false;
                }

                foreach (ComponentPattern component in components[componentName]) //how many of our components are in each state?
                {
                    state = component.state.Contains( "!" ) ? "!" : component.state;
                    if (!componentsInStateThis.ContainsKey( state ))
                    {
                        componentsInStateThis[state] = 1;
                    }
                    else
                    {
                        componentsInStateThis[state]++;
                    }
                }
                foreach (ComponentPattern otherComponent in other.components[componentName]) //how many of the other's components are in each state?
                {
                    state = otherComponent.state.Contains( "!" ) ? "!" : otherComponent.state;
                    if (!componentsInStateOther.ContainsKey( state ))
                    {
                        componentsInStateOther[state] = 1;
                    }
                    else
                    {
                        componentsInStateOther[state]++;
                    }
                }

                foreach (string s in componentsInStateThis.Keys) //does the other at least have as many components in each state as we do?
                {
                    if (!componentsInStateOther.ContainsKey( s ))
                    {
                        //Debug.Log( "there are no " + componentName + " components in state " + s );
                        return false;
                    }
                    if (componentsInStateOther[s] < componentsInStateThis[s])
                    {
                        //Debug.Log( "number of " + componentName + " components in state " + s + " is " + componentsInStateOther[s] + " when it should be at least " + componentsInStateThis[s] );
                        return false;
                    }
                }

                componentsInStateThis.Clear();
                componentsInStateOther.Clear();
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

            string state;
            Dictionary<string,int> componentsInStateThis = new Dictionary<string,int>();
            Dictionary<string,int> componentsInStateOther = new Dictionary<string,int>();
            foreach (string componentName in components.Keys)
            {
                if (!_molecule.components.ContainsKey( componentName )) //does this type of component exist in the molecule?
                {
                    //Debug.Log( "component " + componentName + " doesn't exist in molecule" );
                    return false;
                }

                foreach (ComponentPattern component in components[componentName]) //how many of our components are in each state?
                {
                    state = component.state.Contains( "!" ) ? "!" : component.state;
                    if (!componentsInStateThis.ContainsKey( state ))
                    {
                        componentsInStateThis[state] = 1;
                    }
                    else
                    {
                        componentsInStateThis[state]++;
                    }
                }
                foreach (MoleculeComponent otherComponent in _molecule.components[componentName]) //how many of the molecule's components are in each state?
                {
                    state = otherComponent.state.Contains( "!" ) ? "!" : otherComponent.state;
                    if (!componentsInStateOther.ContainsKey( state ))
                    {
                        componentsInStateOther[state] = 1;
                    }
                    else
                    {
                        componentsInStateOther[state]++;
                    }
                }

                foreach (string s in componentsInStateThis.Keys) //does the molecule at least have as many components in each state as we do?
                {
                    if (!componentsInStateOther.ContainsKey( s ))
                    {
                        //Debug.Log( "there are no " + componentName + " components in state " + s );
                        return false;
                    }
                    if (componentsInStateOther[s] < componentsInStateThis[s])
                    {
                        //Debug.Log( "number of " + componentName + " components in state " + s + " is " + componentsInStateOther[s] + " when it should be at least " + componentsInStateThis[s] );
                        return false;
                    }
                }

                componentsInStateThis.Clear();
                componentsInStateOther.Clear();
            }
            return true;
        }

        public virtual bool ContainsComponent (string componentName)
        {
            return components.ContainsKey( componentName );
        }

        public override bool Equals (object obj)
        {
            MoleculePattern other = obj as MoleculePattern;
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
            string s = moleculeDef.moleculeName + ":";
            int i = 0;
            int n = GetNumberOfComponents();
            foreach (List<ComponentPattern> aTypeOfComponent in components.Values)
            {
                foreach (ComponentPattern component in aTypeOfComponent)
                {
                    s += component;
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
            foreach (List<ComponentPattern> aTypeOfComponent in components.Values)
            {
                foreach (ComponentPattern component in aTypeOfComponent)
                {
                    n++;
                }
            }
            return n;
        }
    }

    //for prototyping in inspector without writing custom property drawer etc
    [System.Serializable]
    public class ComponentPattern : IComponent
    {
        [SerializeField] string _componentName;
        public string componentName
        {
            get
            {
                return _componentName;
            }
        }

        [SerializeField] string _state;
        public string state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public ComponentPattern (string _theComponentName, string _theState)
        {
            _componentName = _theComponentName;
            state = _theState;
        }

        public bool MatchesID (IComponent other)
        {
            return other.componentName == componentName;
        }

        public bool MatchesState (IComponent other)
        {
            return other.componentName == componentName && other.state == state;
        }

		public override string ToString()
		{
            return "(" + componentName + " state = " + state + ")";
		}
	}
}

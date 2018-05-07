using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule : MonoBehaviour 
    {
        public Complex complex;
        public MoleculeDef definition;
        public Dictionary<string,List<MoleculeComponent>> components = new Dictionary<string,List<MoleculeComponent>>();
        public float collisionRadius;
        public float interactionRadius;
        public bool couldReactOnCollision;
        public bool stateWasUpdated;

        Transform _theTransform;
        public Transform theTransform
        {
            get
            {
                if (_theTransform == null)
                {
                    _theTransform = transform;
                }
                return _theTransform;
            }
        }

        bool GetCouldReactOnCollision ()
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    if (component.couldReactOnCollision)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void Init (MoleculePattern moleculePattern, Complex _complex)
        {
            complex = _complex;
            definition = moleculePattern.moleculeDef;
            collisionRadius = interactionRadius = definition.radius;
            interactionRadius += 1f;
            CreateComponents( moleculePattern );
            SetColorForCurrentState();
        }

        protected virtual void CreateComponents (MoleculePattern moleculePattern)
        {
            foreach (List<ComponentDef> aTypeOfComponent in definition.componentDefs.Values)
            {
                foreach (ComponentDef componentDef in aTypeOfComponent)
                {
                    CreateComponent( componentDef );
                }
            }
            moleculePattern.SetStateOfMolecule( this );
        }

        protected virtual void CreateComponent (ComponentDef componentDef)
        {
            GameObject componentObject = new GameObject();
            componentObject.transform.SetParent( theTransform );
            componentDef.transformOnMolecule.Apply( theTransform, componentObject.transform );
            componentObject.name = name + "_" + componentDef.componentName;

            MoleculeComponent component = componentObject.AddComponent<MoleculeComponent>();
            component.Init( componentDef, this );

            if (!components.ContainsKey( componentDef.componentName ))
            {
                components.Add( componentDef.componentName, new List<MoleculeComponent>() );
            }
            components[componentDef.componentName].Add( component );
        }

        public virtual bool InteractWith (Molecule other)
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    if (component.couldReactOnCollision)
                    {
                        foreach (List<MoleculeComponent> aTypeOfComponentOther in other.components.Values)
                        {
                            foreach (MoleculeComponent otherComponent in aTypeOfComponentOther)
                            {
                                if (otherComponent.couldReactOnCollision && component.ReactWith( otherComponent ))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public virtual void MoveToComplex (Complex _complex)
        {
            complex.RemoveMolecule( this );
            complex = _complex;
            name = complex.name + "_" + definition.moleculeName;
            theTransform.SetParent( complex.theTransform );
        }

        public void SetToProductState (ReactionCenter reactionCenter)
        {
            reactionCenter.productMolecule.SetStateOfMolecule( this );
            stateWasUpdated = true;

            complex.SetToProductState( reactionCenter );
        }

        public virtual void UpdateReactions (BindReaction[] relevantBindReactions, 
                                             CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    component.UpdateReactions( relevantBindReactions, relevantCollisionFreeReactions );
                }
            }
            couldReactOnCollision = GetCouldReactOnCollision();
        }

        Material _material;
        Material material
        {
            get
            {
                if (_material == null)
                {
                    _material = GetComponent<MeshRenderer>().material;
                }
                return _material;
            }
        }

        public void SetColorForCurrentState ()
        {
            if (definition.colors != null)
            {
                foreach (MoleculePatternColor moleculeColor in definition.colors)
                {
                    if (moleculeColor.pattern.Matches( this ))
                    {
                        material.color = moleculeColor.color;
                        return;
                    }
                }
            }
        }

        Animator _animator;
        Animator animator
        {
            get
            {
                if (_animator == null && transform.childCount > 0)
                {
                    _animator = transform.GetChild( 0 ).GetComponent<Animator>();
                }
                return _animator;
            }
        }

        public void AnimateReaction ()
        {
            if (animator != null)
            {
                animator.SetTrigger( "React" );
            }
        }

        public override string ToString ()
        {
            string s = "Molecule " + name + " [" + definition.moleculeName + ":";
            int i = 0;
            int n = GetNumberOfComponents();
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    s += component;
                    if (i < n - 1)
                    {
                        s += ",";
                    }
                    i++;
                }
            }
            return s + "]";
        }

        int GetNumberOfComponents ()
        {
            int n = 0;
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                n += aTypeOfComponent.Count;
            }
            return n;
        }
	}
}
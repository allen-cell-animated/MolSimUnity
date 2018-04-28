using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule : MonoBehaviour 
    {
        public Complex complex;
        public MoleculeDef definition;
        public Dictionary<string,List<MoleculeComponent>> components = new Dictionary<string, List<MoleculeComponent>>();
        public float collisionRadius;
        public float interactionRadius;
        public bool couldReactOnCollision;

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

        public virtual void Init (MoleculePattern moleculePattern, Complex _complex, 
                                  BimolecularReaction[] relevantBimolecularReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            complex = _complex;
            definition = moleculePattern.moleculeDef;
            collisionRadius = interactionRadius = definition.radius;
            interactionRadius += 1f;
            CreateComponents( moleculePattern, relevantBimolecularReactions, relevantCollisionFreeReactions );
            couldReactOnCollision = GetCouldReactOnCollision();
            SetColorForCurrentState();
        }

        protected virtual void CreateComponents (MoleculePattern moleculePattern, BimolecularReaction[] relevantBimolecularReactions, 
                                                 CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (List<ComponentDef> aTypeOfComponent in definition.componentDefs.Values)
            {
                foreach (ComponentDef componentDef in aTypeOfComponent)
                {
                    CreateComponent( componentDef, relevantBimolecularReactions, relevantCollisionFreeReactions );
                }
            }
            moleculePattern.SetStateOfMolecule( this );
        }

        protected virtual void CreateComponent (ComponentDef componentDef, BimolecularReaction[] relevantBimolecularReactions, 
                                                CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            GameObject componentObject = new GameObject();
            componentObject.transform.SetParent( theTransform );
            componentDef.transformOnMolecule.Apply( theTransform, componentObject.transform );
            componentObject.name = name + "_" + componentDef.componentName;

            MoleculeComponent component = componentObject.AddComponent<MoleculeComponent>();
            component.Init( componentDef, relevantBimolecularReactions, relevantCollisionFreeReactions, this );

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

        public virtual void MoveToComplex (Complex _complex, BimolecularReaction[] relevantBimolecularReactions, 
                                           CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            complex.RemoveMolecule( this );
            complex = _complex;
            name = complex.name + "_" + definition.moleculeName;
            theTransform.SetParent( complex.theTransform );

            UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
        }

        public virtual void UpdateReactions (BimolecularReaction[] relevantBimolecularReactions, 
                                             CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (List<MoleculeComponent> aTypeOfComponent in components.Values)
            {
                foreach (MoleculeComponent component in aTypeOfComponent)
                {
                    component.UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
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
            return "Molecule " + name;
        }
	}
}
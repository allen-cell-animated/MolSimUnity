using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule : MonoBehaviour 
    {
        public Complex complex;
        public MoleculeDef definition;
        public Dictionary<BindingSiteRef,BindingSite> bindingSites = new Dictionary<BindingSiteRef,BindingSite>();
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
            foreach (BindingSite bindingSite in bindingSites.Values)
            {
                if (bindingSite.couldReactOnCollision)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void Init (MoleculeSnapshot moleculeSnapshot, Complex _complex, 
                                  BimolecularReaction[] relevantBimolecularReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            complex = _complex;
            definition = moleculeSnapshot.moleculeDef;
            collisionRadius = interactionRadius = definition.radius;
            interactionRadius += 1f;
            CreateBindingSites( moleculeSnapshot, relevantBimolecularReactions, relevantCollisionFreeReactions );
            couldReactOnCollision = GetCouldReactOnCollision();
            SetColorForCurrentState();
        }

        protected virtual void CreateBindingSites (MoleculeSnapshot moleculeSnapshot, BimolecularReaction[] relevantBimolecularReactions, 
                                                   CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (BindingSiteRef bindingSiteRef in definition.bindingSiteDefs.Keys)
            {
                CreateBindingSite( bindingSiteRef, moleculeSnapshot, relevantBimolecularReactions, relevantCollisionFreeReactions );
            }
            SetBindingSiteStates( moleculeSnapshot );
        }

        protected virtual void CreateBindingSite (BindingSiteRef bindingSiteRef, MoleculeSnapshot moleculeSnapshot, 
                                                  BimolecularReaction[] relevantBimolecularReactions, 
                                                  CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            GameObject bindingSiteObject = new GameObject();
            bindingSiteObject.transform.SetParent( theTransform );
            definition.bindingSiteDefs[bindingSiteRef].transformOnMolecule.Apply( theTransform, bindingSiteObject.transform );
            bindingSiteObject.name = name + "_" + bindingSiteRef;

            BindingSite bindingSite = bindingSiteObject.AddComponent<BindingSite>();
            bindingSite.Init( bindingSiteRef, moleculeSnapshot, relevantBimolecularReactions, relevantCollisionFreeReactions, this );

            bindingSites.Add( bindingSiteRef, bindingSite );
        }

        public virtual void SetBindingSiteStates (MoleculeSnapshot moleculeSnapshot)
        {
            //TODO consider current states for binding sites with same ID
            foreach (SiteState site in moleculeSnapshot.siteStates)
            {
                if (bindingSites.ContainsKey( site.siteRef ))
                {
                    bindingSites[site.siteRef].state = site.state;
                }
            }
        }

        public virtual bool InteractWith (Molecule other)
        {
            foreach (BindingSite bindingSite in bindingSites.Values)
            {
                if (bindingSite.couldReactOnCollision)
                {
                    foreach (BindingSite otherBindingSite in other.bindingSites.Values)
                    {
                        if (otherBindingSite.couldReactOnCollision && bindingSite.ReactWith( otherBindingSite ))
                        {
                            return true;
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
            name = complex.name + "_" + definition.species;
            theTransform.SetParent( complex.theTransform );

            UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
        }

        public virtual void UpdateReactions (BimolecularReaction[] relevantBimolecularReactions, 
                                             CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (BindingSite bindingSite in bindingSites.Values)
            {
                bindingSite.UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
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
                foreach (MoleculeSnapshotColor moleculeColor in definition.colors)
                {
                    if (moleculeColor.snapshot.IsSatisfiedBy( this ))
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
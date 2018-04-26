using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule : MonoBehaviour 
    {
        public Complex complex;
        public MoleculeDef definition;
        public Dictionary<string,List<BindingSite>> bindingSites = new Dictionary<string, List<BindingSite>>();
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
            foreach (List<BindingSite> aTypeOfBindingSite in bindingSites.Values)
            {
                foreach (BindingSite bindingSite in aTypeOfBindingSite)
                {
                    if (bindingSite.couldReactOnCollision)
                    {
                        return true;
                    }
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
            foreach (List<BindingSiteDef> aTypeOfBindingSite in definition.bindingSiteDefs.Values)
            {
                foreach (BindingSiteDef bindingSiteDef in aTypeOfBindingSite)
                {
                    CreateBindingSite( bindingSiteDef, relevantBimolecularReactions, relevantCollisionFreeReactions );
                }
            }
            moleculeSnapshot.SetStateOfMolecule( this );
        }

        protected virtual void CreateBindingSite (BindingSiteDef bindingSiteDef, BimolecularReaction[] relevantBimolecularReactions, 
                                                  CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            GameObject bindingSiteObject = new GameObject();
            bindingSiteObject.transform.SetParent( theTransform );
            bindingSiteDef.transformOnMolecule.Apply( theTransform, bindingSiteObject.transform );
            bindingSiteObject.name = name + "_" + bindingSiteDef.id;

            BindingSite bindingSite = bindingSiteObject.AddComponent<BindingSite>();
            bindingSite.Init( bindingSiteDef, relevantBimolecularReactions, relevantCollisionFreeReactions, this );

            if (!bindingSites.ContainsKey( bindingSiteDef.id))
            {
                bindingSites.Add( bindingSiteDef.id, new List<BindingSite>() );
            }
            bindingSites[bindingSiteDef.id].Add( bindingSite );
        }

        public virtual bool InteractWith (Molecule other)
        {
            foreach (List<BindingSite> aTypeOfBindingSite in bindingSites.Values)
            {
                foreach (BindingSite bindingSite in aTypeOfBindingSite)
                {
                    if (bindingSite.couldReactOnCollision)
                    {
                        foreach (List<BindingSite> aTypeOfBindingSiteOther in other.bindingSites.Values)
                        {
                            foreach (BindingSite otherBindingSite in aTypeOfBindingSiteOther)
                            {
                                if (otherBindingSite.couldReactOnCollision && bindingSite.ReactWith( otherBindingSite ))
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
            name = complex.name + "_" + definition.species;
            theTransform.SetParent( complex.theTransform );

            UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
        }

        public virtual void UpdateReactions (BimolecularReaction[] relevantBimolecularReactions, 
                                             CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            foreach (List<BindingSite> aTypeOfBindingSite in bindingSites.Values)
            {
                foreach (BindingSite bindingSite in aTypeOfBindingSite)
                {
                    bindingSite.UpdateReactions( relevantBimolecularReactions, relevantCollisionFreeReactions );
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
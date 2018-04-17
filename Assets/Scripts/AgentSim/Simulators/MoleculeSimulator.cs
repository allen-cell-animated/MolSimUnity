using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Molecule : MonoBehaviour 
    {
        public Complex complex;
        public MoleculeDef definition;
        public Dictionary<string,BindingSite> bindingSites = new Dictionary<string,BindingSite>();
        public float collisionRadius;
        public float interactionRadius;

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

        public bool couldReactOnCollision;

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

        public string species
        {
            get
            {
                return definition.species;
            }
        }

        public virtual void Init (MoleculeSnapshot moleculeSnapshot, Complex _complex, 
                                  BimolecularReactionSimulator[] relevantBimolecularSimulators, CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            complex = _complex;
            definition = moleculeSnapshot.moleculeDef;
            collisionRadius = interactionRadius = definition.radius;
            interactionRadius += 1f;
            CreateBindingSites( moleculeSnapshot, relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            couldReactOnCollision = GetCouldReactOnCollision();
        }

        protected virtual void CreateBindingSites (MoleculeSnapshot moleculeSnapshot, BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                                   CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            foreach (string bindingSiteID in definition.bindingSiteDefs.Keys)
            {
                CreateBindingSite( bindingSiteID, moleculeSnapshot, relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            }
        }

        protected virtual void CreateBindingSite (string bindingSiteID, MoleculeSnapshot moleculeSnapshot, 
                                                  BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                                  CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            GameObject bindingSiteObject = new GameObject();
            bindingSiteObject.transform.SetParent( theTransform );
            definition.bindingSiteDefs[bindingSiteID].transformOnMolecule.Apply( theTransform, bindingSiteObject.transform );
            bindingSiteObject.name = name + "_" + bindingSiteID;

            BindingSite bindingSite = bindingSiteObject.AddComponent<BindingSite>();
            bindingSite.Init( bindingSiteID, moleculeSnapshot, relevantBimolecularSimulators, relevantCollisionFreeSimulators, this );

            bindingSites.Add( bindingSiteID, bindingSite );
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

        public virtual void MoveToComplex (Complex _complex, BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                           CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            complex.Remove( this );
            complex = _complex;
            name = complex.name + "_" + species;
            theTransform.SetParent( complex.theTransform );

            UpdateReactions( relevantBimolecularSimulators, relevantCollisionFreeSimulators );
        }

        public virtual void UpdateReactions (BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                             CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            foreach (BindingSite bindingSite in bindingSites.Values)
            {
                bindingSite.UpdateReactions( relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            }
            couldReactOnCollision = GetCouldReactOnCollision();
        }

        public override string ToString ()
        {
            return "MoleculeSimulator " + name;
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

        public void SetColor (Color color)
        {
            material.color = color;
        }
	}
}
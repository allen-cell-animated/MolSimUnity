using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSite : MonoBehaviour 
    {
        public BindingSiteDef definition;
        public Molecule molecule;
        public string state;
        public BindingSite boundSite;
        public bool couldReactOnCollision;

        [SerializeField] protected BimolecularReaction[] bimolecularReactions;
        [SerializeField] protected CollisionFreeReaction[] collisionFreeReactions;

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

        public Complex complex
        {
            get
            {
                return molecule.complex;
            }
        }

        public Molecule[] molecules
        {
            get
            {
                return complex.molecules;
            }
        }

        public MoleculeDef moleculeDef
        {
            get
            {
                return molecule.definition;
            }
        }

        public string id
        {
            get
            {
                return definition.id;
            }
        }

        protected float interactionRadius
        {
            get
            {
                return definition.radius;
            }
        }

        public virtual void Init (string bindingSiteID, MoleculeSnapshot moleculeSnapshot, BimolecularReaction[] relevantBimolecularReactions, 
                                  CollisionFreeReaction[] relevantCollisionFreeReactions, Molecule _molecule)
        {
            definition = moleculeSnapshot.moleculeDef.bindingSiteDefs[bindingSiteID];
            molecule = _molecule;
            state = moleculeSnapshot.bindingSiteStates.ContainsKey(bindingSiteID) ? moleculeSnapshot.bindingSiteStates[bindingSiteID] 
                                                                                  : moleculeDef.bindingSiteDefs[bindingSiteID].states[0];
            SetBimolecularReactions( relevantBimolecularReactions );
            RegisterWithCollisionFreeReactions( relevantCollisionFreeReactions );
        }

        protected void SetBimolecularReactions (BimolecularReaction[] relevantBimolecularReactions)
        {
            List<BimolecularReaction> bimolecularReactionsList = new List<BimolecularReaction>();
            foreach (BimolecularReaction reaction in relevantBimolecularReactions)
            {
                if (reaction.SiteIsRelevant( this ))
                {
                    bimolecularReactionsList.Add( reaction );
                }
            }
            bimolecularReactions = bimolecularReactionsList.ToArray();
            couldReactOnCollision = bimolecularReactions.Length > 0;
        }

        protected void RegisterWithCollisionFreeReactions (CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            List<CollisionFreeReaction> collisionFreeReactionsList = new List<CollisionFreeReaction>();
            foreach (CollisionFreeReaction reaction in relevantCollisionFreeReactions)
            {
                if (reaction.RegisterBindingSite( this ))
                {
                    collisionFreeReactionsList.Add( reaction );
                }
            }
            collisionFreeReactions = collisionFreeReactionsList.ToArray();
        }

        protected void UnregisterWithCollisionFreeReactions ()
        {
            foreach (CollisionFreeReaction reaction in collisionFreeReactions)
            {
                reaction.UnregisterBindingSite( this );
            }
        }

        public virtual bool ReactWith (BindingSite other)
        {
            if (IsNear( other ))
            {
                bimolecularReactions.Shuffle();
                foreach (BimolecularReaction bimolecularReaction in bimolecularReactions)
                {
                    if (bimolecularReaction.TryReactOnCollision( this, other ))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        bool IsNear (BindingSite other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
        }

        public virtual void UpdateReactions (BimolecularReaction[] relevantBimolecularReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            SetBimolecularReactions( relevantBimolecularReactions );
            UnregisterWithCollisionFreeReactions();
            RegisterWithCollisionFreeReactions( relevantCollisionFreeReactions );
            name = molecule.name + "_" + id;
        }

        public override string ToString ()
        {
            return "BindingSite " + name;
        }
    }
}
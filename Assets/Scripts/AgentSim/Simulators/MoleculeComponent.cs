using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeComponent : MonoBehaviour, IComponent
    {
        public ComponentDef definition;
        public Molecule molecule;
        public MoleculeComponent boundComponent;
        public bool couldReactOnCollision;

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

        public string componentName
        {
            get
            {
                return definition.componentName;
            }
        }

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

        protected float interactionRadius
        {
            get
            {
                return definition.radius;
            }
        }

        public virtual void Init (ComponentDef componentDef, BimolecularReaction[] relevantBimolecularReactions, 
                                  CollisionFreeReaction[] relevantCollisionFreeReactions, Molecule _molecule)
        {
            definition = componentDef;
            molecule = _molecule;
            state = (definition.states == null || definition.states.Length < 1) ? "" : definition.states[0];
            SetBimolecularReactions( relevantBimolecularReactions );
            RegisterWithCollisionFreeReactions( relevantCollisionFreeReactions );
        }

        protected void SetBimolecularReactions (BimolecularReaction[] relevantBimolecularReactions)
        {
            List<BimolecularReaction> bimolecularReactionsList = new List<BimolecularReaction>();
            foreach (BimolecularReaction reaction in relevantBimolecularReactions)
            {
                if (reaction.definition.ComponentIsInReactionCenter( this ))
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
                if (reaction.RegisterComponent( this ))
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
                reaction.UnregisterComponent( this );
            }
        }

        public virtual bool ReactWith (MoleculeComponent other)
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

        bool IsNear (MoleculeComponent other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
        }

        public virtual void UpdateReactions (BimolecularReaction[] relevantBimolecularReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            SetBimolecularReactions( relevantBimolecularReactions );
            UnregisterWithCollisionFreeReactions();
            RegisterWithCollisionFreeReactions( relevantCollisionFreeReactions );
            name = molecule.name + "_" + componentName;
        }

        public override string ToString ()
        {
            return "Component " + name;
        }
    }
}
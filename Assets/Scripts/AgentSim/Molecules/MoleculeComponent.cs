using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeComponent : IComponent
    {
        public ComponentDef definition;
        public Molecule molecule;
        public MoleculeComponent boundComponent;
        public bool couldReactOnCollision;
        public bool stateWasUpdated;
        public string lastBondName;
        public RelativeTransform localTransform;

        public RelativeTransform worldTransform
        {
            get
            {
                return complex.reactor.GetWorldTransform( molecule.worldTransform, localTransform );
            }
        }

        public Vector3 position
        {
            get
            {
                return worldTransform.position;
            }
        }

        public Vector3 rotation
        {
            get
            {
                return worldTransform.rotation;
            }
        }

        #region IComponent
        public string componentName
        {
            get
            {
                return definition.componentName;
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

        public bool bound
        {
            get
            {
                return boundComponent != null;
            }
        }

        public bool Matches (IComponent other)
        {
            return other.componentName == componentName && other.state == state && other.bound == bound;
        }
        #endregion

        [SerializeField] protected ReactionCenter[] bindReactionCenters;
        [SerializeField] protected CollisionFreeReaction[] collisionFreeReactions;

        public Complex complex
        {
            get
            {
                return molecule.complex;
            }
        }

        protected float interactionRadius
        {
            get
            {
                return definition.radius;
            }
        }

        public MoleculeComponent (ComponentDef componentDef, Molecule _molecule)
        {
            definition = componentDef;
            molecule = _molecule;
            state = (definition.states == null || definition.states.Length < 1) ? "" : definition.states[0];
            localTransform = new RelativeTransform( componentDef.transformOnMolecule );
        }

        public virtual bool ReactWith (MoleculeComponent other)
        {
            if (IsNear( other ))
            {
                bindReactionCenters.Shuffle();
                other.bindReactionCenters.Shuffle();
                foreach (ReactionCenter reactionCenter in bindReactionCenters)
                {
                    foreach (ReactionCenter otherReactionCenter in other.bindReactionCenters)
                    {
                        if (reactionCenter != otherReactionCenter && reactionCenter.reaction == otherReactionCenter.reaction && reactionCenter.reaction.ShouldHappen())
                        {
                            if (reactionCenter.reaction.React( new MoleculeComponent[]{ this, other }, new ReactionCenter[]{ reactionCenter, otherReactionCenter } ))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        bool IsNear (MoleculeComponent other)
        {
            return other != this 
                && Vector3.Distance( position, other.position ) < interactionRadius + other.interactionRadius;
        }

        public void SetToProductState (MoleculePattern productMolecule, ComponentPattern productComponent)
        {
            productComponent.SetStateOfComponent( this );
            stateWasUpdated = true;
            molecule.SetToProductState( productMolecule );
        }

        public void SetToProductState (ReactionCenter reactionCenter)
        {
            reactionCenter.productComponent.SetStateOfComponent( this );
            stateWasUpdated = true;
            molecule.SetToProductState( reactionCenter.productMolecule );
        }

        public virtual void UpdateReactions (BindReaction[] relevantBindReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            SetBindReactionCenters( relevantBindReactions );
            UnregisterWithCollisionFreeReactions();
            RegisterWithCollisionFreeReactions( relevantCollisionFreeReactions );
        }

        protected void SetBindReactionCenters (BindReaction[] relevantBindReactions)
        {
            List<ReactionCenter> reactionCentersList = new List<ReactionCenter>();
            foreach (BindReaction reaction in relevantBindReactions)
            {
                ReactionCenter reactionCenter = reaction.GetReactionCenterForComponent( this );
                if (reactionCenter != null)
                {
                    reactionCentersList.Add( reactionCenter );
                }
            }
            bindReactionCenters = reactionCentersList.ToArray();
            couldReactOnCollision = bindReactionCenters.Length > 0;
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
            if (collisionFreeReactions != null)
            {
                foreach (CollisionFreeReaction reaction in collisionFreeReactions)
                {
                    reaction.UnregisterComponent( this );
                }
            }
        }

        public override string ToString ()
        {
            return componentName + "~" + state + (bound ? "!" + (string.IsNullOrEmpty( lastBondName ) ? "+" : lastBondName ) : "");
        }
    }
}
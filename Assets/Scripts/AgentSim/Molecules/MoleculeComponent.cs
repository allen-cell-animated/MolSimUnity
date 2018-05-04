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

        public bool Matches (IComponent other)
        {
            return other.componentName == componentName && (other.state.Contains( "!" ) ? state.Contains( "!" ) : other.state == state);
        }

        [SerializeField] protected ReactionCenter[] bimolecularReactionCenters;
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

        public Dictionary<string,List<Molecule>> molecules
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

        public virtual void Init (ComponentDef componentDef, Molecule _molecule)
        {
            definition = componentDef;
            molecule = _molecule;
            state = (definition.states == null || definition.states.Length < 1) ? "" : definition.states[0];
        }

        public virtual bool ReactWith (MoleculeComponent other)
        {
            if (IsNear( other ))
            {
                bimolecularReactionCenters.Shuffle();
                other.bimolecularReactionCenters.Shuffle();
                foreach (ReactionCenter reactionCenter in bimolecularReactionCenters)
                {
                    foreach (ReactionCenter otherReactionCenter in other.bimolecularReactionCenters)
                    {
                        if (reactionCenter.reaction == otherReactionCenter.reaction)
                        {
                            reactionCenter.reaction.React( this, other );
                        }
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

        public virtual void UpdateReactions (BindReaction[] relevantBindReactions, CollisionFreeReaction[] relevantCollisionFreeReactions)
        {
            SetBindReactionCenters( relevantBindReactions );
            UnregisterWithCollisionFreeReactions();
            RegisterWithCollisionFreeReactions( relevantCollisionFreeReactions );
            name = molecule.name + "_" + componentName;
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
            bimolecularReactionCenters = reactionCentersList.ToArray();
            couldReactOnCollision = bimolecularReactionCenters.Length > 0;
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
            return "Component " + name + " (state=" + state + ")";
        }
    }
}
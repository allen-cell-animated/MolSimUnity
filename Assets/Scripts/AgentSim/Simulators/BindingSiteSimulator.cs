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

        [SerializeField] protected BimolecularReactionSimulator[] bimolecularReactionSimulators;
        [SerializeField] protected CollisionFreeReactionSimulator[] collisionFreeReactionSimulators;

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

        public Reactor reactor
        {
            get
            {
                return complex.reactor;
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

        public float interactionRadius
        {
            get
            {
                return definition.radius;
            }
        }

        public virtual void Init (string bindingSiteID, MoleculeSnapshot moleculeSnapshot, BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                  CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators, Molecule _molecule)
        {
            definition = moleculeSnapshot.moleculeDef.bindingSiteDefs[bindingSiteID];
            molecule = _molecule;
            state = moleculeSnapshot.bindingSiteStates.ContainsKey(bindingSiteID) ? moleculeSnapshot.bindingSiteStates[bindingSiteID] 
                                                                                  : moleculeDef.bindingSiteDefs[bindingSiteID].states[0];
            SetBimolecularReactionSimulators( relevantBimolecularSimulators );
            RegisterWithCollisionFreeReactionSimulators( relevantCollisionFreeSimulators );
        }

        protected virtual void SetBimolecularReactionSimulators (BimolecularReactionSimulator[] relevantBimolecularSimulators)
        {
            List<BimolecularReactionSimulator> bimolecularReactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in relevantBimolecularSimulators)
            {
                if (reactionSimulator.SiteIsRelevant( this ))
                {
                    bimolecularReactionSimulatorsList.Add( reactionSimulator );
                }
            }
            bimolecularReactionSimulators = bimolecularReactionSimulatorsList.ToArray();
            couldReactOnCollision = bimolecularReactionSimulators.Length > 0;
        }

        protected virtual void RegisterWithCollisionFreeReactionSimulators (CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            List<CollisionFreeReactionSimulator> collisionFreeReactionSimulatorsList = new List<CollisionFreeReactionSimulator>();
            foreach (CollisionFreeReactionSimulator reactionSimulator in relevantCollisionFreeSimulators)
            {
                if (reactionSimulator.Register( this ))
                {
                    collisionFreeReactionSimulatorsList.Add( reactionSimulator );
                }
            }
            collisionFreeReactionSimulators = collisionFreeReactionSimulatorsList.ToArray();
        }

        protected virtual void UnregisterWithCollisionFreeReactionSimulators ()
        {
            foreach (CollisionFreeReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                reactionSimulator.Unregister( this );
            }
        }

        public virtual bool ReactWith (BindingSite other)
        {
            if (IsNear( other ))
            {
                bimolecularReactionSimulators.Shuffle();
                foreach (BimolecularReactionSimulator bimolecularReactionSimulator in bimolecularReactionSimulators)
                {
                    if (bimolecularReactionSimulator.TryReactOnCollision( this, other ))
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

        public virtual void UpdateReactions (BimolecularReactionSimulator[] relevantBimolecularSimulators, CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            SetBimolecularReactionSimulators( relevantBimolecularSimulators );
            UnregisterWithCollisionFreeReactionSimulators();
            RegisterWithCollisionFreeReactionSimulators( relevantCollisionFreeSimulators );
            name = molecule.name + "_" + id;
        }

        public override string ToString ()
        {
            return "BindingSiteSimulator " + name;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSiteSimulator : MonoBehaviour 
    {
        public BindingSite bindingSite;
        public MoleculeSimulator moleculeSimulator;
        public string state;
        public BindingSiteSimulator boundSite;

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

        public bool couldReactOnCollision
        {
            get
            {
                return bimolecularReactionSimulators.Length > 0;
            }
        }

        public Reactor reactor
        {
            get
            {
                return particleSimulator.population.reactor;
            }
        }

        public ParticleSimulator particleSimulator
        {
            get
            {
                return moleculeSimulator.particleSimulator;
            }
        }

        public MoleculeSimulator[] complex
        {
            get
            {
                return particleSimulator.complex;
            }
        }

        public Molecule molecule
        {
            get
            {
                return moleculeSimulator.molecule;
            }
        }

        public string id
        {
            get
            {
                return bindingSite.id;
            }
        }

        public float interactionRadius
        {
            get
            {
                return bindingSite.radius;
            }
        }

        public virtual void Init (string bindingSiteID, MoleculeState moleculeState, BimolecularReactionSimulator[] relevantBimolecularSimulators, 
                                  CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators, MoleculeSimulator _moleculeSimulator)
        {
            bindingSite = moleculeState.molecule.bindingSites[bindingSiteID];
            moleculeSimulator = _moleculeSimulator;
            state = moleculeState.bindingSiteStates.ContainsKey(bindingSiteID) ? moleculeState.bindingSiteStates[bindingSiteID] : molecule.bindingSites[bindingSiteID].states[0];
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

        public virtual bool ReactWith (BindingSiteSimulator other)
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

        bool IsNear (BindingSiteSimulator other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
        }

        public virtual void UpdateReactions (BimolecularReactionSimulator[] relevantBimolecularSimulators, CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators)
        {
            SetBimolecularReactionSimulators( relevantBimolecularSimulators );
            UnregisterWithCollisionFreeReactionSimulators();
            RegisterWithCollisionFreeReactionSimulators( relevantCollisionFreeSimulators );
            name = moleculeSimulator.name + "_" + id;
        }

        public override string ToString ()
        {
            return "BindingSiteSimulator " + name;
        }
    }
}
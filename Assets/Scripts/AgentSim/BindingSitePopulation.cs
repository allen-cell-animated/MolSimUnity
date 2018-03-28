using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSitePopulation : MonoBehaviour 
    {
        public ParticlePopulation particlePopulation;
        public MoleculeBindingSite moleculeBindingSite;
        public string initialState;
        public float interactionRadius;
        public List<BindingSiteSimulator> bindingSiteSimulators = new List<BindingSiteSimulator>();

        public BindingSite bindingSite
        {
            get
            {
                return moleculeBindingSite.molecule.bindingSites[moleculeBindingSite.bindingSiteID];
            }
        }

        public string id
        {
            get
            {
                return moleculeBindingSite.bindingSiteID;
            }
        }

        public RelativeTransform transformOnMolecule
        {
            get
            {
                return bindingSite.transformOnMolecule;
            }
        }

        public float maxExtentFromMoleculeCenter
        {
            get
            {
                return Vector3.Magnitude( bindingSite.transformOnMolecule.position ) + interactionRadius;
            }
        }

        BimolecularReactionSimulator[] _reactionSimulators;
        BimolecularReactionSimulator[] reactionSimulators
        {
            get
            {
                if (_reactionSimulators == null)
                {
                    List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
                    foreach (BimolecularReactionSimulator reactionSimulator in particlePopulation.reactor.bimolecularReactionSimulators)
                    {
                        foreach (MoleculeBindingSite site in reactionSimulator.reaction.relevantSites)
                        {
                            if (moleculeBindingSite.molecule == site.molecule && bindingSite.id == site.bindingSiteID)
                            {
                                reactionSimulatorsList.Add( reactionSimulator );
                            }
                        }
                    }
                    _reactionSimulators = reactionSimulatorsList.ToArray();
                }
                return _reactionSimulators;
            }
        }

        public virtual void Init (MoleculeState moleculeState, string _bindingSiteID, ParticlePopulation _particlePopulation)
        {
            particlePopulation = _particlePopulation;
            moleculeBindingSite = new MoleculeBindingSite( moleculeState.molecule, _bindingSiteID );
            initialState = string.IsNullOrEmpty( moleculeState.bindingSiteStates[_bindingSiteID] ) ? bindingSite.states[0] :  moleculeState.bindingSiteStates[_bindingSiteID];
            interactionRadius = bindingSite.radius;

            foreach (CollisionFreeReactionSimulator reactionWatcher in _particlePopulation.reactor.collisionFreeReactionSimulators)
            {
                reactionWatcher.RegisterBindingSitePopulation( this, _particlePopulation.complexState );
            }
        }

        public void RegisterBindingSiteSimulator (BindingSiteSimulator _bindingSiteSimulator)
        {
            if (!bindingSiteSimulators.Contains( _bindingSiteSimulator ))
            {
                bindingSiteSimulators.Add(_bindingSiteSimulator );
            }
        }

        public void UnregisterBindingSiteSimulator (BindingSiteSimulator _bindingSiteSimulator)
        {
            if (bindingSiteSimulators.Contains( _bindingSiteSimulator ))
            {
                bindingSiteSimulators.Remove(_bindingSiteSimulator );
            }
        }

        public virtual bool DoCollisionFreeReaction (Reaction reaction)
        {
            if (bindingSiteSimulators.Count > 0)
            {
                bindingSiteSimulators.Shuffle();
                reaction.React( bindingSiteSimulators[0] );
                return true;
            }
            return false;
        }

        public virtual bool DoBimolecularReaction (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2)
        {
            reactionSimulators.Shuffle();
            foreach (BimolecularReactionSimulator reactionSimulator in reactionSimulators)
            {
                return reactionSimulator.TryReactOnCollision( bindingSiteSimulator1, bindingSiteSimulator2 );
            }
            return false;
        }

        public override string ToString()
        {
            return "BindingSitePopulation for " + moleculeBindingSite + " on " + name;
        }
    }
}
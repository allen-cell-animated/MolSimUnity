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
        [SerializeField] protected List<BindingSiteSimulator> bindingSiteSimulators = new List<BindingSiteSimulator>();

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

        BimolecularReactionWatcher[] _reactionWatchers;
        BimolecularReactionWatcher[] reactionWatchers
        {
            get
            {
                if (_reactionWatchers == null)
                {
                    List<BimolecularReactionWatcher> reactionWatchersList = new List<BimolecularReactionWatcher>();
                    foreach (BimolecularReactionWatcher reactionWatcher in particlePopulation.reactor.bimolecularReactionWatchers)
                    {
                        foreach (MoleculeBindingSite site in reactionWatcher.reaction.relevantSites)
                        {
                            if (moleculeBindingSite.molecule == site.molecule && bindingSite.id == site.bindingSiteID)
                            {
                                reactionWatchersList.Add( reactionWatcher );
                            }
                        }
                    }
                    _reactionWatchers = reactionWatchersList.ToArray();
                }
                return _reactionWatchers;
            }
        }

        public virtual void Init (MoleculeState moleculeState, string _bindingSiteID, ParticlePopulation _particlePopulation)
        {
            particlePopulation = _particlePopulation;
            moleculeBindingSite = new MoleculeBindingSite( moleculeState.molecule, _bindingSiteID );
            initialState = string.IsNullOrEmpty( moleculeState.bindingSiteStates[_bindingSiteID] ) ? bindingSite.states[0] :  moleculeState.bindingSiteStates[_bindingSiteID];
            interactionRadius = bindingSite.radius;

            foreach (CollisionFreeReactionWatcher reactionWatcher in _particlePopulation.reactor.collisionFreeReactionWatchers)
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

        public virtual void DoCollisionFreeReaction (Reaction reaction)
        {
            if (bindingSiteSimulators.Count > 0)
            {
                bindingSiteSimulators.Shuffle();
                reaction.React( bindingSiteSimulators[0] );
            }
        }

        public virtual Reaction GetNextBimolecularReaction (BindingSiteSimulator bindingSiteSimulator1, BindingSiteSimulator bindingSiteSimulator2)
        {
            reactionWatchers.Shuffle();
            foreach (BimolecularReactionWatcher reactionWatcher in reactionWatchers)
            {
                if (reactionWatcher.TryReactOnCollision( bindingSiteSimulator1, bindingSiteSimulator2 ))
                {
                    return reactionWatcher.reaction;
                }
            }
            return null;
        }
    }
}
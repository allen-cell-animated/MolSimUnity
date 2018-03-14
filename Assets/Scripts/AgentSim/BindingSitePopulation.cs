using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSitePopulation : MonoBehaviour 
    {
        public Molecule molecule;
        public MoleculePopulation moleculePopulation;
        public BindingSite bindingSite;
        public string initialState;
        public float interactionRadius;

        public float maxExtentFromMoleculeCenter
        {
            get
            {
                return Vector3.Magnitude( bindingSite.transformOnMolecule.position ) + interactionRadius;
            }
        }

        ReactionWatcher[] _reactionWatchers;
        public ReactionWatcher[] reactionWatchers
        {
            get
            {
                if (_reactionWatchers == null)
                {
                    Reaction reaction;
                    List<ReactionWatcher> reactionWatchersList = new List<ReactionWatcher>();
                    for (int i = 0; i < moleculePopulation.reactor.model.reactions.Length; i++)
                    {
                        reaction = moleculePopulation.reactor.model.reactions[i];
                        foreach (MoleculeBindingSite site in reaction.relevantSites)
                        {
                            if (site.molecule == molecule && bindingSite.id == site.bindingSiteID)
                            {
                                reactionWatchersList.Add( moleculePopulation.reactor.reactionWatchers[i] );
                            }
                        }
                    }
                    _reactionWatchers = reactionWatchersList.ToArray();
                }
                return _reactionWatchers;
            }
        }

        public virtual void Init (Molecule _molecule, BindingSite _bindingSite, string _initialState, MoleculePopulation _moleculePopulation)
        {
            molecule = _molecule;
            moleculePopulation = _moleculePopulation;
            bindingSite = _bindingSite;
            initialState = string.IsNullOrEmpty( _initialState ) ? bindingSite.states[0] :  _initialState;
            interactionRadius = bindingSite.radius;
        }

        public bool StateIsActive (string state)
        {
            foreach (string activeState in bindingSite.activeStates)
            {
                if (state == activeState)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual Reaction GetNextReaction (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2)
        {
            reactionWatchers.Shuffle();
            foreach (ReactionWatcher reactionWatcher in reactionWatchers)
            {
                if (reactionWatcher.TryReaction( bindingSite1, bindingSite2 ))
                {
                    return reactionWatcher.reaction;
                }
            }
            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSitePopulation : MonoBehaviour 
    {
        public ComplexPopulation complexPopulation;
        public Molecule molecule;
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
                    ComplexPopulation complexPopulation = GetComponent<ComplexPopulation>();
                    for (int i = 0; i < complexPopulation.reactor.model.reactions.Length; i++)
                    {
                        reaction = complexPopulation.reactor.model.reactions[i];
                        foreach (MoleculeBindingSite site in reaction.relevantSites)
                        {
                            if (molecule == site.molecule && bindingSite.id == site.bindingSiteID)
                            {
                                reactionWatchersList.Add( complexPopulation.reactor.reactionWatchers[i] );
                            }
                        }
                    }
                    _reactionWatchers = reactionWatchersList.ToArray();
                }
                return _reactionWatchers;
            }
        }

        public virtual void Init (Molecule _molecule, BindingSite _bindingSite, string _initialState, ComplexPopulation _complexPopulation)
        {
            complexPopulation = _complexPopulation;
            molecule = _molecule;
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
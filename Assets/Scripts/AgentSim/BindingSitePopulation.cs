using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class BindingSitePopulation : MonoBehaviour 
    {
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
                    List<ReactionWatcher> reactionWatchersList = new List<ReactionWatcher>();
                    for (int i = 0; i < moleculePopulation.reactor.model.reactions.Length; i++)
                    {
                        foreach (MoleculeBindingSite site in moleculePopulation.reactor.model.reactions[i].relevantSites)
                        {
                            if (moleculePopulation.molecule == site.molecule && bindingSite.id == site.bindingSiteID)
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

        public virtual void Init (BindingSite _bindingSite, string _initialState, MoleculePopulation _moleculePopulation)
        {
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
    }
}
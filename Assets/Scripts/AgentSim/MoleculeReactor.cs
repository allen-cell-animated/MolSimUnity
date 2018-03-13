using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class MoleculeReactor : AgentComponent 
    {
        [Tooltip( "use physics engine for collision detection or let container manage collisions?" )]
        public bool usePhysicsEngine;
        [Tooltip( "if not using physics engine, how many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;
        public Model model;

        public ReactionWatcher[] reactionWatchers;
        [HideInInspector] public Container container;

        void Start ()
        {
            SetupReactionData();
            CreateContainer();

            foreach (MoleculeConcentration molecule in model.molecules)
            {
                CreatePopulation( molecule );
            }
        }

        protected virtual void SetupReactionData ()
        {
            reactionWatchers = new ReactionWatcher[model.reactions.Length];
            for (int i = 0; i < model.reactions.Length; i++)
            {
                model.reactions[i].Init(); //for prototyping in inspector without writing custom property drawer etc
                reactionWatchers[i] = new ReactionWatcher( model.reactions[i] );
            }
        }

        protected virtual void CreateContainer ()
        {
            container = gameObject.AddComponent<Container>();
            container.Init( model.containerVolume, periodicBoundary );
        }

        protected virtual void CreatePopulation (MoleculeConcentration moleculeConcentration)
        {
            GameObject population = new GameObject( moleculeConcentration.moleculeState.molecule.species + "Population", new System.Type[] {typeof(Agent), typeof(MoleculePopulation)} );
            population.transform.SetParent( transform );
            population.GetComponent<Agent>().Init( moleculeConcentration.moleculeState.molecule.species, agent.scale );
            population.GetComponent<MoleculePopulation>().Init( moleculeConcentration, this );
        }
    }

    // runtime data for a reaction used to keep rate near its theoretical value
    [System.Serializable]
    public class ReactionWatcher
    {
        public Reaction reaction;

        public int attempts;
        public int events;
        public float observedRate;

        public ReactionWatcher (Reaction _reaction)
        {
            reaction = _reaction;
        }

        bool observedRateTooHigh
        {
            get
            {
                return observedRate > 1.2f * reaction.rate;
            }
        }

        bool observedRateTooLow
        {
            get
            {
                return observedRate < 0.8f * reaction.rate;
            }
        }

        bool shouldHappen
        {
            get
            {
                bool react;
                if (observedRateTooLow)
                {
                    react = true;
                }
                else 
                {
                    react = Random.value <= reaction.rate * World.Instance.dT * (World.Instance.steps / attempts);
                }
                events = react ? events + 1 : events;
                observedRate = Mathf.Round( events / World.Instance.time );
                return react;
            }
        }

        public bool TryReaction (BindingSiteSimulator bindingSite1, BindingSiteSimulator bindingSite2)
        {
            attempts++;

            if (!observedRateTooHigh && ReactantsEqual( bindingSite1.molecule, bindingSite2.molecule ))
            {
                return shouldHappen;
            }
            return false;
        }

        bool ReactantsEqual (MoleculeSimulator molecule1, MoleculeSimulator molecule2)
        {
            return (reaction.reactants.Length == 0 && molecule1 == null && molecule1 == null)
                || (reaction.reactants.Length == 1 && ((reaction.reactants[0].Matches( molecule1 ) && molecule2 == null)
                                                    || (reaction.reactants[0].Matches( molecule2 ) && molecule1 == null)))
                || (reaction.reactants.Length == 2 && ((reaction.reactants[0].Matches( molecule1 ) && reaction.reactants[1].Matches( molecule2 ) ))
                                                    || (reaction.reactants[0].Matches( molecule2 ) && reaction.reactants[1].Matches( molecule1 ) ));
        }

        public void Reset ()
        {
            events = attempts = 0;
            observedRate = 0;
        }
    }
}
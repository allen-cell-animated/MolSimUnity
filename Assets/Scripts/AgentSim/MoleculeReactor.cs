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

        public Dictionary<string,MoleculePopulation> populations;
        public ReactionWatcher[] reactionWatchers;
        [HideInInspector] public Container container;

        void Start ()
        {
            SetupReactionData();
            CreateContainer();

            populations = new Dictionary<string,MoleculePopulation>();
            foreach (MoleculeConcentration molecule in model.molecules)
            {
                CreatePopulation( molecule );
            }
        }

        protected virtual void SetupReactionData ()
        {
            model.Init(); //for prototyping in inspector without writing custom property drawer etc
            reactionWatchers = new ReactionWatcher[model.reactions.Length];
            for (int i = 0; i < model.reactions.Length; i++)
            {
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
            GameObject obj = new GameObject( moleculeConcentration.species + "Population", new System.Type[] {typeof(Agent), typeof(MoleculePopulation)} );
            Agent a = obj.GetComponent<Agent>();
            a.Init( moleculeConcentration.species, agent.scale );
            a.SetParent( agent );
            MoleculePopulation population = obj.GetComponent<MoleculePopulation>();
            population.Init( moleculeConcentration, this );
            populations.Add( moleculeConcentration.species, population );
        }

        public virtual MoleculePopulation GetPopulationForMoleculeSet (MoleculeStateSet moleculeStateSet)
        {
            if (!populations.ContainsKey( moleculeStateSet.species ))
            {
                CreatePopulation( new MoleculeConcentration( moleculeStateSet, 0 ) );
            }
            return populations[moleculeStateSet.species];
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

            if (!observedRateTooHigh && ReactantsEqual( bindingSite1.molecule.GetBoundMoleculesSet(), bindingSite2.molecule.GetBoundMoleculesSet() ))
            {
                return shouldHappen;
            }
            return false;
        }

        bool ReactantsEqual (MoleculeSimulator[] moleculeSet1, MoleculeSimulator[] moleculeSet2)
        {
            return (reaction.reactants.Length == 0 && moleculeSet1 == null && moleculeSet2 == null)
                || (reaction.reactants.Length == 1 && ((reaction.reactants[0].Matches( moleculeSet1 ) && moleculeSet2 == null)
                                                    || (reaction.reactants[0].Matches( moleculeSet2 ) && moleculeSet1 == null)))
                || (reaction.reactants.Length == 2 && ((reaction.reactants[0].Matches( moleculeSet1 ) && reaction.reactants[1].Matches( moleculeSet2 ) ))
                                                    || (reaction.reactants[0].Matches( moleculeSet2 ) && reaction.reactants[1].Matches( moleculeSet1 ) ));
        }

        public void Reset ()
        {
            events = attempts = 0;
            observedRate = 0;
        }
    }
}
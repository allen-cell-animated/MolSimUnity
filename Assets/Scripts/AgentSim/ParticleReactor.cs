using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleReactor : AgentComponent 
    {
        [Tooltip( "use physics engine for collision detection or let container manage collisions?" )]
        public bool usePhysicsEngine;
        [Tooltip( "if not using physics engine, how many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;
        public Model model;

        public ParticleReaction[] reactionData;
        [HideInInspector] public Container container;

        void Start ()
        {
            SetupReactionData();
            CreateContainer();
            CreateInitialSpecies();
        }

        protected virtual void SetupReactionData ()
        {
            reactionData = new ParticleReaction[model.reactions.Length];
            for (int i = 0; i < model.reactions.Length; i++)
            {
                reactionData[i] = new ParticleReaction( model.reactions[i] );
            }
        }

        protected virtual void CreateContainer ()
        {
            container = gameObject.AddComponent<Container>();
            container.Init( model.containerVolume, periodicBoundary );
        }

        protected virtual void CreateInitialSpecies ()
        {
            foreach (Molecule molecule in model.molecules)
            {
                CreateSpecies( molecule );
            }
        }

        protected virtual void CreateSpecies (Molecule molecule)
        {
            GameObject population = new GameObject( molecule.species + "Population", new System.Type[] {typeof(Agent), typeof(ParticlePopulation)} );
            population.transform.SetParent( transform );
            population.GetComponent<Agent>().Init( molecule.species, agent.scale );
            population.GetComponent<ParticlePopulation>().Init( molecule, this );
        }
    }

    // runtime data for a reaction used to keep rate near its theoretical value
    [System.Serializable]
    public class ParticleReaction
    {
        public Reaction reaction;

        public int attempts;
        public int events;
        public float observedRate;

        public ParticleReaction (Reaction _reaction)
        {
            reaction = _reaction;
        }

        public bool observedRateTooHigh
        {
            get
            {
                return observedRate > 1.2f * reaction.rate;
            }
        }

        public bool observedRateTooLow
        {
            get
            {
                return observedRate < 0.8f * reaction.rate;
            }
        }

        public bool ShouldHappen ()
        {
            attempts++;

            bool react;
            if (observedRateTooLow)
            {
                react = true;
            }
            else if (observedRateTooHigh)
            {
                react = false;
            }
            else 
            {
                react = Random.value <= reaction.rate * World.Instance.dT * (World.Instance.steps / attempts);
            }
            events = react ? events + 1 : events;
            observedRate = Mathf.Round( events / World.Instance.time );
            return react;
        }

        public void Reset ()
        {
            events = attempts = 0;
            observedRate = 0;
        }
    }
}
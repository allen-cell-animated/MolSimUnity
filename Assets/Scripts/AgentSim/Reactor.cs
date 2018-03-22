using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reactor : MonoBehaviour 
    {
        [HideInInspector] public Container container;
        public Model model;
        public List<BimolecularReactionWatcher> bimolecularReactionWatchers = new List<BimolecularReactionWatcher>();
        public List<CollisionFreeReactionWatcher> collisionFreeReactionWatchers = new List<CollisionFreeReactionWatcher>();
        public Dictionary<string,ParticlePopulation> particlePopulations = new Dictionary<string,ParticlePopulation>();
        [Tooltip( "How many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;

        protected List<ParticleSimulator> particleSimulators = new List<ParticleSimulator>();
        protected List<ParticleSimulator> activeParticleSimulators = new List<ParticleSimulator>();
        protected List<ParticleSimulator> particleSimulatorsToDestroy = new List<ParticleSimulator>();

        float dT
        {
            get
            {
                return World.Instance.dT;
            }
        }

        void Start ()
        {
            SetupReactionData();
            CreateContainer();

            foreach (ComplexConcentration complex in model.complexes)
            {
                CreatePopulation( complex );
            }
        }

        protected virtual void SetupReactionData ()
        {
            model.Init(); //for prototyping in inspector without writing custom property drawer etc

            foreach (Reaction reaction in model.reactions)
            {
                if (reaction.isBimolecular)
                {
                    bimolecularReactionWatchers.Add( new BimolecularReactionWatcher( reaction ) );
                }
                else
                {
                    collisionFreeReactionWatchers.Add( new CollisionFreeReactionWatcher( reaction ) );
                }
            }
        }

        protected virtual void CreateContainer ()
        {
            container = gameObject.AddComponent<Container>();
            container.Init( model.scale, model.containerVolume, periodicBoundary );
        }

        protected virtual void CreatePopulation (ComplexConcentration complexConcentration)
        {
            GameObject obj = new GameObject( complexConcentration.species + "Population" );
            obj.transform.SetParent( transform );

            ParticlePopulation particlePopulation = obj.AddComponent<ParticlePopulation>();
            particlePopulation.Init( complexConcentration, this );
            particlePopulations.Add( complexConcentration.species, particlePopulation );
        }

        public virtual ParticlePopulation GetPopulationForComplex (ComplexState complexState)
        {
            if (!particlePopulations.ContainsKey( complexState.species ))
            {
                CreatePopulation( new ComplexConcentration( complexState, 0 ) );
            }
            return particlePopulations[complexState.species];
        }

        public void RegisterParticleSimulator (ParticleSimulator particleSimulator)
        {
            if (!particleSimulators.Contains( particleSimulator ))
            {
                particleSimulators.Add( particleSimulator );
            }
            if (particleSimulator.active && !activeParticleSimulators.Contains( particleSimulator ))
            {
                activeParticleSimulators.Add( particleSimulator );
            }
        }

        public void UnregisterParticleSimulator (ParticleSimulator particleSimulator)
        {
            if (particleSimulators.Contains( particleSimulator ))
            {
                particleSimulators.Remove( particleSimulator );
            }
            if (activeParticleSimulators.Contains( particleSimulator ))
            {
                activeParticleSimulators.Remove( particleSimulator );
            }
            if (!particleSimulatorsToDestroy.Contains( particleSimulator ))
            {
                particleSimulatorsToDestroy.Add( particleSimulator );
            }
        }

        void Update ()
        {
            CalculateObservedRates();
            MoveParticles();
            DoCollisionFreeReactions();
            DoBimolecularReactions();
            Cleanup();
        }

		void CalculateObservedRates ()
        {
            foreach (ReactionWatcher reactionWatcher in collisionFreeReactionWatchers)
            {
                reactionWatcher.CalculateObservedRate();
            }
            foreach (ReactionWatcher reactionWatcher in bimolecularReactionWatchers)
            {
                reactionWatcher.CalculateObservedRate();
            }
        }

        protected virtual void MoveParticles ()
        {
            //UnityEngine.Profiling.Profiler.BeginSample("MoveParticles");
            foreach (ParticleSimulator particleSimulator in particleSimulators)
            {
                particleSimulator.Move( dT );
            }
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        protected virtual void DoCollisionFreeReactions ()
        {
            foreach (CollisionFreeReactionWatcher collisionFreeReactionWatcher in collisionFreeReactionWatchers)
            {
                collisionFreeReactionWatcher.TryReact();
            }
        }

        protected virtual void DoBimolecularReactions ()
        {
            //UnityEngine.Profiling.Profiler.BeginSample("CalculateCollisions");
            for (int i = 0; i < activeParticleSimulators.Count - 1; i++)
            {
                for (int j = i + 1; j < activeParticleSimulators.Count; j++)
                {
                    activeParticleSimulators[i].InteractWith( activeParticleSimulators[j] );
                }
            }
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        protected virtual void Cleanup ()
        {
            foreach (ParticleSimulator particleSimulator in particleSimulatorsToDestroy)
            {
                Destroy( particleSimulator.gameObject );
            }
            particleSimulatorsToDestroy.Clear();
        }

        public virtual List<ParticleSimulator> GetCollidingParticleSimulators (ParticleSimulator particleSimulator, Vector3 newPosition, List<ParticleSimulator> collidingParticleSimulators)
        {
            collidingParticleSimulators.Clear();
            foreach (ParticleSimulator otherParticleSimulator in particleSimulators)
            {
                if (particleSimulator.IsCollidingWith( otherParticleSimulator, newPosition ))
                {
                    collidingParticleSimulators.Add( otherParticleSimulator );
                }
            }
            return collidingParticleSimulators;
        }
    }
}
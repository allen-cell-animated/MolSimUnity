using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reactor : MonoBehaviour 
    {
        [Tooltip( "How many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;
        public Model model;

        public Dictionary<string,ParticlePopulation> populations;
        public List<CollisionFreeReactionWatcher> collisionFreeReactionWatchers = new List<CollisionFreeReactionWatcher>();
        public List<BimolecularReactionWatcher> bimolecularReactionWatchers = new List<BimolecularReactionWatcher>();
        [HideInInspector] public Container container;

        List<ParticleSimulator> particles = new List<ParticleSimulator>();
        List<ParticleSimulator> activeParticles = new List<ParticleSimulator>();
        List<ParticleSimulator> particlesToDestroy = new List<ParticleSimulator>();

        void Start ()
        {
            SetupReactionData();
            CreateContainer();

            populations = new Dictionary<string,ParticlePopulation>();
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
            ParticlePopulation population = obj.AddComponent<ParticlePopulation>();
            population.Init( complexConcentration, this );
            populations.Add( complexConcentration.species, population );
        }

        public virtual ParticlePopulation GetPopulationForComplex (ComplexState complexState)
        {
            if (!populations.ContainsKey( complexState.species ))
            {
                CreatePopulation( new ComplexConcentration( complexState, 0 ) );
            }
            return populations[complexState.species];
        }

        public void RegisterParticle (ParticleSimulator particle)
        {
            if (!particles.Contains( particle ))
            {
                particles.Add( particle );
            }
            if (particle.active && !activeParticles.Contains( particle ))
            {
                activeParticles.Add( particle );
            }
        }

        public void UnregisterParticle (ParticleSimulator particle)
        {
            if (particles.Contains( particle ))
            {
                particles.Remove( particle );
            }
            if (activeParticles.Contains( particle ))
            {
                activeParticles.Remove( particle );
            }
            if (!particlesToDestroy.Contains( particle ))
            {
                particlesToDestroy.Add( particle );
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
            foreach (ParticleSimulator particle in particles)
            {
                particle.Move( World.Instance.dT );
            }
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        protected virtual void DoCollisionFreeReactions ()
        {
            foreach (CollisionFreeReactionWatcher reactionWatcher in collisionFreeReactionWatchers)
            {
                reactionWatcher.TryReact();
            }
        }

        protected virtual void DoBimolecularReactions ()
        {
            //UnityEngine.Profiling.Profiler.BeginSample("CalculateCollisions");
            for (int i = 0; i < activeParticles.Count - 1; i++)
            {
                for (int j = i + 1; j < activeParticles.Count; j++)
                {
                    if (activeParticles[i].IsNear( activeParticles[j] ))
                    {
                        activeParticles[i].InteractWith( activeParticles[j] );
                    }
                }
            }
            //UnityEngine.Profiling.Profiler.EndSample();
        }

        protected virtual void Cleanup ()
        {
            foreach (ParticleSimulator particle in particlesToDestroy)
            {
                Destroy( particle.gameObject );
            }
            particlesToDestroy.Clear();
        }

        public virtual bool WillCollide (ParticleSimulator particle, Vector3 newPosition, out ParticleSimulator[] others)
        {
            List<ParticleSimulator> othersList = new List<ParticleSimulator>();
            foreach (ParticleSimulator other in particles)
            {
                if (particle.IsCollidingWith( other ))
                {
                    othersList.Add( other );
                }
            }
            others = othersList.ToArray();
            return others.Length > 0;
        }
    }
}
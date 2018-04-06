using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reactor : MonoBehaviour 
    {
        [HideInInspector] public Container container;
        public Model model;
        public List<BimolecularReactionSimulator> bimolecularReactionSimulators = new List<BimolecularReactionSimulator>();
        public List<CollisionFreeReactionSimulator> collisionFreeReactionSimulators = new List<CollisionFreeReactionSimulator>();
        public Dictionary<ComplexState,Population> populations = new Dictionary<ComplexState,Population>();
        [Tooltip( "How many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;

        public List<ParticleSimulator> particleSimulators = new List<ParticleSimulator>();
        public List<ParticleSimulator> particleSimulatorsInBimolecularReactions = new List<ParticleSimulator>();
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
            CreateReactionSimulators();
            CreateContainer();

            foreach (ComplexConcentration complex in model.complexes)
            {
                CreatePopulation( complex );
            }
        }

        protected virtual void CreateReactionSimulators ()
        {
            model.Init(); //for prototyping in inspector without writing custom property drawer etc

            foreach (Reaction reaction in model.reactions)
            {
                if (reaction.isBimolecular)
                {
                    bimolecularReactionSimulators.Add( new BimolecularReactionSimulator( reaction ) );
                }
                else
                {
                    collisionFreeReactionSimulators.Add( new CollisionFreeReactionSimulator( reaction ) );
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
            GameObject obj = new GameObject( complexConcentration.complexState.species + "Population" );
            obj.transform.SetParent( transform );

            Population population = obj.AddComponent<Population>();
            population.Init( complexConcentration, this );
            populations.Add( complexConcentration.complexState, population );
        }

        public virtual Population GetPopulation (ComplexState complexState)
        {
            if (!populations.ContainsKey( complexState ))
            {
                CreatePopulation( new ComplexConcentration( complexState, 0 ) );
            }
            return populations[complexState];
        }

        public BimolecularReactionSimulator[] GetRelevantBimolecularReactionSimulators (ComplexState complexState)
        {
            List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                if (reactionSimulator.IsReactant( complexState ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public BimolecularReactionSimulator[] GetRelevantBimolecularReactionSimulators (MoleculeSimulator[] complex)
        {
            List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                if (reactionSimulator.IsReactant( complex ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public CollisionFreeReactionSimulator[] GetRelevantCollisionFreeReactionSimulators (ComplexState complexState)
        {
            List<CollisionFreeReactionSimulator> reactionSimulatorsList = new List<CollisionFreeReactionSimulator>();
            foreach (CollisionFreeReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                if (reactionSimulator.IsReactant( complexState ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public CollisionFreeReactionSimulator[] GetRelevantCollisionFreeReactionSimulators (MoleculeSimulator[] complex)
        {
            List<CollisionFreeReactionSimulator> reactionSimulatorsList = new List<CollisionFreeReactionSimulator>();
            foreach (CollisionFreeReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                if (reactionSimulator.IsReactant( complex ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public void Register (ParticleSimulator particleSimulator)
        {
            if (!particleSimulators.Contains( particleSimulator ))
            {
                particleSimulators.Add( particleSimulator );
            }
            else
            {
                Debug.LogWarning( "Trying to register " + particleSimulator + " but it's already registered!" );
            }

            if (particleSimulator.couldReactOnCollision && !particleSimulatorsInBimolecularReactions.Contains( particleSimulator ))
            {
                particleSimulatorsInBimolecularReactions.Add( particleSimulator );
            }
        }

        public void Unregister (ParticleSimulator particleSimulator)
        {
            if (particleSimulators.Contains( particleSimulator ))
            {
                particleSimulators.Remove( particleSimulator );
            }
            else
            {
                Debug.LogWarning( "Trying to remove " + particleSimulator + " but it's not registered!" );
            }

            if (particleSimulatorsInBimolecularReactions.Contains( particleSimulator ))
            {
                particleSimulatorsInBimolecularReactions.Remove( particleSimulator );
            }

            if (!particleSimulatorsToDestroy.Contains( particleSimulator ))
            {
                particleSimulatorsToDestroy.Add( particleSimulator );
            }
        }

        public void ParticleSimulatorChangedCouldReactOnCollisionState (ParticleSimulator particleSimulator)
        {
            if (particleSimulator.couldReactOnCollision)
            {
                if (particleSimulatorsInBimolecularReactions.Contains( particleSimulator ))
                {
                    particleSimulatorsInBimolecularReactions.Remove( particleSimulator );
                }
            }
            else
            {
                if (!particleSimulatorsInBimolecularReactions.Contains( particleSimulator ))
                {
                    particleSimulatorsInBimolecularReactions.Add( particleSimulator );
                }
            }
        }

        void Update ()
        {
            #if UNITY_EDITOR
            if (Input.GetKeyDown( KeyCode.X ))
            {
                ObjectStateTests.StateOfReactorIsCorrect( this );
            }
            #endif

                //UnityEngine.Profiling.Profiler.BeginSample("MoveParticles");
            MoveParticles();
                //UnityEngine.Profiling.Profiler.EndSample();

            CalculateObservedRates();

                //UnityEngine.Profiling.Profiler.BeginSample("CollisionFreeReactions");
            DoCollisionFreeReactions();
                //UnityEngine.Profiling.Profiler.EndSample();

                //UnityEngine.Profiling.Profiler.BeginSample("BimolecularReactions");
            DoBimolecularReactions();
                //UnityEngine.Profiling.Profiler.EndSample();

            Cleanup();
        }

		void CalculateObservedRates ()
        {
            foreach (ReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                reactionSimulator.CalculateObservedRate();
            }
            foreach (ReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                reactionSimulator.CalculateObservedRate();
            }
        }

        protected virtual void MoveParticles ()
        {
            foreach (ParticleSimulator particleSimulator in particleSimulators)
            {
                particleSimulator.Move( dT );
            }
        }

        public virtual bool WillCollide (ParticleSimulator particleSimulator, Vector3 newPosition)
        {
            foreach (ParticleSimulator otherParticleSimulator in particleSimulators)
            {
                if (particleSimulator.IsCollidingWith( otherParticleSimulator, newPosition ))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void DoCollisionFreeReactions ()
        {
            collisionFreeReactionSimulators.Shuffle();
            foreach (CollisionFreeReactionSimulator collisionFreeReactionSimulator in collisionFreeReactionSimulators)
            {
                collisionFreeReactionSimulator.TryReact();
            }
        }

        protected virtual void DoBimolecularReactions ()
        {
            particleSimulatorsInBimolecularReactions.Shuffle();
            for (int i = 0; i < particleSimulatorsInBimolecularReactions.Count - 1; i++)
            {
                for (int j = i + 1; j < particleSimulatorsInBimolecularReactions.Count; j++)
                {
                    particleSimulatorsInBimolecularReactions[i].InteractWith( particleSimulatorsInBimolecularReactions[j] );
                }
            }
        }

        protected virtual void Cleanup ()
        {
            foreach (ParticleSimulator particleSimulator in particleSimulatorsToDestroy)
            {
                if (particleSimulator.GetComponent<MoleculeSimulator>())
                {
                    Destroy( particleSimulator );
                }
                else
                {
                    Destroy( particleSimulator.gameObject );
                }
            }
            particleSimulatorsToDestroy.Clear();
        }
    }
}
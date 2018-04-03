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
        public Dictionary<ComplexState,ParticlePopulation> particlePopulations = new Dictionary<ComplexState,ParticlePopulation>();
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

            ParticlePopulation particlePopulation = obj.AddComponent<ParticlePopulation>();
            particlePopulation.Init( complexConcentration, this );
            particlePopulations.Add( complexConcentration.complexState, particlePopulation );
        }

        public virtual ParticlePopulation GetPopulationForComplex (ComplexState complexState)
        {
            if (!particlePopulations.ContainsKey( complexState ))
            {
                CreatePopulation( new ComplexConcentration( complexState, 0 ) );
            }
            return particlePopulations[complexState];
        }

        public BimolecularReactionSimulator[] GetRelevantBimolecularReactionSimulators (ComplexState complexState)
        {
            List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                if (reactionSimulator.ComplexIsReactant( complexState ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public BimolecularReactionSimulator[] GetRelevantBimolecularReactionSimulators (MoleculeSimulator[] moleculeSimulators)
        {
            List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                if (reactionSimulator.ComplexIsReactant( moleculeSimulators ))
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
                if (reactionSimulator.ComplexIsReactant( complexState ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public CollisionFreeReactionSimulator[] GetRelevantCollisionFreeReactionSimulators (MoleculeSimulator[] moleculeSimulators)
        {
            List<CollisionFreeReactionSimulator> reactionSimulatorsList = new List<CollisionFreeReactionSimulator>();
            foreach (CollisionFreeReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                if (reactionSimulator.ComplexIsReactant( moleculeSimulators ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public void RegisterParticleSimulator (ParticleSimulator particleSimulator)
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

        public void UnregisterParticleSimulator (ParticleSimulator particleSimulator)
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
            if (Input.GetKeyDown( KeyCode.X ))
            {
                ObjectStateTests.StateOfReactorIsCorrect( this );
            }

            MoveParticles();

            CalculateObservedRates();
            DoCollisionFreeReactions();
            DoBimolecularReactions();

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
            UnityEngine.Profiling.Profiler.BeginSample("MoveParticles");
            foreach (ParticleSimulator particleSimulator in particleSimulators)
            {
                particleSimulator.Move( dT );
            }
            UnityEngine.Profiling.Profiler.EndSample();
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
            UnityEngine.Profiling.Profiler.BeginSample("CollisionFreeReactions");
            foreach (CollisionFreeReactionSimulator collisionFreeReactionSimulator in collisionFreeReactionSimulators)
            {
                collisionFreeReactionSimulator.TryReact();
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        protected virtual void DoBimolecularReactions ()
        {
            UnityEngine.Profiling.Profiler.BeginSample("BimolecularReactions");
            particleSimulatorsInBimolecularReactions.Shuffle();
            for (int i = 0; i < particleSimulatorsInBimolecularReactions.Count - 1; i++)
            {
                for (int j = i + 1; j < particleSimulatorsInBimolecularReactions.Count; j++)
                {
                    particleSimulatorsInBimolecularReactions[i].InteractWith( particleSimulatorsInBimolecularReactions[j] );
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
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

        static GameObject _flashPrefab;
        static GameObject flashPrefab
        {
            get
            {
                if (_flashPrefab == null)
                {
                    _flashPrefab = Resources.Load( "Flash" ) as GameObject;
                }
                return _flashPrefab;
            }
        }

        public static void ShowFlash (Transform parent)
        {
            if (flashPrefab != null)
            {
                Instantiate( flashPrefab, parent.position, Quaternion.identity, parent );
            }
        }
    }
}
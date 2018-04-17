using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class Reactor : MonoBehaviour 
    {
        [HideInInspector] public Container container;
        [HideInInspector] public Spawner spawner;
        public ModelDef modelDef;
        public List<BimolecularReactionSimulator> bimolecularReactionSimulators = new List<BimolecularReactionSimulator>();
        public List<CollisionFreeReactionSimulator> collisionFreeReactionSimulators = new List<CollisionFreeReactionSimulator>();
        [Tooltip( "How many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;

        public List<Mover> movers = new List<Mover>();
        public List<Complex> complexes = new List<Complex>();
        protected List<Mover> moversToDestroy = new List<Mover>();
        protected List<Complex> complexesToDestroy = new List<Complex>();

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
            SpawnComplexes();
        }

        protected virtual void CreateReactionSimulators ()
        {
            modelDef.Init(); //for prototyping in inspector without writing custom property drawer etc

            foreach (ReactionDef reactionDef in modelDef.reactionDefs)
            {
                if (reactionDef.isBimolecular)
                {
                    bimolecularReactionSimulators.Add( new BimolecularReactionSimulator( reactionDef, this ) );
                }
                else
                {
                    collisionFreeReactionSimulators.Add( new CollisionFreeReactionSimulator( reactionDef, this ) );
                }
            }
        }

        protected virtual void CreateContainer ()
        {
            container = gameObject.AddComponent<Container>();
            container.Init( modelDef.scale, modelDef.containerVolume, periodicBoundary );
        }

        protected virtual void SpawnComplexes ()
        {
            spawner = gameObject.AddComponent<Spawner>();
            foreach (ComplexConcentration complex in modelDef.complexes)
            {
                spawner.SpawnComplexes( complex, this );
            }
        }

        public BimolecularReactionSimulator[] GetRelevantBimolecularReactionSimulators (ComplexSnapshot complexSnapshot)
        {
            List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                if (reactionSimulator.IsReactant( complexSnapshot ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public BimolecularReactionSimulator[] GetRelevantBimolecularReactionSimulators (Molecule[] molecules)
        {
            List<BimolecularReactionSimulator> reactionSimulatorsList = new List<BimolecularReactionSimulator>();
            foreach (BimolecularReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                if (reactionSimulator.IsReactant( molecules ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public CollisionFreeReactionSimulator[] GetRelevantCollisionFreeReactionSimulators (ComplexSnapshot complexSnapshot)
        {
            List<CollisionFreeReactionSimulator> reactionSimulatorsList = new List<CollisionFreeReactionSimulator>();
            foreach (CollisionFreeReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                if (reactionSimulator.IsReactant( complexSnapshot ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public CollisionFreeReactionSimulator[] GetRelevantCollisionFreeReactionSimulators (Molecule[] molecules)
        {
            List<CollisionFreeReactionSimulator> reactionSimulatorsList = new List<CollisionFreeReactionSimulator>();
            foreach (CollisionFreeReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                if (reactionSimulator.IsReactant( molecules ))
                {
                    reactionSimulatorsList.Add( reactionSimulator );
                }
            }
            return reactionSimulatorsList.ToArray();
        }

        public void RegisterMover (Mover mover)
        {
            if (!movers.Contains( mover ))
            {
                movers.Add( mover );
            }
        }

        public void UnregisterParticle (Mover mover)
        {
            if (movers.Contains( mover ))
            {
                movers.Remove( mover );
            }

            if (!moversToDestroy.Contains( mover ))
            {
                moversToDestroy.Add( mover );
            }
        }

        public void RegisterComplex (Complex complex)
        {
            if (complex.couldReactOnCollision)
            {
                if (!complexes.Contains( complex ))
                {
                    complexes.Add( complex );
                }
            }
        }

        public void UnregisterComplex (Complex complex)
        {
            if (complexes.Contains( complex ))
            {
                complexes.Remove( complex );
            }

            if (!complexesToDestroy.Contains( complex ))
            {
                complexesToDestroy.Add( complex );
            }
        }

        public void ComplexChangedCouldReactOnCollisionState (Complex complex)
        {
            if (complex.couldReactOnCollision)
            {
                if (complexes.Contains( complex ))
                {
                    complexes.Remove( complex );
                }
            }
            else
            {
                if (!complexes.Contains( complex ))
                {
                    complexes.Add( complex );
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

                //UnityEngine.Profiling.Profiler.BeginSample("CollisionFreeReactions");
            DoCollisionFreeReactions();
                //UnityEngine.Profiling.Profiler.EndSample();

                //UnityEngine.Profiling.Profiler.BeginSample("BimolecularReactions");
            DoBimolecularReactions();
                //UnityEngine.Profiling.Profiler.EndSample();

            Cleanup();
        }

        protected virtual void MoveParticles ()
        {
            foreach (Mover mover in movers)
            {
                mover.Move( dT );
            }
        }

        public virtual bool WillCollide (Mover mover, Vector3 newPosition)
        {
            foreach (Mover otherMover in movers)
            {
                if (mover.IsCollidingWith( otherMover, newPosition ))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void DoCollisionFreeReactions ()
        {
            foreach (ReactionSimulator reactionSimulator in collisionFreeReactionSimulators)
            {
                reactionSimulator.CalculateObservedRate();
            }

            collisionFreeReactionSimulators.Shuffle();
            foreach (CollisionFreeReactionSimulator collisionFreeReactionSimulator in collisionFreeReactionSimulators)
            {
                collisionFreeReactionSimulator.TryReact();
            }
        }

        protected virtual void DoBimolecularReactions ()
        {
            foreach (ReactionSimulator reactionSimulator in bimolecularReactionSimulators)
            {
                reactionSimulator.CalculateObservedRate();
            }

            Complex complex;
            int start = complexes.GetRandomIndex();
            for (int i = 0; i < complexes.Count; i++)
            {
                complex = complexes[(start + i) % complexes.Count];
                for (int j = i + 1; j < complexes.Count; j++)
                {
                    complex.InteractWith( complexes[(start + j) % complexes.Count] );
                }
            }
        }

        protected virtual void Cleanup ()
        {
            foreach (Complex complex in complexesToDestroy)
            {
                Destroy( complex );
            }
            complexesToDestroy.Clear();

            foreach (Mover mover in moversToDestroy)
            {
                Destroy( mover.gameObject );
            }
            moversToDestroy.Clear();
        }
    }
}
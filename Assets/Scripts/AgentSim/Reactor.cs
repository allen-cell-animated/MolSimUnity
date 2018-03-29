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
        public Dictionary<string,ParticlePopulation> particlePopulations = new Dictionary<string,ParticlePopulation>();
        [Tooltip( "How many attempts to move particles each frame? collisions and boundaries can cause move to fail" )]
        public int maxMoveAttempts = 20;
        [Tooltip( "Reflect particle to other side of container when it runs into a wall?" )]
        public bool periodicBoundary = true;

        public List<ParticleSimulator> particleSimulators = new List<ParticleSimulator>();
        public List<ParticleSimulator> activeParticleSimulators = new List<ParticleSimulator>();
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

        public void ParticleSimulatorChangedActiveState (ParticleSimulator particleSimulator)
        {
            if (particleSimulator.active)
            {
                if (activeParticleSimulators.Contains( particleSimulator ))
                {
                    activeParticleSimulators.Remove( particleSimulator );
                }
            }
            else
            {
                if (!activeParticleSimulators.Contains( particleSimulator ))
                {
                    activeParticleSimulators.Add( particleSimulator );
                }
            }
        }

        void Update ()
        {
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
            for (int i = 0; i < activeParticleSimulators.Count - 1; i++)
            {
                for (int j = i + 1; j < activeParticleSimulators.Count; j++)
                {
                    activeParticleSimulators[i].InteractWith( activeParticleSimulators[j] );
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
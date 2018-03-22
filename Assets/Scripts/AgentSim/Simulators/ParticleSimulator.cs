using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public bool canMove = true;
        public ParticlePopulation population;
        public List<MoleculeSimulator> moleculeSimulators;
        protected List<ParticleSimulator> collidingParticleSimulators = new List<ParticleSimulator>();

        public bool active
        {
            get
            {
                foreach (MoleculeSimulator moleculeSimulator in moleculeSimulators)
                {
                    if (moleculeSimulator.active)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        float collisionRadius
        {
            get 
            {
                return population.collisionRadius;
            }
        }

        float interactionRadius
        {
            get 
            {
                return population.interactionRadius;
            }
        }

        public virtual void Init (List<MoleculeSimulator> _moleculeSimulators, ParticlePopulation _particlePopulation)
        {
            population = _particlePopulation;
            moleculeSimulators = _moleculeSimulators;
            population.reactor.RegisterParticleSimulator( this );
        }

        public virtual void Move (float dTime)
        {
            if (canMove)
            {
                int i = 0;
                bool moved = false;
                while (!moved && i < population.reactor.maxMoveAttempts)
                {
                    moved = MoveRandomStep( dTime );
                    i++;
                }
                transform.position += GetExitDirection();

                RotateRandomly( dTime );
            }
        }

        protected virtual bool MoveRandomStep (float dTime)
        {
            Vector3 moveStep = 2E3f * GetDisplacement( dTime ) * Random.onUnitSphere;

            Vector3 collisionToCenter;
            if (population.reactor.container.IsOutOfBounds( transform.position + moveStep, out collisionToCenter ))
            {
                if (population.reactor.periodicBoundary)
                {
                    ReflectPeriodically( collisionToCenter );
                    return true;
                }
                return false;
            }

            if (population.reactor.GetCollidingParticleSimulators( this, transform.position + moveStep, collidingParticleSimulators ).Count > 0)
            {
                return false;
            }

            transform.position += moveStep;
            return true;
        }

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( transform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, population.reactor.container.boundaryLayer ))
            {
                transform.position = info.point - collisionToCenter.normalized;
            }
        }

        protected virtual void RotateRandomly (float dTime)
        {
            transform.rotation *= Quaternion.Euler( 4E4f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        protected float GetDisplacement (float dTime)
        {
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( population.complexState.diffusionCoefficient * dTime ) );
        }

        protected virtual Vector3 GetExitDirection ()
        {
            int n = 0;
            Vector3 exitVector = Vector3.zero;
            foreach (ParticleSimulator other in collidingParticleSimulators)
            {
                exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                n++;
            }
            return exitVector.normalized;
        }

        public bool IsCollidingWith (ParticleSimulator other, Vector3 newPosition)
        {
            return other != this 
                && Vector3.Distance( newPosition, other.transform.position ) < collisionRadius + other.collisionRadius;
        }

        public virtual void InteractWith (ParticleSimulator other)
        {
            if (IsNear( other ))
            {
                foreach (MoleculeSimulator moleculeSimulator in moleculeSimulators)
                {
                    foreach (MoleculeSimulator otherMoleculeSimulator in other.moleculeSimulators)
                    {
                        if (moleculeSimulator.InteractWith( otherMoleculeSimulator ))
                        {
                            return;
                        }
                    }
                }
            }
        }

        bool IsNear (ParticleSimulator other)
        {
            return other != this 
                && Vector3.Distance( transform.position, other.transform.position ) < interactionRadius + other.interactionRadius;
        }

        public MoleculeSimulator[] GetMoleculesAtEndOfBond (BindingSiteSimulator bindingSiteSimulator)
        {
            // TODO
            return new MoleculeSimulator[]{bindingSiteSimulator.moleculeSimulator};
        }

        public void RemoveMolecule (MoleculeSimulator moleculeSimulator)
        {
            if (moleculeSimulators.Contains( moleculeSimulator ))
            {
                moleculeSimulators.Remove( moleculeSimulator );
            }
            if (moleculeSimulators.Count == 0)
            {
                population.reactor.UnregisterParticleSimulator( this );
            }
        }
	}
}

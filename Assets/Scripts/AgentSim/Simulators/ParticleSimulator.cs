using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public ParticlePopulation population;
        public MoleculeSimulator[] moleculeSimulators;

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

        public virtual void Init (MoleculeSimulator[] _moleculeSimulators, ParticlePopulation _particlePopulation)
        {
            population = _particlePopulation;
            moleculeSimulators = _moleculeSimulators;
            population.reactor.RegisterParticleSimulator( this );
        }

        public virtual void Move (float dTime)
        {
            int i = 0;
            bool moved = false;
            while (!moved && i < population.reactor.maxMoveAttempts)
            {
                moved = MoveRandomStep( dTime );
                i++;
            }
            RotateRandomly( dTime );
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

            if (population.reactor.WillCollide( this, transform.position + moveStep ))
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
                    if (moleculeSimulator != null)
                    {
                        foreach (MoleculeSimulator otherMoleculeSimulator in other.moleculeSimulators)
                        {
                            if (otherMoleculeSimulator != null)
                            {
                                if (moleculeSimulator.InteractWith( otherMoleculeSimulator ))
                                {
                                    return;
                                }
                            }
                            else 
                            {
                                Debug.Log( "NULL molecule in OTHER particle " + name );
                            }
                        }
                    }
                    else 
                    {
                        Debug.Log( "NULL molecule in particle " + name );
                    }
                }
            }
        }

        bool IsNear (ParticleSimulator other)
        {
            return other != this 
                && Vector3.Distance( transform.position, other.transform.position ) < interactionRadius + other.interactionRadius;
        }

        public MoleculeSimulator[] GetComplexAtEndOfBond (BindingSiteSimulator bindingSiteSimulator)
        {
            // TODO trace complex
            return new MoleculeSimulator[]{bindingSiteSimulator.moleculeSimulator};
        }

        public void RemoveMoleculeSimulator (MoleculeSimulator moleculeSimulatorToRemove)
        {
            MoleculeSimulator[] newMoleculeSimulators = new MoleculeSimulator[moleculeSimulators.Length - 1];
            int j = 0;
            for (int i = 0; i < moleculeSimulators.Length; i++)
            {
                if (moleculeSimulators[i] != moleculeSimulatorToRemove)
                {
                    newMoleculeSimulators[j] = moleculeSimulators[i];
                    j++;
                }
            }
            moleculeSimulators = newMoleculeSimulators;

            if (moleculeSimulators.Length == 0)
            {
                population.reactor.UnregisterParticleSimulator( this );
            }
        }
	}
}

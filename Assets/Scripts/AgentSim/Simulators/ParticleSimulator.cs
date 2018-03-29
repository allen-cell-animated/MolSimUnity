using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public ParticlePopulation population;
        public MoleculeSimulator[] moleculeSimulators;

        Transform _theTransform;
        public Transform theTransform
        {
            get
            {
                if (_theTransform == null)
                {
                    _theTransform = transform;
                }
                return _theTransform;
            }
        }

        public bool active;

        public void UpdateActive (bool aMoleculeIsActive = false)
        {
            bool newActive = aMoleculeIsActive ? true : GetActive();

            if (newActive != active)
            {
                active = newActive;
                population.reactor.ParticleSimulatorChangedActiveState( this );
            }
        }

        bool GetActive ()
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
            active = GetActive();
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

            if (!population.reactor.container.IsInBounds( theTransform.position + moveStep ))
            {
                if (population.reactor.periodicBoundary)
                {
                    ReflectPeriodically( population.reactor.container.theTransform.position - (theTransform.position + moveStep) );
                    return true;
                }
                return false;
            }

            if (population.reactor.WillCollide( this, theTransform.position + moveStep ))
            {
                return false;
            }

            theTransform.position += moveStep;
            return true;
        }

        protected virtual void ReflectPeriodically (Vector3 collisionToCenter)
        {
            RaycastHit info;
            if (Physics.Raycast( theTransform.position, collisionToCenter.normalized, out info, 2f * collisionToCenter.magnitude, population.reactor.container.boundaryLayer ))
            {
                theTransform.position = info.point - collisionToCenter.normalized;
            }
        }

        protected virtual void RotateRandomly (float dTime)
        {
            theTransform.rotation *= Quaternion.Euler( 4E4f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        protected float GetDisplacement (float dTime)
        {
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( population.complexState.diffusionCoefficient * dTime ) );
        }

        public bool IsCollidingWith (ParticleSimulator other, Vector3 newPosition)
        {
            return other != this 
                && Vector3.Distance( newPosition, other.theTransform.position ) < collisionRadius + other.collisionRadius;
        }

        public virtual void InteractWith (ParticleSimulator other)
        {
            if (IsNear( other ))
            {
                foreach (MoleculeSimulator moleculeSimulator in moleculeSimulators)
                {
                    if (moleculeSimulator != null && moleculeSimulator.active)
                    {
                        foreach (MoleculeSimulator otherMoleculeSimulator in other.moleculeSimulators)
                        {
                            if (otherMoleculeSimulator != null && otherMoleculeSimulator.active)
                            {
                                if (moleculeSimulator.InteractWith( otherMoleculeSimulator ))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IsNear (ParticleSimulator other)
        {
            return other != this 
                && Vector3.Distance( theTransform.position, other.theTransform.position ) < interactionRadius + other.interactionRadius;
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

		public override string ToString()
		{
            return "ParticleSimulator " + name;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public Population population;
        public MoleculeSimulator[] complex;

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

        public bool couldReactOnCollision;

        bool GetCouldReactOnCollision ()
        {
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                if (moleculeSimulator.couldReactOnCollision)
                {
                    return true;
                }
            }
            return false;
        }

        float _collisionRadius = -1f;
        float collisionRadius
        {
            get 
            {
                if (_collisionRadius < 0)
                {
                    float d, maxD = 0;
                    foreach (MoleculeSimulator moleculeSimulator in complex)
                    {
                        d = Vector3.Distance( theTransform.position, moleculeSimulator.theTransform.position ) + moleculeSimulator.collisionRadius;
                        if (d > maxD)
                        {
                            maxD = d;
                        }
                    }
                    _collisionRadius = maxD;
                }
                return _collisionRadius;
            }
        }

        float _interactionRadius = -1f;
        float interactionRadius
        {
            get 
            {
                if (_interactionRadius < 0)
                {
                    float d, maxD = 0;
                    foreach (MoleculeSimulator moleculeSimulator in complex)
                    {
                        d = Vector3.Distance( theTransform.position, moleculeSimulator.theTransform.position ) + moleculeSimulator.interactionRadius;
                        if (d > maxD)
                        {
                            maxD = d;
                        }
                    }
                    _interactionRadius = maxD;
                }
                return _interactionRadius;
            }
        }

        public virtual void Init (MoleculeSimulator[] _complex, Population _population)
        {
            population = _population;
            complex = _complex;
            couldReactOnCollision = GetCouldReactOnCollision();
            population.reactor.Register( this );
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
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( population.diffusionCoefficient * dTime ) );
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
                complex.Shuffle();
                foreach (MoleculeSimulator moleculeSimulator in complex)
                {
                    if (moleculeSimulator != null && moleculeSimulator.couldReactOnCollision)
                    {
                        other.complex.Shuffle();
                        foreach (MoleculeSimulator otherMoleculeSimulator in other.complex)
                        {
                            if (otherMoleculeSimulator != null && otherMoleculeSimulator.couldReactOnCollision)
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

        public virtual void UpdateReactions ()
        {
            BimolecularReactionSimulator[] relevantBimolecularSimulators = population.reactor.GetRelevantBimolecularReactionSimulators( complex );
            CollisionFreeReactionSimulator[] relevantCollisionFreeSimulators = population.reactor.GetRelevantCollisionFreeReactionSimulators( complex );
            foreach (MoleculeSimulator moleculeSimulator in complex)
            {
                moleculeSimulator.UpdateReactions( relevantBimolecularSimulators, relevantCollisionFreeSimulators );
            }
            UpdateCouldReactOnCollision();
        }

        public void Remove (MoleculeSimulator moleculeSimulatorToRemove)
        {
            if (complex.Length < 2)
            {
                population.reactor.Unregister( this );
            }
            else
            {
                MoleculeSimulator[] newComplex = new MoleculeSimulator[complex.Length - 1];
                int j = 0;
                for (int i = 0; i < complex.Length; i++)
                {
                    if (complex[i] != moleculeSimulatorToRemove)
                    {
                        newComplex[j] = complex[i];
                        j++;
                    }
                }
                complex = newComplex;
            }
        }

        public void UpdateCouldReactOnCollision ()
        {
            bool newCouldReactOnCollision = GetCouldReactOnCollision();
            if (newCouldReactOnCollision != couldReactOnCollision)
            {
                couldReactOnCollision = newCouldReactOnCollision;
                population.reactor.ParticleSimulatorChangedCouldReactOnCollisionState( this );
            }
        }

		public override string ToString ()
		{
            return "ParticleSimulator " + name;
		}
	}
}

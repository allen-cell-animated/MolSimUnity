using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ParticleSimulator : MonoBehaviour 
    {
        public bool canMove = true;
        public ParticlePopulation population;
        public List<MoleculeSimulator> molecules;
        protected ParticleSimulator[] collidingMolecules;

        public bool active
        {
            get
            {
                foreach (MoleculeSimulator molecule in molecules)
                {
                    if (molecule.active)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public float collisionRadius
        {
            get 
            {
                return population.collisionRadius;
            }
        }

        public float interactionRadius
        {
            get 
            {
                return population.interactionRadius;
            }
        }

        public virtual void Init (ParticlePopulation _population)
        {
            population = _population;
            population.reactor.RegisterParticle( this );
        }

        void Update ()
        {
            if (canMove)
            {
                transform.position += GetExitDirection( collidingMolecules );
            }
        }

        protected virtual Vector3 GetExitDirection (ParticleSimulator[] collidingParticles)
        {
            if (collidingParticles != null)
            {
                int n = 0;
                Vector3 exitVector = Vector3.zero;
                foreach (ParticleSimulator other in collidingParticles)
                {
                    exitVector = (n * exitVector + (transform.position - other.transform.position)) / (n + 1f);
                    n++;
                }
                return exitVector.normalized;
            }
            return Vector3.zero;
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

            if (population.reactor.WillCollide( this, transform.position + moveStep, out collidingMolecules ))
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
            return Helpers.SampleExponentialDistribution( Time.deltaTime * Mathf.Sqrt( population.diffusionCoefficient * dTime ) );
        }

        public virtual void ToggleMotion (bool move)
        {
            canMove = move;
        }

        public bool IsCollidingWith (ParticleSimulator other)
        {
            return other != this 
                && Vector3.Distance( transform.position, other.transform.position ) < collisionRadius + other.collisionRadius;
        }

        public bool IsNear (ParticleSimulator other)
        {
            return other != this 
                && Vector3.Distance( transform.position, other.transform.position ) < interactionRadius + other.interactionRadius;
        }

        public virtual void InteractWith (ParticleSimulator other)
        {
            foreach (MoleculeSimulator molecule in molecules)
            {
                foreach (MoleculeSimulator otherMolecule in other.molecules)
                {
                    if (molecule.InteractWith( otherMolecule ))
                    {
                        return;
                    }
                }
            }
        }

        public MoleculeSimulator[] GetMoleculesAtEndOfBond (BindingSiteSimulator bindingSite)
        {
            // TODO
            return new MoleculeSimulator[]{bindingSite.molecule};
        }

        public void RemoveMolecule (MoleculeSimulator molecule)
        {
            if (molecules.Contains( molecule ))
            {
                molecules.Remove( molecule );
            }
            if (molecules.Count == 0)
            {
                population.reactor.UnregisterParticle( this );
            }
        }
	}
}

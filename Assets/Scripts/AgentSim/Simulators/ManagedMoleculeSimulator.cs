using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that don't use the physics engine, Container detects collisions instead
    public class ManagedMoleculeSimulator : MoleculeSimulator 
    {
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

        public override void Init (MoleculeState moleculeState, MoleculePopulation _population)
        {
            base.Init( moleculeState, _population );
            population.reactor.container.RegisterMolecule( this );
        }

        public override void SimulateFor (float dTime)
        {
            if (!CheckBind())
            {
                transform.position += GetExitDirection();
            }
            collidingMolecules.Clear();
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
                if (population.reactor.container.periodicBoundary)
                {
                    ReflectPeriodically( collisionToCenter );
                    return true;
                }
                return false;
            }

            ManagedMoleculeSimulator[] others;
            if (population.reactor.container.WillCollide( this, transform.position + moveStep, out others ))
            {
                SaveCollidingSimulators( others );
                return false;
            }

            transform.position += moveStep;
            return true;
        }

        protected virtual void RotateRandomly (float dTime)
        {
            transform.rotation *= Quaternion.Euler( 2E4f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        protected override void ToggleMotion (bool move)
        {
            canMove = move;
        }
    }
}
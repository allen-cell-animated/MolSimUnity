using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that don't use the physics engine, Container detects collisions instead
    public class ManagedParticleSimulator : ParticleSimulator 
    {
        public float radius;

        protected override void DoAdditionalInit ()
        {
            radius = population.molecule.radius;
            population.reactor.container.RegisterSimulator( this );
        }

        public override void SimulateFor (float dTime)
        {
            if (!CheckBind())
            {
                transform.position += GetExitDirection();
            }
            collidingSimulators.Clear();

            int i = 0;
            bool moved = false;
            while (!moved && i < population.reactor.maxMoveAttempts)
            {
                moved = MoveRandomStep( dTime );
                i++;
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
                else
                {
                    return false;
                }
            }
            else 
            {
                ManagedParticleSimulator[] others;
                if (population.reactor.container.WillCollide( this, transform.position + moveStep, out others ))
                {
                    SaveCollidingSimulators( others );
                    return false;
                }
                else
                {
                    transform.position += moveStep;
                    return true;
                }
            }
        }
    }
}
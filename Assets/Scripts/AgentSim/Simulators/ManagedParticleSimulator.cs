using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that don't use the physics engine, Container detects collisions instead
    public class ManagedParticleSimulator : ParticleSimulator 
    {
        [Tooltip( "[Agent's scale] meters" )]
        public float radius;
        [Tooltip( "How many tries to move each frame, collisions or bounds can cause move to fail" )]
        public int maxMoveAttempts = 20;

        protected override void Setup ()
        {
            agent.container.RegisterSimulator( this );
        }

        public override void SimulateFor (float dTime)
        {
            Vector3 exitVector;
            if (!CheckBind( out exitVector ))
            {
                transform.position += exitVector;
            }

            int i = 0;
            bool moved = false;
            while (!moved && i < maxMoveAttempts)
            {
                moved = MoveRandomStep( dTime );
                i++;
            }
        }

        protected virtual bool MoveRandomStep (float dTime)
        {
            Vector3 moveStep = 2E3f * GetDisplacement( dTime ) * Random.onUnitSphere;

            Vector3 collisionToCenter;
            if (agent.container.IsOutOfBounds( transform.position + moveStep, out collisionToCenter ))
            {
                if (agent.container.periodicBoundary)
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
                if (agent.container.WillCollide( this, transform.position + moveStep, out others ))
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
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    // Directly simulated particles that don't use the physics engine, Container detects collisions instead
    public class ManagedMoleculeSimulator : MoleculeSimulator 
    {
        [SerializeField] protected ManagedMoleculeSimulator[] collidingMolecules;

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

        public override void Init (MoleculePopulation _population, MoleculeState moleculeState = null)
        {
            base.Init( _population, moleculeState );
            population.reactor.container.RegisterMolecule( this );
        }

        public override void SimulateFor (float dTime)
        {
            if (canMove)
            {
                Vector3 exit = GetExitDirection( (MoleculeSimulator[])collidingMolecules );
                if (exit.magnitude > 0)
                {
                    if (log)
                    {
                        GameObject.CreatePrimitive(PrimitiveType.Sphere).transform.position = transform.position + exit;
                    }
                    transform.position += exit;
                }
            }
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

            if (population.reactor.container.WillCollide( this, transform.position + moveStep, out collidingMolecules ))
            {
                return false;
            }

            transform.position += moveStep;
            return true;
        }

        protected virtual void RotateRandomly (float dTime)
        {
            transform.rotation *= Quaternion.Euler( 2E4f * GetDisplacement( dTime ) * Random.onUnitSphere );
        }

        public override void ToggleMotion (bool move)
        {
            canMove = move;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICS.AgentSim
{
    public class ComplexSimulator : MonoBehaviour 
    {
        public Population population;
        public ParticleSimulator particleSimulator;
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

        public virtual void Init (MoleculeSimulator[] _complex, Population _population, ParticleSimulator _particleSimulator)
        {
            population = _population;
            particleSimulator = _particleSimulator;
            complex = _complex;
            couldReactOnCollision = GetCouldReactOnCollision();
            population.reactor.RegisterComplex( this );
        }

        public virtual void InteractWith (ComplexSimulator other)
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

        bool IsNear (ComplexSimulator other)
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
                population.reactor.UnregisterComplex( this );
                particleSimulator.Destroy();
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

        protected void UpdateCouldReactOnCollision ()
        {
            bool newCouldReactOnCollision = GetCouldReactOnCollision();
            if (newCouldReactOnCollision != couldReactOnCollision)
            {
                couldReactOnCollision = newCouldReactOnCollision;
                population.reactor.ComplexChangedCouldReactOnCollisionState( this );
            }
        }

        public override string ToString ()
        {
            return "ComplexSimulator " + name;
        }
    }
}
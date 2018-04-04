﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ObjectStateTests : AgentSimTests
{
    int numberOfTimesToCheck = 5;

    float waitTime
    {
        get
        {
            return 2f + 3f * Random.value;
        }
    }

    [UnityTest]
    public IEnumerator MoleculesBindCorrectly ()
    {
        Reactor reactor = CreateReactor( "ABBindReaction" );

        yield return new WaitForEndOfFrame();

        AssertIsTrue( StateOfReactorIsCorrect( reactor ) );

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
        }

        DestroyReactor( reactor );
    }

    [UnityTest]
    public IEnumerator MoleculesReleaseCorrectly ()
    {
        Reactor reactor = CreateReactor( "ABReleaseReaction" );

        yield return new WaitForEndOfFrame();

        AssertIsTrue( StateOfReactorIsCorrect( reactor ) );

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
        }

        DestroyReactor( reactor );
    }

    [UnityTest]
    public IEnumerator MoleculesBindAndReleaseCorrectly ()
    {
        Reactor reactor = CreateReactor( "ABReversibleReaction" );

        yield return new WaitForEndOfFrame();

        AssertIsTrue( StateOfReactorIsCorrect( reactor ) );

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
        }

        DestroyReactor( reactor );
    }

    public static bool StateOfReactorIsCorrect (Reactor reactor)
    {
        //particle simulators
        foreach (ParticleSimulator particleSimulator in reactor.particleSimulators)
        {
            if (particleSimulator == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null ParticleSimulator" ); }
                return false;
            }
            if (ParticleSimulatorReallyIsBimolecularReactant( particleSimulator ))
            {
                bool found = false;
                foreach (ParticleSimulator bimolecularReactantParticleSimulator in reactor.particleSimulatorsInBimolecularReactions)
                {
                    if (particleSimulator == bimolecularReactantParticleSimulator)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (debug) { Debug.Log( particleSimulator + " is a bimolecular reactant but isn't registered as such in the Reactor" ); }
                    return false;
                }
            }
            int count = 0;
            foreach (ParticleSimulator otherParticleSimulator in reactor.particleSimulators)
            {
                if (particleSimulator == otherParticleSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( particleSimulator + " is registered to the Reactor " + count + " times" ); }
                return false;
            }
        }
        foreach (ParticleSimulator particleSimulator in reactor.particleSimulatorsInBimolecularReactions)
        {
            if (particleSimulator == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null bimolecular reactant ParticleSimulator" ); }
                return false;
            }
            if (!ParticleSimulatorReallyIsBimolecularReactant( particleSimulator ))
            {
                if (debug) { Debug.Log( particleSimulator + " isn't a bimolecular reactant but is registered as such in the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (ParticleSimulator otherParticleSimulator in reactor.particleSimulatorsInBimolecularReactions)
            {
                if (particleSimulator == otherParticleSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( particleSimulator + " is registered as a bimolecular reactant in the Reactor " + count + " times" ); }
                return false;
            }
        }
        foreach (ParticleSimulator particleSimulator in GameObject.FindObjectsOfType<ParticleSimulator>())
        {
            if (!reactor.particleSimulators.Contains( particleSimulator ))
            {
                if (debug) { Debug.Log( particleSimulator + " isn't registered to the Reactor" ); }
                return false;
            }
        }

        //particle populations
        foreach (Population population in reactor.populations.Values)
        {
            if (population == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null Population" ); }
                return false;
            }
            if (population.reactor != reactor)
            {
                if (debug) { Debug.Log( population + " doesn't reference the Reactor" ); }
                return false;
            }
            if (population.theTransform.parent != reactor.transform)
            {
                if (debug) { Debug.Log( population + " is not parented to the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (Population otherPopulation in reactor.populations.Values)
            {
                if (population == otherPopulation)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( population + " is registered to the Reactor " + count + " times" ); }
                return false;
            }
            AssertIsTrue( StateOfPopulationIsCorrect( population ) );
        }
        foreach (Population population in GameObject.FindObjectsOfType<Population>())
        {
            if (!reactor.populations.ContainsValue( population ))
            {
                if (debug) { Debug.Log( population + " isn't registered to the Reactor" ); }
                return false;
            }
        }

        if (debug) { Debug.Log( "Reactor passed check" ); }

        return true;
    }

    static bool ParticleSimulatorReallyIsBimolecularReactant (ParticleSimulator particleSimulator)
    {
        //foreach (MoleculeSimulator moleculeSimulator in particleSimulator.complex)
        //{
        //    foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
        //    {
        //        foreach (string activeState in bindingSiteSimulator.population.activeStates)
        //        {
        //            if (bindingSiteSimulator.state == activeState)
        //            {
        //                return true;
        //            }
        //        }
        //    }
        //}
        return false;
    }

    static bool StateOfPopulationIsCorrect (Population population)
    {
        foreach (ParticleSimulator particleSimulator in population.GetComponentsInChildren<ParticleSimulator>())
        {
            if (particleSimulator.population != population)
            {
                if (debug) { Debug.Log( particleSimulator + " doesn't reference " + population ); }
                return false;
            }
            if (particleSimulator.theTransform.parent != population.theTransform)
            {
                if (debug) { Debug.Log( particleSimulator + " is not parented to " + population ); }
                return false;
            }
            AssertIsTrue( StateOfParticleSimulatorIsCorrect( particleSimulator ) );
        }
        return true;
    }

    static bool StateOfParticleSimulatorIsCorrect (ParticleSimulator particleSimulator)
    {
        foreach (MoleculeSimulator moleculeSimulator in particleSimulator.complex)
        {
            if (moleculeSimulator == null)
            {
                if (debug) { Debug.Log( particleSimulator + " has a null MoleculeSimulator" ); }
                return false;
            }
            if (moleculeSimulator.particleSimulator != particleSimulator)
            {
                if (debug) { Debug.Log( moleculeSimulator + " doesn't reference " + particleSimulator ); }
                return false;
            }
            int count = 0;
            foreach (MoleculeSimulator otherMoleculeSimulator in particleSimulator.complex)
            {
                if (moleculeSimulator == otherMoleculeSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( moleculeSimulator + " is registered to " + particleSimulator + " " + count + " times" ); }
                return false;
            }
            AssertIsTrue( StateOfMoleculeSimulatorIsCorrect( moleculeSimulator ) );
        }
        foreach (MoleculeSimulator moleculeSimulator in particleSimulator.GetComponentsInChildren<MoleculeSimulator>())
        {
            if (moleculeSimulator.particleSimulator != particleSimulator)
            {
                if (debug) { Debug.Log( moleculeSimulator + " isn't registered to " + particleSimulator ); }
                return false;
            }
        }

        if (particleSimulator.complex.Length == 1)
        {
            if (particleSimulator.complex[0].gameObject != particleSimulator.gameObject)
            {
                if (debug) { Debug.Log( particleSimulator.complex[0] + " isn't on the same GameObject as " + particleSimulator 
                                        + "even though it's the only molecule in the complex" ); }
                return false;
            }
        }
        else if (particleSimulator.complex.Length > 1)
        {
            foreach (MoleculeSimulator moleculeSimulator in particleSimulator.complex)
            {
                if (moleculeSimulator.theTransform.parent != particleSimulator.theTransform)
                {
                    if (debug) { Debug.Log( moleculeSimulator + " is not parented to " + particleSimulator ); }
                    return false;
                }
            }
        }
        else 
        {
            if (debug) { Debug.Log( particleSimulator + " has no MoleculeSimulators and therefore shouldn't exist" ); }
            return false;
        }
        return true;
    }

    static bool StateOfMoleculeSimulatorIsCorrect (MoleculeSimulator moleculeSimulator)
    {
        foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
        {
            if (bindingSiteSimulator == null)
            {
                if (debug) { Debug.Log( moleculeSimulator + " has a null BindingSiteSimulator" ); }
                return false;
            }
            if (bindingSiteSimulator.moleculeSimulator != moleculeSimulator)
            {
                if (debug) { Debug.Log( bindingSiteSimulator + " doesn't reference " + moleculeSimulator ); }
                return false;
            }
            if (bindingSiteSimulator.theTransform.parent != moleculeSimulator.theTransform)
            {
                if (debug) { Debug.Log( bindingSiteSimulator + " is not parented to " + moleculeSimulator ); }
                return false;
            }
            int count = 0;
            foreach (BindingSiteSimulator otherBindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
            {
                if (bindingSiteSimulator == otherBindingSiteSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( bindingSiteSimulator + " is registered to " + moleculeSimulator + " " + count + " times" ); }
                return false;
            }
        }
        foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.GetComponentsInChildren<BindingSiteSimulator>())
        {
            if (!moleculeSimulator.bindingSiteSimulators.ContainsValue( bindingSiteSimulator ))
            {
                if (debug) { Debug.Log( bindingSiteSimulator + " isn't registered to " + moleculeSimulator ); }
                return false;
            }
        }
        return true;
    }
}

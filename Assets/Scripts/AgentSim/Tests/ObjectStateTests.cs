using System.Collections;
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
            if (particleSimulator.reactor != reactor)
            {
                if (debug) { Debug.Log( particleSimulator + " doesn't reference the Reactor" ); }
                return false;
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
        foreach (ParticleSimulator particleSimulator in reactor.GetComponentsInChildren<ParticleSimulator>())
        {
            if (!reactor.particleSimulators.Contains( particleSimulator ))
            {
                if (debug) { Debug.Log( particleSimulator + " isn't registered to the Reactor" ); }
                return false;
            }
            ComplexSimulator complexSimulator = particleSimulator.GetComponent<ComplexSimulator>();
            if (complexSimulator == null)
            {
                if (debug) { Debug.Log( particleSimulator + " doesn't have a ComplexSimulator" ); }
                return false;
            }
        }

        //complex simulators
        foreach (ComplexSimulator complexSimulator in reactor.complexSimulators)
        {
            if (complexSimulator == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null ComplexSimulator" ); }
                return false;
            }
            if (complexSimulator.reactor != reactor)
            {
                if (debug) { Debug.Log( complexSimulator + " doesn't reference the Reactor" ); }
                return false;
            }
            if (!ComplexSimulatorReallyIsBimolecularReactant( complexSimulator ))
            {
                if (debug) { Debug.Log( complexSimulator + " isn't a bimolecular reactant but is registered as such in the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (ComplexSimulator otherComplexSimulator in reactor.complexSimulators)
            {
                if (complexSimulator == otherComplexSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( complexSimulator + " is registered to the Reactor " + count + " times" ); }
                return false;
            }
        }
        foreach (ComplexSimulator complexSimulator in reactor.GetComponentsInChildren<ComplexSimulator>())
        {
            if (ComplexSimulatorReallyIsBimolecularReactant( complexSimulator ) && !reactor.complexSimulators.Contains( complexSimulator ))
            {
                if (debug) { Debug.Log( complexSimulator + " isn't registered to the Reactor" ); }
                return false;
            }
            AssertIsTrue( StateOfComplexSimulatorIsCorrect( complexSimulator ) );
        }

        if (debug) { Debug.Log( "Reactor passed check" ); }
        return true;
    }

    static bool ComplexSimulatorReallyIsBimolecularReactant (ComplexSimulator complexSimulator)
    {
        foreach (BimolecularReactionSimulator reactionSimulator in complexSimulator.reactor.bimolecularReactionSimulators)
        {
            if (reactionSimulator.IsReactant( complexSimulator.complex ))
            {
                return true;
            }
        }
        return false;
    }

    static bool StateOfComplexSimulatorIsCorrect (ComplexSimulator complexSimulator)
    {
        bool bimolecularReactant = false;
        foreach (MoleculeSimulator moleculeSimulator in complexSimulator.complex)
        {
            if (moleculeSimulator == null)
            {
                if (debug) { Debug.Log( complexSimulator + " has a null MoleculeSimulator" ); }
                return false;
            }
            if (moleculeSimulator.complexSimulator != complexSimulator)
            {
                if (debug) { Debug.Log( moleculeSimulator + " doesn't reference " + complexSimulator ); }
                return false;
            }
            int count = 0;
            foreach (MoleculeSimulator otherMoleculeSimulator in complexSimulator.complex)
            {
                if (moleculeSimulator == otherMoleculeSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( moleculeSimulator + " is registered to " + complexSimulator + " " + count + " times" ); }
                return false;
            }
            if (moleculeSimulator.couldReactOnCollision)
            {
                bimolecularReactant = true;
                if (!complexSimulator.couldReactOnCollision)
                {
                    if (debug) { Debug.Log( moleculeSimulator + " is bimolecular reactant but " + complexSimulator + " isn't" ); }
                    return false;
                }
            }
            AssertIsTrue( StateOfMoleculeSimulatorIsCorrect( moleculeSimulator ) );
        }
        if (complexSimulator.couldReactOnCollision && !bimolecularReactant)
        {
            if (debug) { Debug.Log( complexSimulator + " has no bimolecular reactant molecule but is marked as a bimolecular reactant" ); }
            return false;
        }
        foreach (MoleculeSimulator moleculeSimulator in complexSimulator.GetComponentsInChildren<MoleculeSimulator>())
        {
            if (moleculeSimulator.complexSimulator != complexSimulator)
            {
                if (debug) { Debug.Log( moleculeSimulator + " isn't registered to " + complexSimulator ); }
                return false;
            }
        }

        //parenting
        if (complexSimulator.complex.Length > 0)
        {
            foreach (MoleculeSimulator moleculeSimulator in complexSimulator.complex)
            {
                if (moleculeSimulator.theTransform.parent != complexSimulator.theTransform)
                {
                    if (debug) { Debug.Log( moleculeSimulator + " is not parented to " + complexSimulator ); }
                    return false;
                }
            }
        }
        else 
        {
            if (debug) { Debug.Log( complexSimulator + " has no MoleculeSimulators and therefore shouldn't exist" ); }
            return false;
        }
        return true;
    }

    static bool StateOfMoleculeSimulatorIsCorrect (MoleculeSimulator moleculeSimulator)
    {
        bool bimolecularReactant = false;
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
            if (bindingSiteSimulator.couldReactOnCollision)
            {
                bimolecularReactant = true;
                if (!moleculeSimulator.couldReactOnCollision)
                {
                    if (debug) { Debug.Log( bindingSiteSimulator + " is bimolecular reactant but " + moleculeSimulator + " isn't" ); }
                    return false;
                }
            }
        }
        if (moleculeSimulator.couldReactOnCollision && !bimolecularReactant)
        {
            if (debug) { Debug.Log( moleculeSimulator + " has no bimolecular reactant binding site but is marked as a bimolecular reactant" ); }
            return false;
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

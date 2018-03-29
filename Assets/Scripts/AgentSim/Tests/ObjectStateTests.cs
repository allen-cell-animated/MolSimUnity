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

        for (int i = 0; i < numberOfTimesToCheck; i++)
        {
            yield return new WaitForSeconds( waitTime );
            yield return new WaitForEndOfFrame();

            AssertIsTrue( StateOfReactorIsCorrect( reactor ) );
        }

        DestroyReactor( reactor );
    }

    bool StateOfReactorIsCorrect (Reactor reactor)
    {
        //particle simulators
        foreach (ParticleSimulator particleSimulator in reactor.particleSimulators)
        {
            if (particleSimulator == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null ParticleSimulator" ); }
                return false;
            }
            if (ParticleSimulatorReallyIsActive( particleSimulator ))
            {
                bool found = false;
                foreach (ParticleSimulator activeParticleSimulator in reactor.activeParticleSimulators)
                {
                    if (particleSimulator == activeParticleSimulator)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    if (debug) { Debug.Log( particleSimulator + " is active but isn't registered as active in the Reactor" ); }
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
        foreach (ParticleSimulator particleSimulator in reactor.activeParticleSimulators)
        {
            if (particleSimulator == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null active ParticleSimulator" ); }
                return false;
            }
            if (!ParticleSimulatorReallyIsActive( particleSimulator ))
            {
                if (debug) { Debug.Log( particleSimulator + " isn't active but is registered as active in the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (ParticleSimulator otherParticleSimulator in reactor.activeParticleSimulators)
            {
                if (particleSimulator == otherParticleSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( particleSimulator + " is registered as active in the Reactor " + count + " times" ); }
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
        foreach (ParticlePopulation particlePopulation in reactor.particlePopulations.Values)
        {
            if (particlePopulation == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null ParticlePopulation" ); }
                return false;
            }
            if (particlePopulation.reactor != reactor)
            {
                if (debug) { Debug.Log( particlePopulation + " doesn't reference the Reactor" ); }
                return false;
            }
            if (particlePopulation.theTransform.parent != reactor.transform)
            {
                if (debug) { Debug.Log( particlePopulation + " is not parented to the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (ParticlePopulation otherParticlePopulation in reactor.particlePopulations.Values)
            {
                if (particlePopulation == otherParticlePopulation)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( particlePopulation + " is registered to the Reactor " + count + " times" ); }
                return false;
            }
            AssertIsTrue( StateOfParticlePopulationIsCorrect( particlePopulation ) );
        }
        foreach (ParticlePopulation particlePopulation in GameObject.FindObjectsOfType<ParticlePopulation>())
        {
            if (!reactor.particlePopulations.ContainsValue( particlePopulation ))
            {
                if (debug) { Debug.Log( particlePopulation + " isn't registered to the Reactor" ); }
                return false;
            }
        }

        if (debug) { Debug.Log( "Reactor passed check" ); }

        return true;
    }

    bool ParticleSimulatorReallyIsActive (ParticleSimulator particleSimulator)
    {
        foreach (MoleculeSimulator moleculeSimulator in particleSimulator.moleculeSimulators)
        {
            foreach (BindingSiteSimulator bindingSiteSimulator in moleculeSimulator.bindingSiteSimulators.Values)
            {
                foreach (string activeState in bindingSiteSimulator.population.bindingSite.activeStates)
                {
                    if (bindingSiteSimulator.state == activeState)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    bool StateOfParticlePopulationIsCorrect (ParticlePopulation particlePopulation)
    {
        //binding site populations
        foreach (BindingSitePopulation bindingSitePopulation in particlePopulation.bindingSitePopulations)
        {
            if (bindingSitePopulation == null)
            {
                if (debug) { Debug.Log( particlePopulation + " has a null BindingSitePopulation" ); }
                return false;
            }
            if (bindingSitePopulation.particlePopulation != particlePopulation)
            {
                if (debug) { Debug.Log( bindingSitePopulation + " doesn't reference " + particlePopulation ); }
                return false;
            }
            if (bindingSitePopulation.transform != particlePopulation.theTransform)
            {
                if (debug) { Debug.Log( bindingSitePopulation + " is not on the same object as " + particlePopulation ); }
                return false;
            }
            int count = 0;
            foreach (BindingSitePopulation otherBindingSitePopulation in particlePopulation.bindingSitePopulations)
            {
                if (bindingSitePopulation == otherBindingSitePopulation)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( bindingSitePopulation + " is registered to " + particlePopulation + " " + count + " times" ); }
                return false;
            }
            AssertIsTrue( StateOfBindingSitePopulationIsCorrect( bindingSitePopulation ) );
        }
        foreach (BindingSitePopulation bindingSitePopulation in particlePopulation.GetComponents<BindingSitePopulation>())
        {
            if (!particlePopulation.bindingSitePopulations.Contains( bindingSitePopulation ))
            {
                if (debug) { Debug.Log( bindingSitePopulation + " isn't registered to " + particlePopulation ); }
                return false;
            }
        }

        //particle simulators
        foreach (ParticleSimulator particleSimulator in particlePopulation.GetComponentsInChildren<ParticleSimulator>())
        {
            if (particleSimulator.population != particlePopulation)
            {
                if (debug) { Debug.Log( particleSimulator + " doesn't reference " + particlePopulation ); }
                return false;
            }
            if (particleSimulator.theTransform.parent != particlePopulation.theTransform)
            {
                if (debug) { Debug.Log( particleSimulator + " is not parented to " + particlePopulation ); }
                return false;
            }
            AssertIsTrue( StateOfParticleSimulatorIsCorrect( particleSimulator ) );
        }
        return true;
    }

    bool StateOfBindingSitePopulationIsCorrect (BindingSitePopulation bindingSitePopulation)
    {
        foreach (BindingSiteSimulator bindingSiteSimulator in bindingSitePopulation.bindingSiteSimulators)
        {
            if (bindingSiteSimulator == null)
            {
                if (debug) { Debug.Log( bindingSitePopulation + " has a null BindingSiteSimulator" ); }
                return false;
            }
            if (bindingSiteSimulator.population != bindingSitePopulation)
            {
                if (debug) { Debug.Log( bindingSiteSimulator + " doesn't reference " + bindingSitePopulation ); }
                return false;
            }
            int count = 0;
            foreach (BindingSiteSimulator otherBindingSiteSimulator in bindingSitePopulation.bindingSiteSimulators)
            {
                if (bindingSiteSimulator == otherBindingSiteSimulator)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( bindingSiteSimulator + " is registered to " + bindingSitePopulation + " " + count + " times" ); }
                return false;
            }
        }
        return true;
    }

    bool StateOfParticleSimulatorIsCorrect (ParticleSimulator particleSimulator)
    {
        foreach (MoleculeSimulator moleculeSimulator in particleSimulator.moleculeSimulators)
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
            foreach (MoleculeSimulator otherMoleculeSimulator in particleSimulator.moleculeSimulators)
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

        if (particleSimulator.moleculeSimulators.Length == 1)
        {
            if (particleSimulator.moleculeSimulators[0].gameObject != particleSimulator.gameObject)
            {
                if (debug) { Debug.Log( particleSimulator.moleculeSimulators[0] + " isn't on the same GameObject as " + particleSimulator 
                                        + "even though it's the only molecule in the complex" ); }
                return false;
            }
        }
        else if (particleSimulator.moleculeSimulators.Length > 1)
        {
            foreach (MoleculeSimulator moleculeSimulator in particleSimulator.moleculeSimulators)
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

    bool StateOfMoleculeSimulatorIsCorrect (MoleculeSimulator moleculeSimulator)
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

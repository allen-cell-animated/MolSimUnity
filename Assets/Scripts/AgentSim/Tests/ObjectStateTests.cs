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
        //particles
        foreach (Mover mover in reactor.movers)
        {
            if (mover == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null Mover" ); }
                return false;
            }
            if (mover.reactor != reactor)
            {
                if (debug) { Debug.Log( mover + " doesn't reference the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (Mover otherMover in reactor.movers)
            {
                if (mover == otherMover)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( mover + " is registered to the Reactor " + count + " times" ); }
                return false;
            }
        }
        foreach (Mover mover in reactor.GetComponentsInChildren<Mover>())
        {
            if (!reactor.movers.Contains( mover ))
            {
                if (debug) { Debug.Log( mover + " isn't registered to the Reactor" ); }
                return false;
            }
            Complex complex = mover.GetComponent<Complex>();
            if (complex == null)
            {
                if (debug) { Debug.Log( mover + " doesn't have a Complex" ); }
                return false;
            }
        }

        //complexes
        foreach (Complex complex in reactor.complexes)
        {
            if (complex == null)
            {
                if (debug) { Debug.Log( "The Reactor has a null Complex" ); }
                return false;
            }
            if (complex.reactor != reactor)
            {
                if (debug) { Debug.Log( complex + " doesn't reference the Reactor" ); }
                return false;
            }
            if (!ComplexReallyIsBimolecularReactant( complex ))
            {
                if (debug) { Debug.Log( complex + " isn't a bimolecular reactant but is registered as such in the Reactor" ); }
                return false;
            }
            int count = 0;
            foreach (Complex otherComplex in reactor.complexes)
            {
                if (complex == otherComplex)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( complex + " is registered to the Reactor " + count + " times" ); }
                return false;
            }
        }
        foreach (Complex complex in reactor.GetComponentsInChildren<Complex>())
        {
            if (ComplexReallyIsBimolecularReactant( complex ) && !reactor.complexes.Contains( complex ))
            {
                if (debug) { Debug.Log( complex + " isn't registered to the Reactor" ); }
                return false;
            }
            AssertIsTrue( StateOfComplexSimulatorIsCorrect( complex ) );
        }

        if (debug) { Debug.Log( "Reactor passed check" ); }
        return true;
    }

    static bool ComplexReallyIsBimolecularReactant (Complex complex)
    {
        foreach (BimolecularReactionSimulator reactionSimulator in complex.reactor.bimolecularReactionSimulators)
        {
            if (reactionSimulator.IsReactant( complex.molecules ))
            {
                return true;
            }
        }
        return false;
    }

    static bool StateOfComplexSimulatorIsCorrect (Complex complex)
    {
        bool bimolecularReactant = false;
        foreach (Molecule molecule in complex.molecules)
        {
            if (molecule == null)
            {
                if (debug) { Debug.Log( complex + " has a null Molecule" ); }
                return false;
            }
            if (molecule.complex != complex)
            {
                if (debug) { Debug.Log( molecule + " doesn't reference " + complex ); }
                return false;
            }
            int count = 0;
            foreach (Molecule otherMolecule in complex.molecules)
            {
                if (molecule == otherMolecule)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( molecule + " is registered to " + complex + " " + count + " times" ); }
                return false;
            }
            if (molecule.couldReactOnCollision)
            {
                bimolecularReactant = true;
                if (!complex.couldReactOnCollision)
                {
                    if (debug) { Debug.Log( molecule + " is bimolecular reactant but " + complex + " isn't" ); }
                    return false;
                }
            }
            AssertIsTrue( StateOfMoleculeSimulatorIsCorrect( molecule ) );
        }
        if (complex.couldReactOnCollision && !bimolecularReactant)
        {
            if (debug) { Debug.Log( complex + " has no bimolecular reactant molecule but is marked as a bimolecular reactant" ); }
            return false;
        }
        foreach (Molecule molecule in complex.GetComponentsInChildren<Molecule>())
        {
            if (molecule.complex != complex)
            {
                if (debug) { Debug.Log( molecule + " isn't registered to " + complex ); }
                return false;
            }
        }

        //parenting
        if (complex.molecules.Length > 0)
        {
            foreach (Molecule molecule in complex.molecules)
            {
                if (molecule.theTransform.parent != complex.theTransform)
                {
                    if (debug) { Debug.Log( molecule + " is not parented to " + complex ); }
                    return false;
                }
            }
        }
        else 
        {
            if (debug) { Debug.Log( complex + " has no Molecules and therefore shouldn't exist" ); }
            return false;
        }
        return true;
    }

    static bool StateOfMoleculeSimulatorIsCorrect (Molecule molecule)
    {
        bool bimolecularReactant = false;
        foreach (BindingSite bindingSite in molecule.bindingSites.Values)
        {
            if (bindingSite == null)
            {
                if (debug) { Debug.Log( molecule + " has a null BindingSite" ); }
                return false;
            }
            if (bindingSite.molecule != molecule)
            {
                if (debug) { Debug.Log( bindingSite + " doesn't reference " + molecule ); }
                return false;
            }
            if (bindingSite.theTransform.parent != molecule.theTransform)
            {
                if (debug) { Debug.Log( bindingSite + " is not parented to " + molecule ); }
                return false;
            }
            int count = 0;
            foreach (BindingSite otherBindingSite in molecule.bindingSites.Values)
            {
                if (bindingSite == otherBindingSite)
                {
                    count++;
                }
            }
            if (count > 1)
            {
                if (debug) { Debug.Log( bindingSite + " is registered to " + molecule + " " + count + " times" ); }
                return false;
            }
            if (bindingSite.couldReactOnCollision)
            {
                bimolecularReactant = true;
                if (!molecule.couldReactOnCollision)
                {
                    if (debug) { Debug.Log( bindingSite + " is bimolecular reactant but " + molecule + " isn't" ); }
                    return false;
                }
            }
        }
        if (molecule.couldReactOnCollision && !bimolecularReactant)
        {
            if (debug) { Debug.Log( molecule + " has no bimolecular reactant binding site but is marked as a bimolecular reactant" ); }
            return false;
        }
        foreach (BindingSite bindingSite in molecule.GetComponentsInChildren<BindingSite>())
        {
            if (!molecule.bindingSites.ContainsValue( bindingSite ))
            {
                if (debug) { Debug.Log( bindingSite + " isn't registered to " + molecule ); }
                return false;
            }
        }
        return true;
    }
}

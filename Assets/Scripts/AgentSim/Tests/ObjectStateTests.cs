using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ObjectStateTests : MolSimTests
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
}

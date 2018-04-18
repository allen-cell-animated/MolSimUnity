using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ContainerTests : MolSimTests
{
    [UnityTest]
    public IEnumerator ParticlesStayInBounds ()
    {
        Reactor reactor = CreateReactor( "BasicMoleculeDiffusion" );

        yield return new WaitForEndOfFrame();

        Mover[] movers = GameObject.FindObjectsOfType<Mover>();

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (Mover mover in movers)
            {
                AssertIsTrue( reactor.container.IsInBounds( mover.theTransform.position ) );
            }
        }

        DestroyReactor( reactor );
    }
}

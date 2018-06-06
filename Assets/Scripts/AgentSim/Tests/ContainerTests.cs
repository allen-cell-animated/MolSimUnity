using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ContainerTests : MolSimTests
{
    [UnityTest]
    public IEnumerator ParticlesStayInBounds ()
    {
        Reactor reactor = CreateReactor( "BasicMoleculeDiffusion" );

        yield return new WaitForEndOfFrame();

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (Mover mover in reactor.movers)
            {
                AssertIsTrue( reactor.container.IsInBounds( mover.position ) );
            }
        }

        DestroyReactor( reactor );
    }
}

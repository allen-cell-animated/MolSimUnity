using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ContainerTests : AgentSimTests
{
    [UnityTest]
    public IEnumerator ParticlesStayInBounds ()
    {
        Reactor reactor = CreateReactor( "BasicMoleculeDiffusion" );

        yield return new WaitForEndOfFrame();

        ParticleSimulator[] particleSimulators = GameObject.FindObjectsOfType<ParticleSimulator>();
        Vector3 nullVector;

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (ParticleSimulator particleSimulator in particleSimulators)
            {
                Assert.IsFalse( reactor.container.IsOutOfBounds( particleSimulator.theTransform.position, out nullVector ) );
            }
        }

        DestroyReactor( reactor );
    }
}

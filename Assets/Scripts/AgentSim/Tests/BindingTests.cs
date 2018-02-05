using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class BindingTests : AgentSimTests
{
    [UnityTest]
    public IEnumerator BimolecularReactantsOnlyReactOnce ()
    {
        CreateWorld();

        ParticleReactor reactor = (GameObject.Instantiate( Resources.Load( "Tests/DefaultReactor" ) as GameObject ) as GameObject).GetComponent<ParticleReactor>();
        reactor.model = Resources.Load( "Tests/BiomolecularReaction" ) as Model;
        World.Instance.rootAgents = new Agent[] { reactor.agent };

        yield return new WaitForEndOfFrame();

        ParticleSimulator[] particles = reactor.GetComponentInChildren<ParticlePopulation>().GetComponentsInChildren<ParticleSimulator>();

        //let them react and bind
        yield return new WaitForSeconds( 10f );

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (ParticleSimulator particle in particles)
            {
                Assert.IsTrue( particle.transform.childCount < 2 );
            }
        }

        DestroyWorld();

        yield return null;
    }
}

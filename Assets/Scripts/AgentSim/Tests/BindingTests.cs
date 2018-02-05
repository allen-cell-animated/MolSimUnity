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

        ParticleSimulator[] particleAs = GetPopulationForSpecies( "A", reactor ).GetComponentsInChildren<ParticleSimulator>();

        //let them react and bind
        yield return new WaitForSeconds( 10f );

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (ParticleSimulator particleA in particleAs)
            {
                Assert.IsTrue( particleA.transform.childCount < 2 );
            }
        }

        DestroyWorld();

        yield return null;
    }

    [UnityTest]
    public IEnumerator BimolecularBindsAreReciprocal ()
    {
        CreateWorld();

        ParticleReactor reactor = (GameObject.Instantiate( Resources.Load( "Tests/DefaultReactor" ) as GameObject ) as GameObject).GetComponent<ParticleReactor>();
        reactor.model = Resources.Load( "Tests/BiomolecularReaction" ) as Model;
        World.Instance.rootAgents = new Agent[] { reactor.agent };

        yield return new WaitForEndOfFrame();

        ParticleSimulator[] particleAs = GetPopulationForSpecies( "A", reactor ).GetComponentsInChildren<ParticleSimulator>();

        //let them react and bind
        yield return new WaitForSeconds( 10f );

        for (int i = 0; i < 500; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (ParticleSimulator particleA in particleAs)
            {
                for (int j = 0; j < particleA.reactionStates.Length; j++)
                {
                    if (particleA.reactionStates[j].currentBindingPartner != null)
                    {
                        Assert.IsTrue( particleA.reactionStates[j].currentBindingPartner.reactionStates[j].currentBindingPartner == particleA );
                    }
                }
            }
        }

        DestroyWorld();

        yield return null;
    }
}

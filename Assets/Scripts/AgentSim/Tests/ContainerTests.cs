using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class ContainerTests
{
    [UnityTest]
    public IEnumerator ManagedParticlesStayInBounds ()
    {
        CreateWorld();

        ParticleReactor reactor = (GameObject.Instantiate( Resources.Load( "Tests/DefaultReactor" ) as GameObject ) as GameObject).GetComponent<ParticleReactor>();
        reactor.model = Resources.Load( "Tests/ANullReaction" ) as Model;
        reactor.Init();
        World.Instance.rootAgents = new Agent[] { reactor.agent };

        ParticleSimulator[] particles = reactor.GetComponentInChildren<ParticlePopulation>().GetComponentsInChildren<ParticleSimulator>();
        Vector3 nullVector;

        for (int i = 0; i < 200; i++)
        {
            yield return new WaitForEndOfFrame();

            foreach (ParticleSimulator particle in particles)
            {
                Assert.IsFalse( reactor.container.IsOutOfBounds( particle.transform.position, out nullVector ) );
            }
        }

        yield return null;
    }

    void CreateWorld ()
    {
        GameObject.Instantiate( Resources.Load( "DefaultLight" ) as GameObject );
        GameObject.Instantiate( Resources.Load( "EventSystem" ) as GameObject );
        new GameObject( "World", typeof(World) );
        World.Instance.observer = (GameObject.Instantiate( Resources.Load( "Observer" ) as GameObject ) as GameObject).GetComponentInChildren<Observer>();
    }
}

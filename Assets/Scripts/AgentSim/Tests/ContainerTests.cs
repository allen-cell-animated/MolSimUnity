//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.TestTools;
//using AICS.AgentSim;

//public class ContainerTests : AgentSimTests
//{
//    [UnityTest]
//    public IEnumerator ManagedParticlesStayInBounds ()
//    {
//        CreateWorld();

//        ParticleReactor reactor = (GameObject.Instantiate( Resources.Load( "Tests/DefaultReactor" ) as GameObject ) as GameObject).GetComponent<ParticleReactor>();
//        reactor.model = Resources.Load( "Tests/ANullReaction" ) as Model;
//        World.Instance.rootAgents = new Agent[] { reactor.agent };

//        yield return new WaitForEndOfFrame();

//        ParticleSimulator[] particles = reactor.GetComponentInChildren<ParticlePopulation>().GetComponentsInChildren<ParticleSimulator>();
//        Vector3 nullVector;

//        for (int i = 0; i < 200; i++)
//        {
//            yield return new WaitForEndOfFrame();

//            foreach (ParticleSimulator particle in particles)
//            {
//                Assert.IsFalse( reactor.container.IsOutOfBounds( particle.transform.position, out nullVector ) );
//            }
//        }

//        DestroyWorld();

//        yield return null;
//    }
//}

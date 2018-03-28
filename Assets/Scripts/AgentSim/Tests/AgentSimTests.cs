using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class AgentSimTests
{
    protected Reactor CreateReactor (string modelName)
    {
        GameObject.Instantiate( Resources.Load( "DefaultLight" ) as GameObject );
        GameObject.Instantiate( Resources.Load( "EventSystem" ) as GameObject );
        new GameObject( "World", typeof(World) );
        World.Instance.observer = (GameObject.Instantiate( Resources.Load( "Observer" ) as GameObject ) as GameObject).GetComponentInChildren<Observer>();

        Reactor reactor = (GameObject.Instantiate( Resources.Load( "DefaultReactor" ) as GameObject ) as GameObject).GetComponent<Reactor>();
        reactor.model = Resources.Load( "Tests/Models/" + modelName ) as Model;

        return reactor;
    }

    protected void DestroyReactor (Reactor reactor)
    {
        GameObject.Destroy( reactor.gameObject );
        GameObject.Destroy( GameObject.Find( "EventSystem(Clone)" ) );
        GameObject.Destroy( GameObject.Find( "DefaultLight(Clone)" ) );
        GameObject.Destroy( World.Instance.observer.GetComponentInParent<FollowLookZoomCamera>().gameObject );
        GameObject.Destroy( World.Instance.gameObject );
    }
}

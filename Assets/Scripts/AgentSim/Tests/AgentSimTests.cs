using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class AgentSimTests
{
    protected void CreateWorld ()
    {
        GameObject.Instantiate( Resources.Load( "DefaultLight" ) as GameObject );
        GameObject.Instantiate( Resources.Load( "EventSystem" ) as GameObject );
        new GameObject( "World", typeof(World) );
        World.Instance.observer = (GameObject.Instantiate( Resources.Load( "Observer" ) as GameObject ) as GameObject).GetComponentInChildren<Observer>();
    }

    protected void DestroyWorld ()
    {
        foreach (Agent agent in World.Instance.rootAgents)
        {
            GameObject.Destroy( agent.gameObject );
        }
        GameObject.Destroy( World.Instance.observer.GetComponentInParent<FollowLookZoomCamera>().gameObject );
        GameObject.Destroy( World.Instance.gameObject );
        GameObject.Destroy( GameObject.Find( "EventSystem(Clone)" ) );
        GameObject.Destroy( GameObject.Find( "DefaultLight(Clone)" ) );
    }
}

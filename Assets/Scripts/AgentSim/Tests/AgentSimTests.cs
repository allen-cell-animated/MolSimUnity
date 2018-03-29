using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using AICS.AgentSim;

public class AgentSimTests
{
    protected static bool debug = true;

    protected static void AssertIsTrue (bool value)
    {
        if (debug && !value)
        {
            Debug.LogError( "Test failed" );
            UnityEditor.EditorApplication.isPaused = true;
        }
        else
        {
            Assert.IsTrue( value );
        }
    }

    GameObject light;
    GameObject eventSystem;
    World world;

    protected Reactor CreateReactor (string modelName)
    {
        light = GameObject.Instantiate( Resources.Load( "DefaultLight" ) as GameObject );
        eventSystem = GameObject.Instantiate( Resources.Load( "EventSystem" ) as GameObject );
        world = new GameObject( "World", typeof(World) ).GetComponent<World>();
        world.observer = (GameObject.Instantiate( Resources.Load( "Observer" ) as GameObject ) as GameObject).GetComponentInChildren<Observer>();

        Reactor reactor = (GameObject.Instantiate( Resources.Load( "DefaultReactor" ) as GameObject ) as GameObject).GetComponent<Reactor>();
        reactor.model = Resources.Load( "Tests/Models/" + modelName ) as Model;

        return reactor;
    }

    protected void DestroyReactor (Reactor reactor)
    {
        GameObject.Destroy( reactor.gameObject );
        GameObject.Destroy( eventSystem );
        GameObject.Destroy( light );
        GameObject.Destroy( world.observer.transform.parent.parent.gameObject );
        GameObject.Destroy( world.gameObject );
    }
}

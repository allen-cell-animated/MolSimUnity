//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Assertions;
//using UnityEngine.TestTools;
//using AICS.AgentSim;

//public class AgentSimTests
//{
//    protected void CreateWorld ()
//    {
//        GameObject.Instantiate( Resources.Load( "DefaultLight" ) as GameObject );
//        GameObject.Instantiate( Resources.Load( "EventSystem" ) as GameObject );
//        new GameObject( "World", typeof(World) );
//        World.Instance.observer = (GameObject.Instantiate( Resources.Load( "Observer" ) as GameObject ) as GameObject).GetComponentInChildren<Observer>();
//    }

//    protected void DestroyWorld ()
//    {
//        GameObject.Destroy( World.Instance.observer.GetComponentInParent<FollowLookZoomCamera>().gameObject );
//        GameObject.Destroy( World.Instance.gameObject );
//        GameObject.Destroy( GameObject.Find( "EventSystem(Clone)" ) );
//        GameObject.Destroy( GameObject.Find( "DefaultLight(Clone)" ) );
//    }

//    protected MoleculePopulation GetPopulationForSpecies (string species, MoleculeReactor reactor)
//    {
//        MoleculePopulation[] populations = reactor.GetComponentsInChildren<MoleculePopulation>();
//        foreach (MoleculePopulation population in populations)
//        {
//            if (population.species == species)
//            {
//                return population;
//            }
//        }
//        return null;
//    }
//}

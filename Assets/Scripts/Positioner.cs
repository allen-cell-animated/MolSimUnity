using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Positioner : MonoBehaviour 
{
    public GameObject prefab;

    static Positioner _Instance;
    public static Positioner Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<Positioner>();
            }
            return _Instance;
        }
    }

	void Start () 
    {
        //create complex 1
        AgentComplex complex1 = new AgentComplex( new Vector3( -5f, 0, 0 ), Quaternion.Euler( new Vector3( 0, 200f, 90f ) ) );
        Agent[] agents = new Agent[4];
        agents[0] = new Agent( complex1, new RelativeTransform( new Vector3( 0, 0, 0 ), new Vector3( 0, 160f, 0 ) ) );
        AgentComponent component1 = new AgentComponent( agents[0], new RelativeTransform( new Vector3( 0.5f, 0, 0 ), new Vector3( 0, 0, 0 ) ) );
        GameObject[] objects = new GameObject[4];
        objects[0] = Instantiate( prefab );
        objects[0].name = "Agent_0";
        objects[0].transform.SetParent( transform );
        objects[0].transform.position = agents[0].position;
        objects[0].transform.rotation = Quaternion.Euler( agents[0].rotation );

        //create complex 2
        AgentComplex complex2 = new AgentComplex( new Vector3( 0, 0, 0 ), Quaternion.Euler( new Vector3( 100f, 0, 0 ) ) );
        agents[1] = new Agent( complex2, new RelativeTransform( new Vector3( 0, -0.35f, -1.97f ), new Vector3( -100f, 0, 90f ) ) );
        agents[2] = new Agent( complex2, new RelativeTransform( new Vector3( -1f, 0, 0 ), new Vector3( -100f, 0, 0 ) ) );
        agents[3] = new Agent( complex2, new RelativeTransform( new Vector3( 1f, 0, 0 ), new Vector3( -100f, 0, 90f ) ) );
        AgentComponent component2 = new AgentComponent( agents[2], new RelativeTransform( new Vector3( -0.5f, 0, 0 ), new Vector3( 0, 0, 0 ) ) );
        for (int i = 1; i < 4; i++)
        {
            objects[i] = Instantiate( prefab );
            objects[i].name = "Agent_" + i;
            objects[i].transform.SetParent( transform );
            objects[i].transform.position = agents[i].position;
            objects[i].transform.rotation = Quaternion.Euler( agents[i].rotation );
        }

        //Bind 1 to 2
        component1.agent.SetWorldTransform( GetWorldTransformForBindingAgent( component1.agent.position, component1.agent.rotation, component1.position, 
                                                                              component1.rotation, component2.position, component2.rotation ) );
        AgentComplex newComplex = new AgentComplex( new Vector3( -1.25f, 0.5f, 0 ), Quaternion.Euler( new Vector3( 0, 0, 0 ) ) );
        for (int i = 0; i < 4; i++)
        {
            agents[i].MoveToNewComplex( newComplex );
            objects[i].transform.position = agents[i].position;
            objects[i].transform.rotation = Quaternion.Euler( agents[i].rotation );
        }
    }

    Transform _parentTransform;
    Transform parentTransform
    {
        get
        {
            if (_parentTransform == null)
            {
                _parentTransform = new GameObject( "parentTransformCalculator" ).transform;
                _parentTransform.SetParent( transform );
            }
            return _parentTransform;
        }
    }

    Transform _childTransform;
    Transform childTransform
    {
        get
        {
            if (_childTransform == null)
            {
                _childTransform = new GameObject( "childTransformCalculator" ).transform;
                _childTransform.SetParent( transform );
            }
            return _childTransform;
        }
    }

    public RelativeTransform GetWorldTransform (Vector3 parentWorldPosition, Quaternion parentWorldRotation, RelativeTransform localTransform)
    {
        parentTransform.position = parentWorldPosition;
        parentTransform.rotation = parentWorldRotation;

        childTransform.position = parentTransform.TransformPoint( localTransform.position );
        childTransform.rotation = parentTransform.rotation * Quaternion.Euler( localTransform.rotation );
        return new RelativeTransform( childTransform.position, childTransform.rotation.eulerAngles );
    }

    public RelativeTransform GetWorldTransformForBindingAgent (Vector3 agentPosition, Vector3 agentRotation, Vector3 agentsComponentPosition, 
                                                               Vector3 agentsComponentRotation, Vector3 otherComponentPosition, Vector3 otherComponentRotation)
    {
        parentTransform.position = otherComponentPosition;
        parentTransform.rotation = Quaternion.Euler( otherComponentRotation );

        childTransform.position = agentsComponentPosition;
        childTransform.rotation = Quaternion.Euler( agentsComponentRotation );

        Vector3 pos = parentTransform.TransformPoint( childTransform.InverseTransformPoint( agentPosition ) );
        Quaternion rot = Quaternion.Euler( agentRotation ) * Quaternion.Inverse( childTransform.rotation ) * parentTransform.rotation;

        return new RelativeTransform( pos, rot.eulerAngles );
    }

    public RelativeTransform GetParentWorldTransform (RelativeTransform childWorldTransform, RelativeTransform childLocalTransform)
    {
        childTransform.SetParent( parentTransform );
        childTransform.localPosition = childLocalTransform.position;
        childTransform.localRotation = Quaternion.Euler( childLocalTransform.rotation );

        childTransform.SetParent( transform );
        parentTransform.SetParent( childTransform );
        childTransform.position = childWorldTransform.position;
        childTransform.rotation = Quaternion.Euler( childWorldTransform.rotation );

        parentTransform.SetParent( transform );
        return new RelativeTransform( parentTransform.position, parentTransform.rotation.eulerAngles );
    }

    public RelativeTransform GetLocalTransform (Vector3 parentWorldPosition, Quaternion parentWorldRotation, RelativeTransform childWorldTransform)
    {
        parentTransform.position = parentWorldPosition;
        parentTransform.rotation = parentWorldRotation;

        childTransform.position = childWorldTransform.position;
        childTransform.rotation = Quaternion.Euler( childWorldTransform.rotation );

        childTransform.SetParent( parentTransform );
        Vector3 childLocalPosition = childTransform.localPosition;
        Vector3 childLocalRotation = childTransform.localRotation.eulerAngles;
        childTransform.SetParent( transform );

        return new RelativeTransform( childLocalPosition, childLocalRotation );
    }
}

public class AgentComplex
{
    public Vector3 position;
    public Quaternion rotation;

    public void SetWorldTransform (RelativeTransform worldTransform)
    {
        position = worldTransform.position;
        rotation = Quaternion.Euler( worldTransform.rotation );
    }

    public AgentComplex (Vector3 _position, Quaternion _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}

public class Agent
{
    public AgentComplex complex;
    public RelativeTransform localTransform;

    public RelativeTransform worldTransform
    {
        get
        {
            return Positioner.Instance.GetWorldTransform( complex.position, complex.rotation, localTransform );
        }
    }

    public Vector3 position
    {
        get
        {
            return worldTransform.position;
        }
    }

    public Vector3 rotation
    {
        get
        {
            return worldTransform.rotation;
        }
    }

    public void SetWorldTransform (RelativeTransform worldTransform)
    {
        complex.SetWorldTransform( Positioner.Instance.GetParentWorldTransform( worldTransform, localTransform ) );
    }

    public Agent (AgentComplex _complex, RelativeTransform _localTransform)
    {
        complex = _complex;
        localTransform = _localTransform;
    }

    public void MoveToNewComplex (AgentComplex _complex)
    {
        RelativeTransform _worldTransform = new RelativeTransform( worldTransform );

        complex = _complex;

        localTransform = Positioner.Instance.GetLocalTransform( complex.position, complex.rotation, _worldTransform );
    }
}

public class AgentComponent
{
    public Agent agent;
    public RelativeTransform localTransform;

    public RelativeTransform worldTransform
    {
        get
        {
            return Positioner.Instance.GetWorldTransform( agent.worldTransform.position, Quaternion.Euler( agent.worldTransform.rotation ), localTransform );
        }
    }

    public Vector3 position
    {
        get
        {
            return worldTransform.position;
        }
    }

    public Vector3 rotation
    {
        get
        {
            return worldTransform.rotation;
        }
    }

    public AgentComponent (Agent _agent, RelativeTransform _localTransform)
    {
        agent = _agent;
        localTransform = _localTransform;
    }
}

public class RelativeTransform
{
    public Vector3 position;
    public Vector3 rotation;

    public RelativeTransform (RelativeTransform _relativeTransform)
    {
        position = _relativeTransform.position;
        rotation = _relativeTransform.rotation;
    }

    public RelativeTransform (Vector3 _position, Vector3 _rotation)
    {
        position = _position;
        rotation = _rotation;
    }
}
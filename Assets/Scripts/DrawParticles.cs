using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParticles : MonoBehaviour 
{
    public Material[] actin_materials;

	void Start () 
    {
        Vector3[] positions = {
            new Vector3(-0.03965151114467284f, -1.9880115902397453f, -13.321342203900473f),
            new Vector3(3.091786537446908f, -1.9815162710592775f, -10.375557924943926f),
            new Vector3(0.055982926675163525f, -1.0812259166693754f, -7.451902484917322f),
            new Vector3(2.6073594854277276f, -2.723676305368459f, -4.447807831008204f),
            new Vector3(0.6523030210438239f, -0.46076629418837367f, -1.391024034100351f),
            new Vector3(1.8230995865832116f, -3.260219691076887f, 1.6960618408148525f),
            new Vector3(1.7796086933924593f, -0.16718933725417534f, 4.721885669123888f),
            new Vector3(0.862079629950828f, -3.040679215058683f, 7.830528040580357f),
            new Vector3(2.5930501270377677f, -0.40111892126801263f, 10.877961550359114f),
            new Vector3(0.02121059020320138f, -2.246583228337101f, 13.934679758253395f),
            new Vector3(2.724386584347353f, -0.9773472906233698f, 17.12147633863358f),
            new Vector3(-0.46528958308698853f, -1.6717556012153845f, 19.980013448837983f),
            new Vector3(2.305193180094751f, -1.2918357653507089f, 23.166145428639144f),
            new Vector3(-0.6769708545508571f, -1.6575735563815392f, 26.053639897679844f),
            new Vector3(1.9600340828978424f, -0.9239393871890339f, 29.044339422880352f),
            new Vector3(0.03181472707714594f, -1.8180136203330197f, 32.46690333238013f),
            new Vector3(1.5484734532035547f, 0.041812903231136844f, 34.807095778636985f),
            new Vector3(1.607583714036089f, -1.7989167907948302f, 38.53571256243624f),
            new Vector3(0.3595103691996989f, 0.4893451448770024f, 40.70075535232131f),
            new Vector3(1.9138239286844845f, -0.3544352004465023f, 44.314983790858186f),
            new Vector3(-0.40761383032977005f, 0.5030879586642812f, 46.857811719797866f),
            new Vector3(2.078138103346854f, 0.34036505046823873f, -49.897116681074834f)
        };

        string[] names = {
            "actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3","actin#ATP_1","actin#ATP_2","actin#ATP_3"
        };

        Edge[] edges = {
            new Edge(0, 1), new Edge(1, 2), new Edge(2, 3), new Edge(3, 4), new Edge(4, 5), new Edge(5, 6), new Edge(6, 7), new Edge(7, 8),
            new Edge(8, 9), new Edge(9, 10), new Edge(10, 11), new Edge(11, 12), new Edge(12, 13), new Edge(13, 14), new Edge(14, 15), new Edge(15, 16), 
            new Edge(16, 17), new Edge(17, 18), new Edge(18, 19), new Edge(19, 20), new Edge(20, 21), new Edge(21, 22)
        };
        
        //CreateSpheres( positions );
        CreateParticles( positions, names, edges );
        //DrawLine( positions );
    }

    void CreateSpheres (Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = positions[i];
            g.transform.SetParent( transform );
            g.name = i.ToString();
        }
    }

    void CreateGeometry (Particle[] particles)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            GameObject g = Instantiate( Resources.Load("geometry/" + particles[i].type + "_cone") as GameObject, particles[i].position, particles[i].rotation ); 
            g.transform.SetParent( transform );
            g.name = i.ToString() + "_" + particles[i].type;
        }
    }

    void CreateParticles (Vector3[] positions, string[] names, Edge[] edges)
    {
        GameObject[] objs = new GameObject[positions.Length];
        GameObject prefab = Resources.Load("Line") as GameObject;
        Transform edge_parent = new GameObject().transform;

        for (int i = 0; i < positions.Length; i++)
        {
            objs[i] = GameObject.CreatePrimitive( PrimitiveType.Sphere );
            objs[i].transform.SetParent( transform );
            objs[i].transform.position = positions[i];
            objs[i].transform.localScale = 3f * Vector3.one;

            // name and color by type
            objs[i].name = names.Length >= positions.Length ? names[i] : (i+1).ToString();
            if (names[i].Substring(0, 3) == "act")
            {
                objs[i].GetComponent<MeshRenderer>().material = actin_materials[int.Parse(names[i].Substring(10))-1];
            }
        }

        // draw edges
        for (int i = 0; i < edges.Length; i++)
        {
            LineRenderer l = (Instantiate(prefab) as GameObject).GetComponent<LineRenderer>();

            l.transform.SetParent( edge_parent );
            l.SetPositions(new Vector3[] {positions[edges[i].index1], positions[edges[i].index2]});
            l.name = i.ToString();
        }
    }

    void DrawLine (Vector3[][] positions)
    {
        GameObject prefab = Resources.Load("Line") as GameObject;

        for (int i = 0; i < positions.Length; i++)
        {
            LineRenderer l = (Instantiate(prefab) as GameObject).GetComponent<LineRenderer>();

            l.transform.SetParent( transform );
            l.positionCount = positions[i].Length;
            l.SetPositions(positions[i]);
            l.name = Mathf.Round(i).ToString();
        }
    }
}

public class Edge
{
    public int index1;
    public int index2;

    public Edge (int _index1, int _index2)
    {
        index1 = _index1;
        index2 = _index2;
    }
}

public class Particle
{
    public string type;
    public Vector3 position;
    public Quaternion rotation;

    public Particle (string _type, Vector3 _position, Quaternion _rotation)
    {
        type = _type;
        position = _position;
        rotation = _rotation;
    }
}

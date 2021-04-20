using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawParticles : MonoBehaviour
{
    public Material[] actin_materials;

	void Start ()
    {
        // ideal
        // Vector3[] positions = new Vector3[]{
        //     new Vector3(191.26f, 208.38f, 277.57f),
        //     new Vector3(218.47f, 241.71f, 271.48f),
        //     new Vector3(247.38f, 208.81f, 266.71f),
        //     new Vector3(276.09f, 240.61f, 275.98f),
        //     new Vector3(303.82f, 211.90f, 257.25f),
        //     new Vector3(333.74f, 235.53f, 279.51f),
        //     new Vector3(360.75f, 216.42f, 250.60f),
        //     new Vector3(390.05f, 228.61f, 279.70f),
        //     new Vector3(280.87f, 308.72f, 266.57f),
        //     new Vector3(292.75f, 275.35f, 239.44f),
        //     new Vector3(298.21f, 330.88f, 233.56f),
        //     new Vector3(304.76f, 360.34f, 265.28f),
        //     new Vector3(308.97f, 385.84f, 230.14f),
        // };
        // string[] names = new string[]{"actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "arp2#ATP", "arp3#branched", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3"};
        // Edge[] edges = new Edge[]{new Edge(0, 1), new Edge(1, 2), new Edge(2, 3), new Edge(3, 4), new Edge(4, 5), new Edge(5, 6), new Edge(6, 7), new Edge(3, 8), new Edge(4, 9), new Edge(8, 9), new Edge(8, 10), new Edge(10, 11), new Edge(11, 12)};
        // CreateParticles(positions, names, edges);

        Vector3[] positions = new Vector3[]{
            new Vector3(-48.91982360683854f, -1.4216740607048233f, -0.8821834007906482f),
            new Vector3(-46.11672484383854f, 1.5841694699236273f, 0.5383416926761201f),
            new Vector3(-43.31362608083854f, -1.6648184040924012f, -0.1666864388806925f),
            new Vector3(-40.51052731783854f, 1.6594541177312099f, -0.21358070759417455f),
            new Vector3(-37.707428554838536f, -1.5683537579118523f, 0.5828131583580347f),
            new Vector3(-34.904329791838535f, 1.3962240453915231f, -0.9219344350776939f),
            new Vector3(-32.101231028838534f, -1.151958100817121f, 1.2134237600580058f),
            new Vector3(-29.298132265838536f, 0.8481759796295222f, -1.442221271196274f),
            new Vector3(-26.495033502838535f, -0.5005726540362302f, 1.5965060931742856f),
            new Vector3(-23.691934739838533f, 0.1271071287116945f, -1.668307065663322f),
            new Vector3(-20.888835976838532f, 0.2529254152533937f, 1.6539145751308904f),
            new Vector3(-18.08573721383853f, -0.6198905102604779f, -1.5540722128654443f),
            new Vector3(-15.282638450838533f, 0.9548288214259126f, 1.3739383571637767f),
            new Vector3(-12.479539687838534f, -1.2404356849188627f, -1.1228196645460777f),
            new Vector3(-9.676440924838534f, 1.461955157343878f, 0.8136902392355039f),
            new Vector3(-6.873342161838534f, -1.6079423849039696f, -0.4625213231201664f),
            new Vector3(-4.070243398838533f, 1.6708549043684224f, 0.08745613791773033f),
            new Vector3(-1.2671446358385337f, -1.6474423261467024f, 0.29212748849062076f),
            new Vector3(1.5359541271614663f, 1.488091443449153f, -0.634933386666874f),
            new Vector3(4.339052890161466f, -1.350877851967828f, 0.9871847428796102f),
            new Vector3(7.1421516531614655f, 1.0930480268480676f, -1.2667480801307895f),
            new Vector3(9.945250416161464f, -0.7787456274511261f, 1.480864590446151f),
            new Vector3(12.748349179161464f, 0.424209158544834f, -1.618471895745545f),
            new Vector3(15.551447942161463f, -0.047755827225767516f, 1.6724604833829342f),
            new Vector3(18.354546705161464f, -0.3311648197408576f, -1.640041020730122f),
            new Vector3(21.157645468161466f, 0.6929757611671566f, 1.5228884666797813f),
            new Vector3(23.960744231161467f, -1.0189839526835123f, -1.3270555345142798f),
            new Vector3(26.763842994161468f, 1.292346107116656f, 1.0626599771008223f),
            new Vector3(29.56694175716147f, -1.4989389067425605f, -0.7433618508942414f),
            new Vector3(32.37004052016147f, 1.6280886877006844f, 0.38565776602132396f),
            new Vector3(35.17313928316147f, -1.6731228972587648f, -0.008028585176170461f),
            new Vector3(37.97623804616147f, 1.6317148327634738f, -0.3700153943434835f),
            new Vector3(40.77933680916147f, -1.5060038512608387f, 0.7289424445722829f),
            new Vector3(43.58243557216147f, 1.3024848391291253f, -1.0502085180335716f),
            new Vector3(46.38553433516147f, -1.0316726523006645f, 1.3172153301591984f),
            new Vector3(49.188633098161475f, 0.7075588638446992f, -1.516167913427234f),
            new Vector3(2.1566358053799344f, 8.275743190648576f, 0.3879175366993759f),
            new Vector3(3.4261772326159705f, 4.879179994602608f, 2.9877069511236902f),
            new Vector3(4.079654817233559f, 10.413936443528288f, 3.6350917643123593f),
            new Vector3(4.451524392568317f, 13.519581818094913f, -0.8523932186827016f),
            new Vector3(4.8512361513809905f, 16.132886221043485f, 2.6003187787933677f),
            new Vector3(6.885169863776497f, 18.60107417400749f, -0.3461216859951364f),
            new Vector3(5.9577933349360634f, 21.680467779963948f, 2.5809343987885285f),
            new Vector3(9.165818569565129f, 23.71414460864071f, 0.4636053783939522f),
            new Vector3(7.278006988762575f, 27.174022144353795f, 2.3009307179499103f),
            new Vector3(11.183189710237354f, 28.90090002635242f, 1.477650557447116f),
            new Vector3(8.897517007421296f, 32.57804068848428f, 1.8834662503628716f),
            new Vector3(12.88070890094933f, 34.18841621303847f, 2.555197252601184f),
            new Vector3(10.84090933761164f, 37.87527934291321f, 1.4797403575578216f),
            new Vector3(14.267048871659883f, 39.58321460292766f, 3.5424750886732292f),
            new Vector3(13.066700651277374f, 43.070276240004056f, 1.2381498470518637f),
            new Vector3(15.414360302967678f, 45.06993196070233f, 4.304127780537808f)
        };
        string[] names = new string[]{"actin#pointed_ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#barbed_ATP_3", "arp2#branched", "arp3#ATP", "actin#branch_ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#ATP_2", "actin#ATP_3", "actin#ATP_1", "actin#barbed_ATP_2"};
        Edge[] edges = new Edge[]{new Edge(0, 1), new Edge(1, 2), new Edge(2, 3), new Edge(3, 4), new Edge(4, 5), new Edge(5, 6), new Edge(6, 7), new Edge(7, 8), new Edge(8, 9), new Edge(9, 10), new Edge(10, 11), new Edge(11, 12), new Edge(12, 13), new Edge(13, 14), new Edge(14, 15), new Edge(15, 16), new Edge(16, 17), new Edge(17, 18), new Edge(18, 19), new Edge(19, 20), new Edge(20, 21), new Edge(21, 22), new Edge(22, 23), new Edge(23, 24), new Edge(24, 25), new Edge(25, 26), new Edge(26, 27), new Edge(27, 28), new Edge(28, 29), new Edge(29, 30), new Edge(30, 31), new Edge(31, 32), new Edge(32, 33), new Edge(33, 34), new Edge(34, 35), new Edge(18, 36), new Edge(19, 37), new Edge(36, 37), new Edge(36, 38), new Edge(38, 39), new Edge(39, 40), new Edge(40, 41), new Edge(41, 42), new Edge(42, 43), new Edge(43, 44), new Edge(44, 45), new Edge(45, 46), new Edge(46, 47), new Edge(47, 48), new Edge(48, 49), new Edge(49, 50), new Edge(50, 51)};
        CreateParticles(positions, names, edges);
    }

    void CreateSpheres (Vector3[] positions, string[] ids)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = positions[i];
            g.transform.SetParent( transform );
            if (ids != null)
            {
                g.name = ids[i];
            }
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
                objs[i].GetComponent<MeshRenderer>().material = actin_materials[int.Parse(names[i].Substring(names[i].Length-1))-1];
            }
        }

        // draw edges
        for (int i = 0; i < edges.Length; i++)
        {
            if (edges[i].index1 < positions.Length && edges[i].index2 < positions.Length)
            {
                LineRenderer l = (Instantiate(prefab) as GameObject).GetComponent<LineRenderer>();
        
                l.transform.SetParent( edge_parent );
                l.SetPositions(new Vector3[] {positions[edges[i].index1], positions[edges[i].index2]});
                l.name = i.ToString();
            }
        }
    }

    void DrawLines (Vector3[][] points)
    {
        GameObject prefab = Resources.Load("Line") as GameObject;

        for (int i = 0; i < points.Length; i++)
        {
            LineRenderer l = (Instantiate(prefab) as GameObject).GetComponent<LineRenderer>();

            l.transform.SetParent( transform );
            l.positionCount = points[i].Length;
            l.SetPositions(points[i]);
            l.name = i.ToString();
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

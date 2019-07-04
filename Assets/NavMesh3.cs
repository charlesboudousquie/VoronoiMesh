using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NavMesh3 : MonoBehaviour
{
    public VoronoiNode[] nodes;

    void Start()
    {
        //First pass create all the nodes and add them to the node[]
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        int size = mesh.triangles.Length/3;//always divisiable by 3
       
        for(int i = 0; i < size; ++i)
        {
            VoronoiNode v = new VoronoiNode(i);
            for(int tri = 0; tri < 3; ++tri)
            {
                v.AddPoint(mesh.vertices[mesh.triangles[3 * i + tri]]);
            }
            nodes[i] = v;
        }


        //Second pass find the neighbors

    }

    void Update()
    {

    }

    public VoronoiNode GetNode(Vector3 position)
    {
        foreach (VoronoiNode vN in nodes)
        {
            if(vN.GetPosition() == position)
            {
                return vN;
            }
        }
        return null;
    }
}


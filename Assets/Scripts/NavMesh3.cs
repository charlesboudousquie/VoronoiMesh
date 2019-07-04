using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NavMesh3 : MonoBehaviour
{
    public VoronoiNode[] nodes { get; }

    void Start()
    {
        //First pass create all the nodes and add them to the node[]
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = mf.mesh;
        int size = mesh.triangles.Length/3;//always divisiable by 3
       
        for(int i = 0; i < size; ++i)
        {
            VoronoiNode v = new VoronoiNode(i);
            Vector3 normals = new Vector3(0, 0, 0);
            for(int tri = 0; tri < 3; ++tri)
            {
                int ver_index = mesh.triangles[3 * i + tri];
                v.AddPoint(mesh.vertices[ver_index]);
                normals += mesh.normals[ver_index];
            }
            v.SetNormal(normals / 3.0f);
            nodes[i] = v;
        }

        //Second pass find the neighbors
        
        for(int i = 0; i < size; ++i)
        {
            VoronoiNode v1 = nodes[i];
            Vector3[] vert1 = v1.GetVertices();
            for (int j = i+1; j < size; ++j)
            {
                VoronoiNode v2 = nodes[j];
                Vector3[] vert2 = v2.GetVertices();
                int count = 0;
                for(int n1 = 0; n1 < 3; ++n1)
                {
                    for(int n2 = n1; n2 < 3; ++n2)
                    {
                        if (vert1[n1] == vert2[n2]) ++count;
                    }
                }
                if(count == 2)
                {
                    v1.AddNeighbor(v2);
                    v2.AddNeighbor(v1);
                }
            }
        }
        

    }

    void Update()
    {

    }

    public VoronoiNode GetNode(int _id)
    {
        return nodes[_id];
    }

    public VoronoiNode GetNode(Vector3 position)
    {
        foreach (VoronoiNode vN in nodes)
        {
            if(vN.Position == position)
            {
                return vN;
            }
        }
        return null;
    }
}


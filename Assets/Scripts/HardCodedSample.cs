using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardCodedSample : MonoBehaviour
{
    public VoronoiNode rootNode;
    // Start is called before the first frame update
    void Start()
    {
        //this is just for the test version, in the actual model we should just load the mesh in with the normal unity componenet
        MeshFilter mf = GetComponent<MeshFilter> ();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(0, 0, 0);
        vertices[1] = new Vector3(1, 0, 0);
        vertices[2] = new Vector3(0, 0, 1);
        vertices[3] = new Vector3(0, 1, 0);
        mesh.vertices = vertices;

        //add the points to the tris in groups of 3 in(ccw?) rotational order, 
        //if the normal is wrong change the order
        int[] tris = new int[] {
            0, 1, 2,
            1, 0, 3,
            0, 2, 3,
            2, 1, 3
        };
        mesh.triangles = tris;

        //normals are by vertex and a pain in the ass to figure out, just use default
        mesh.RecalculateNormals();


        rootNode = new VoronoiNode(0);
        VoronoiNode n1 = new VoronoiNode(1);
        VoronoiNode n2 = new VoronoiNode(2);
        VoronoiNode n3 = new VoronoiNode(3);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

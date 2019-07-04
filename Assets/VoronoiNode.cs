using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiNode
{

    private VoronoiNode[] neighbors;
    private Vector3[] vertices;
    private Vector3 normal;
    int numVerts, numNeighbors;
    // Start is called before the first frame update
    public void Start()
    {
        neighbors = new VoronoiNode[3];
        vertices = new Vector3[3];
        numVerts = 0;
        numNeighbors = 0;
    }

    public void AddNeighbor(ref VoronoiNode n) {
        if (numNeighbors < 3) {
            neighbors[numNeighbors] = n;
            numNeighbors++;
        } else {
            Debug.Log("Attempt to add 4th neighbor.");
        }
    }

    public void SetNormal(Vector3 n) {
        normal = n;
    }

    public void AddPoint(Vector3 p) {
        if (numVerts < 3) {
            vertices[numVerts] = p;
            numVerts++;
        } else {
            Debug.Log("Attempt to add 4th vert.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

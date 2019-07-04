using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiNode
{

    private VoronoiNode[] neighbors;
    private Vector3[] vertices;
    private Vector3 normal;
    private Vector3 position;
    int numVerts, numNeighbors;
    public float Cost;
    public float Hueristic;//recommend Euclidean distance
    // Start is called before the first frame update
    public void Start()
    {
        neighbors = new VoronoiNode[3];
        vertices = new Vector3[3];
        numVerts = 0;
        numNeighbors = 0;
    }

    public VoronoiNode[] GetNeighbors() {
        return neighbors;
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

    public Vector3 GetPosition()
    {
        return position;
    }

    public void AddPoint(Vector3 p) {
        if (numVerts < 3) {
            vertices[numVerts] = p;
            numVerts++;
        } else {
            Debug.Log("Attempt to add 4th vert.");
        }
        if(numVerts == 3)
        {
            position = (vertices[0] + vertices[1] + vertices[2]) / 3.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

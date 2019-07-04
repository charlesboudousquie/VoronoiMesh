using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiNode
{

    private int[] neighbors;
    private Vector3[] vertices;
    private Vector3 normal;
    public Vector3 Position { get; set; }
    int numVerts, numNeighbors;
    public int Id { get; }
    public float Cost;
    public float Hueristic;//recommend Euclidean distance
    public float Given;
    public bool Open;
    public bool Closed;
    public int Parent;
    public VoronoiNode(int _id)
    {
        Id = _id;
    }
    // Start is called before the first frame update
    public void Start()
    {
        Open = Closed = false;
        Cost = Hueristic = Given = 0;
        neighbors = new int[3];
        vertices = new Vector3[3];
        numVerts = 0;
        numNeighbors = 0;
    }

    public void Reset()
    {
        Open = Closed = false;
        Cost = Hueristic = Given = 0;
    }

    public int[] GetNeighbors() {
        return neighbors;
    }

    public Vector3[] GetVertices()
    {
        return vertices;
    }

    public void AddNeighbor(VoronoiNode n) {
        if (numNeighbors < 3) {
            neighbors[numNeighbors] = n.Id;
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
        if(numVerts == 3)
        {
            Position = (vertices[0] + vertices[1] + vertices[2]) / 3.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

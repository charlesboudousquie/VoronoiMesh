using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStartNavMesh3 : MonoBehaviour
{
    NodeHeap openHeap;
    public NavMesh3 navMesh;
    List<Vector3> moves;

    // Start is called before the first frame update
    void Start()
    {
        moves = new List<Vector3>();
        openHeap = new NodeHeap();
        if (navMesh != null && navMesh.nodes != null) {
            openHeap.InitHeap(navMesh.nodes);
        }
    }

    void OnMapReset() {
        openHeap.InitHeap(navMesh.nodes);
        string res = "";
        for (int i = 0; i < navMesh.nodes.Length; i++) {
            res = res + "node " + i + ": ";
            for (int j = 0; j < 3; j++) {
                res = res + navMesh.nodes[i].GetNeighbors()[j] + ", ";
            }
            res = res + "\n";
        }
        Debug.Log(res);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            int start = Random.Range(0, navMesh.nodes.Length);
            int end = Random.Range(0, navMesh.nodes.Length);
            Debug.Log(start);
            Debug.Log(end);
            moves = GetPath(navMesh.nodes[start], navMesh.nodes[end]);
            foreach (Vector3 move in moves) {
                Debug.Log(move);
            }
        }
        bool first = true;
        Vector3 last = Vector3.zero;
        foreach (Vector3 move in moves) {
            if (first) {
                last = move;
                first = false;
            }
            Debug.DrawLine(last, move, Color.black);
            last = move;
        }
    }

    public List<Vector3> GetPath(VoronoiNode start, VoronoiNode end) {
        List<Vector3> path = new List<Vector3>();
        
        openHeap.ResetHeap();
        openHeap.Push(start.Id);

        int NumLoops = 100000;
        Debug.Log("enter pathfinding");
        while (!openHeap.IsEmpty() && NumLoops > 0) {
            NumLoops--;
            VoronoiNode least = openHeap.Pop();

            if (least.Id == end.Id) {
                Debug.Log("path found");
                while (least.Id != start.Id) {
                    path.Add(least.Position);
                    least = openHeap.GetNode(least.Parent);
                }
                path.Add(start.Position);
                return path;
            }

            //if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);
            int[] neighbors = least.GetNeighbors();
            for (int i = 0; i < 3; i++) {
                VoronoiNode nbr = openHeap.GetNode(neighbors[i]);
                if (nbr.Open) {
                    float DistToNbr = Vector3.Distance(nbr.Position, least.Position);
                    if (nbr.Given > least.Given + DistToNbr) {
                        openHeap.SetGiven(nbr.Id, least.Id, least.Given + DistToNbr);
                        openHeap.UpdateNode(nbr.Id);
                    }
                } else if (!nbr.Closed) {
                    openHeap.SetHeuristic(nbr.Id, least.Given + Vector3.Distance(nbr.Position, least.Position));
                    openHeap.SetGiven(nbr.Id, least.Id, least.Given + Vector3.Distance(nbr.Position, least.Position));
                    openHeap.Push(nbr.Id);
                    //if (request.settings.debugColoring) terrain->set_color(ny, nx, Colors::Yellow);
                }
            }
            //if (request.settings.singleStep) return PathResult::PROCESSING;
        }

        if (NumLoops == 0) {
            Debug.Log("exited due to too many iterations in pathfinding loop");
        } else {
            Debug.Log("no path found");
        }
        //handle error: no path exists
        //return PathResult::IMPOSSIBLE;
        
        return path;
    }
}

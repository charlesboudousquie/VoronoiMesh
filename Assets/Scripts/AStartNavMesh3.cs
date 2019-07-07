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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P)) {
            int start = Random.Range(0, navMesh.nodes.Length);
            int end = Random.Range(0, navMesh.nodes.Length);
            moves = GetPath(navMesh.nodes[start], navMesh.nodes[end]);
        }
        bool first = true;
        Vector3 last = Vector3.zero;
        foreach (Vector3 move in moves) {
            if (first) {
                last = move;
                first = false;
            }
            Debug.DrawLine(last, move, Color.gray, .01f, false);
            Debug.DrawLine(last, move, Color.black, .01f);
            last = move;
        }
    }

    public List<Vector3> GetPath(VoronoiNode start, VoronoiNode end) {
        List<Vector3> path = new List<Vector3>();
        
        openHeap.ResetHeap();
        openHeap.Push(start.Id);

        int MaxNumLoops = 100000;
        while (!openHeap.IsEmpty() && MaxNumLoops > 0) {
            MaxNumLoops--;
            VoronoiNode least = openHeap.Pop();

            if (least.Id == end.Id) {
                while (least.Id != start.Id) {
                    path.Add(least.Position);
                    least = openHeap.GetNode(least.Parent);
                }
                path.Add(start.Position);
                //funnel path!!
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

        if (MaxNumLoops == 0) {
            Debug.Log("error: exited due to too many iterations in pathfinding loop");
        } else {
            Debug.Log("error: no path found");
        }
        //handle error: no path exists
        //return PathResult::IMPOSSIBLE;
        
        return path;
    }
}

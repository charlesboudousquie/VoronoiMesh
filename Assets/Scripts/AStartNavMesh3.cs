using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStartNavMesh3 : MonoBehaviour
{
    NodeHeap openHeap;
    public NavMesh3 navMesh;

    // Start is called before the first frame update
    void Start()
    {
        openHeap.InitHeap(navMesh.nodes);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector3> GetPath(VoronoiNode start, VoronoiNode end) {
        List<Vector3> path = new List<Vector3>();

        int numNodes = navMesh.nodes.Length;
        for (int i = 0; i < numNodes; i++) {
            navMesh.nodes[i].Reset();
        }
        
        openHeap.ResetHeap();

        int s = start.Id;
        openHeap.Push(s);
        start.Open = true;

        
        while (!openHeap.IsEmpty()) {

            VoronoiNode least = openHeap.Pop();

            if (least.Id == end.Id) {
                //celebrate: push stuff onto solution path and return success
                while (least.Id != start.Id) {
                    path.Add(least.Position);
                    least = navMesh.nodes[least.Parent];
                }
                path.Add(start.Position);
                return path;
            }

            least.Open = false;
            //if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);
            int[] neighbors = least.GetNeighbors();
            for (int i = 0; i < 3; i++) {
                VoronoiNode nbr = navMesh.nodes[neighbors[i]];
                if (nbr.Open) {
                    float DistToNbr = Vector3.Distance(nbr.Position, least.Position);
                    if (nbr.Given > least.Given + DistToNbr) {
                        nbr.Parent = least.Id;
                        nbr.Cost = least.Cost + DistToNbr;
                        openHeap.UpdateNode(nbr.Id);
                    }
                } else if (!nbr.Closed) {
                    nbr.Parent = least.Id;
                    nbr.Open = true;
                    nbr.Hueristic = Vector3.Distance(nbr.Position, end.Position);
                    nbr.Cost = least.Cost + Vector3.Distance(nbr.Position, least.Position);
                    openHeap.Push(nbr.Id);
                    //if (request.settings.debugColoring) terrain->set_color(ny, nx, Colors::Yellow);
                }
            }
            //if (request.settings.singleStep) return PathResult::PROCESSING;
        }
        //handle error: no path exists
        //return PathResult::IMPOSSIBLE;
        
        return path;
    }
}

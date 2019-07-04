using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStartNavMesh3 : MonoBehaviour
{
    NodeHeap openHeap;

    // Start is called before the first frame update
    void Start()
    {
        VoronoiNode[] list = new VoronoiNode[1];
        openHeap.InitHeap(list);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector3> GetPath(VoronoiNode start, VoronoiNode end) {
        List<Vector3> path = new List<Vector3>();

        int numNodes = 0;
        for (int i = 0; i < numNodes; i++) {
            //reset all nodes somehow
        }
        
        openHeap.ResetHeap();

        //int s = start.Id;
        int s = 0;
        openHeap.Push(s);
        //start.Open = true;

        
        while (!openHeap.IsEmpty()) {

            VoronoiNode least = openHeap.Pop();

            if (least.Id == end.Id) {
                //celebrate: push stuff onto solution path and return success
                while (least.Id != start.Id) {
                    path.Add(least.position);
                    least = least.parent;
                }
                path.Add(start.position);
                return path;
            }

            least.Open = false;
            //if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);

            for (int i = 0; i < 3; i++) {
                VoronoiNode nbr = least.neighbors[i];
                if (nbr.Open) {
                    float DistToNbr = Vector3.Distance(nbr, least);
                    if (nbr.Given > least.Given + DistToNbr) {
                        nbr.Parent = least;
                        nbr.Cost = least.Cost + DistToNbr;
                        openHeap.UpdateNode(nbr.Id);
                    }
                } else if (!nbr.Closed) {
                    nbr.Parent = least;
                    nbr.Open = true;
                    nbr.Heuristic = Vector3.Distance(nbr.Position - end.Position);
                    nbr.Cost = least.Cost + Vector3.Distance(nbr, least);
                    openHeap.Push(nbr);
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

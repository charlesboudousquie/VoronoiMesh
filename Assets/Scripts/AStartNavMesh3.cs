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

        /*
        while (!openHeap.IsEmpty()) {

            VoronoiNode least = openHeap.Pop();

            lx = least->x();
            ly = least->y();

            if (least.Id == end.Id) {
                //celebrate: push stuff onto solution path and return success
                while (least.Id != start.Id) {
                    path.Add(least.position);
                    least = least.parent;
                }
                path.Add(start.position);
                return path;
            }

            least->SetClosed();
            if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);
            nbrList = least->GetNeighbors();

            for (int i = 0; i < 8; i++) {
                if (!(nbrList & (0b00000001 << i))) continue;
                nbr = getNodeFromInt(i, lx, ly);
                if (nbr->OnOpen()) {
                    if (nbr->GetGivenS() - (least->GetGivenS() + costS[i]) > (least->GetGivenD() + costD[i] - nbr->GetGivenD()) * 1.141) {
                        nbr->Open(least, costS[i], costD[i]);
                        openHeap.UpdateNode(nbr->x(), nbr->y());
                    }
                } else if (!nbr->OnClosed()) {
                    nx = nbr->x();
                    ny = nbr->y();
                    nbr->Open(least, costS[i], costD[i]);
                    SetHeuristic(nbr, std::abs(goal.col - nx), std::abs(goal.row - ny), request.settings.heuristic);
                    openHeap.Push(nbr);
                    if (request.settings.debugColoring) terrain->set_color(ny, nx, Colors::Yellow);
                }
            }
            if (request.settings.singleStep) return PathResult::PROCESSING;
        }
        //handle error: no path exists
        return PathResult::IMPOSSIBLE;
        */


        return path;
    }
}

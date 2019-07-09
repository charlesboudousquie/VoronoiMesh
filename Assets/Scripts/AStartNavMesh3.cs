using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStartNavMesh3 : MonoBehaviour
{
    NodeHeap openHeap;
    public NavMesh3 navMesh;
    List<Vector3> moves;
    List<Vector3> smoothMoves, roughMoves;
    public bool readyToPathfind = false;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMesh3>();
        moves = new List<Vector3>();
        smoothMoves = new List<Vector3>();
        roughMoves = new List<Vector3>();
        if (openHeap == null) {
            openHeap = new NodeHeap();
        }
        if (navMesh != null && navMesh.nodes != null) {
            openHeap.InitHeap(navMesh.nodes);
            readyToPathfind = true;
        }
    }

    void OnMapReset() {
        if (openHeap == null) {
            openHeap = new NodeHeap();
        }
        openHeap.InitHeap(navMesh.nodes);
        readyToPathfind = true;
        Debug.Log("mesh loaded, numNodes = " + navMesh.nodes.Length);
        /*
        string res = "";
        for (int i = 0; i < navMesh.nodes.Length; i++) {
            res = res + "node " + i + ": ";
            for (int j = 0; j < 3; j++) {
                res = res + navMesh.nodes[i].GetNeighbors()[j] + ", ";
            }
            res = res + "\n";
        }
        Debug.Log(res);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            int start = Random.Range(0, navMesh.nodes.Length);
            int end = Random.Range(0, navMesh.nodes.Length);
            Debug.Log("navigating from " + start + " to " + end);
            moves = GetPath(openHeap.GetNode(start), openHeap.GetNode(end));
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            int start = 19;
            int end = 21;
            Debug.Log("navigating from " + start + " to " + end);
            moves = GetPath(openHeap.GetNode(start), openHeap.GetNode(end));
        }
        bool first = true;
        Vector3 last = Vector3.zero;
        foreach (Vector3 move in smoothMoves) {
            if (first) {
                last = move;
                first = false;
            }
            Debug.DrawLine(last, move, Color.black);
            last = move;
        }
        first = true;
        last = Vector3.zero;
        foreach (Vector3 move in roughMoves) {
            if (first) {
                last = move;
                first = false;
            }
            Debug.DrawLine(last, move, Color.gray, .01f, false);
            last = move;
        }
    }

    public List<Vector3> GetPathToSafeSpot(VoronoiNode start, float SafetyThreshold, out VoronoiNode safeSpot) {
        List<Vector3> path = new List<Vector3>();
        safeSpot = start;
        if (navMesh.nodeVisability[start.Id] < SafetyThreshold) return path;
        openHeap.ResetHeap();
        openHeap.Push(start.Id);

        int MaxNumLoops = 100000;
        while (!openHeap.IsEmpty() && MaxNumLoops > 0) {
            MaxNumLoops--;
            VoronoiNode least = openHeap.Pop();

            if (navMesh.nodeVisability[least.Id] < SafetyThreshold) {
                safeSpot = least;
                int numStops = 1;
                List<VoronoiNode> nodeList = new List<VoronoiNode>();
                while (least.Id != start.Id) {
                    numStops++;
                    path.Add(least.Position);
                    nodeList.Add(least);
                    least = openHeap.GetNode(least.Parent);
                }
                nodeList.Add(start);
                path.Add(start.Position);
                roughMoves = path;


                VoronoiNode[] nodeArr = new VoronoiNode[numStops];
                int i = 0;
                foreach (VoronoiNode n in nodeList) {
                    nodeArr[i] = n;
                    i++;
                }


                path = GetFunnelPath(nodeArr);
                smoothMoves = path;
                //funnel path!!
                return path;
            }

            //if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);
            int[] neighbors = least.GetNeighbors();
            for (int i = 0; i < 3; i++) {
                VoronoiNode nbr = openHeap.GetNode(neighbors[i]);
                if (!nbr.Open &&!nbr.Closed) {
                    openHeap.SetGiven(nbr.Id, least.Id, least.Given + Vector3.Distance(nbr.Position, least.Position));
                    openHeap.SetCost(nbr.Id);
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

    public List<Vector3> GetPath(VoronoiNode start, VoronoiNode end) {
        List<Vector3> path = new List<Vector3>();

        if (start.Id == end.Id) return path;
        openHeap.ResetHeap();
        openHeap.Push(start.Id);

        int MaxNumLoops = 100000;
        while (!openHeap.IsEmpty() && MaxNumLoops > 0) {
            MaxNumLoops--;
            VoronoiNode least = openHeap.Pop();

            if (least.Id == end.Id) {
                int numStops = 1;
                List<VoronoiNode> nodeList = new List<VoronoiNode>();
                while (least.Id != start.Id) {
                    numStops++;
                    path.Add(least.Position);
                    nodeList.Add(least);
                    least = openHeap.GetNode(least.Parent);
                }
                nodeList.Add(start);
                path.Add(start.Position);
                roughMoves = path;


                VoronoiNode[] nodeArr = new VoronoiNode[numStops];
                int i = 0;
                foreach (VoronoiNode n in nodeList) {
                    nodeArr[i] = n;
                    i++;
                }


                path = GetFunnelPath(nodeArr);
                smoothMoves = path;
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
                        openHeap.SetCost(nbr.Id);
                        openHeap.UpdateNode(nbr.Id);
                    }
                } else if (!nbr.Closed) {
                    openHeap.SetHeuristic(nbr.Id, Vector3.Distance(nbr.Position, end.Position));
                    openHeap.SetGiven(nbr.Id, least.Id, least.Given + Vector3.Distance(nbr.Position, least.Position));
                    openHeap.SetCost(nbr.Id);
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


    public VoronoiNode GetFurthestNode(Vector3 position) {
        VoronoiNode FurthestNode = openHeap.GetNode(0);

        openHeap.ResetHeap();
        openHeap.Push(0);

        int numCloser = 0;
        float furthestDist = 10000;

        int MaxNumLoops = 100000;
        while (!openHeap.IsEmpty() && MaxNumLoops > 0) {
            MaxNumLoops--;
            VoronoiNode least = openHeap.Pop();
            float distToPos = Vector3.SqrMagnitude(least.Position - position);

            if (distToPos > furthestDist) {
                FurthestNode = least;
                furthestDist = distToPos;
                numCloser = 0;
            } else {
                numCloser++;
                if (numCloser > 10) {
                    return FurthestNode;
                }
            }

            //if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);
            int[] neighbors = least.GetNeighbors();
            for (int i = 0; i < 3; i++) {
                VoronoiNode nbr = openHeap.GetNode(neighbors[i]);
                if (!nbr.Closed && !nbr.Open) {
                    openHeap.SetHeuristic(nbr.Id, 1/distToPos);
                    openHeap.SetCost(nbr.Id);
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

        return FurthestNode;
    }


    public VoronoiNode GetClosestNode(Vector3 position) {
        VoronoiNode ClosestNode = openHeap.GetNode(0);

        openHeap.ResetHeap();
        openHeap.Push(0);

        int numFurther = 0;
        float closestDist = 10000;

        int MaxNumLoops = 100000;
        while (!openHeap.IsEmpty() && MaxNumLoops > 0) {
            MaxNumLoops--;
            VoronoiNode least = openHeap.Pop();
            float distToPos = Vector3.SqrMagnitude(least.Position - position);

            if (distToPos < closestDist) {
                ClosestNode = least;
                closestDist = distToPos;
                numFurther = 0;
            } else {
                numFurther++;
                if (numFurther > 10) {
                    return ClosestNode;
                }
            }

            //if (request.settings.debugColoring) terrain->set_color(ly, lx, Colors::Blue);
            int[] neighbors = least.GetNeighbors();
            for (int i = 0; i < 3; i++) {
                VoronoiNode nbr = openHeap.GetNode(neighbors[i]);
                if (!nbr.Closed && !nbr.Open) {
                    openHeap.SetHeuristic(nbr.Id, distToPos);
                    openHeap.SetCost(nbr.Id);
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

        return ClosestNode;
    }



    public static Vector3 NormRelativeVect(Vector3 nodeCenter, Vector3 nodeNorm, Vector3 pnt) {
        return pnt - (nodeCenter + nodeNorm * Vector3.Dot(pnt - nodeCenter, nodeNorm));
    }

    bool isInFunnel(Vector3 left, Vector3 right, Vector3 test, out bool crossedFunnelBound) {
        float lMag = left.magnitude;
        float rMag = right.magnitude;
        float tMag = test.magnitude;
        float funnelAngle = Mathf.Acos(Vector3.Dot(left, right) / (lMag * rMag));
        float testLeftAngle = Mathf.Acos(Vector3.Dot(left, test) / (lMag * tMag));
        float testRightAngle = Mathf.Acos(Vector3.Dot(test, right) / (tMag * rMag));
        //Debug.Log("fAngle = " + funnelAngle + ", leftAngle = " + testLeftAngle + ", rightAngle = " + testRightAngle);
        crossedFunnelBound = false;
        if (testLeftAngle > funnelAngle || testRightAngle > funnelAngle) {
            if (testLeftAngle > testRightAngle) {
                crossedFunnelBound = true;
            }
            return false;
        }
        return true;
    }

    void AddSlicedWaypoints(Vector3 startPos, Vector3 startNorm, Vector3 endPos, List<Vector3> lines, ref List<Vector3> outputPath) {
        
        Vector3 sliceNormal = Vector3.Cross(startNorm, endPos - startPos).normalized;
        bool calculate = false;
        Vector3 p1 = Vector3.zero;
        if (lines == null) return;
        foreach (Vector3 p2 in lines) {
            if (Vector3.Distance(p2, endPos) < .01) break;
            if (calculate) {
                Vector3 lineDirection = (p2 - p1).normalized;
                Vector3 PointOnLine = p1;
                float numerator = Vector3.Dot((startPos - PointOnLine), sliceNormal);
                float denominator = Vector3.Dot(lineDirection, sliceNormal);
                if (denominator != 0 && numerator != 0) {
                    Vector3 intersection = PointOnLine + lineDirection.normalized * (numerator/denominator);
                    outputPath.Add(intersection);
                }
            }
            p1 = p2;
            calculate = !calculate;
        }
        

        outputPath.Add(endPos);
    }

    public List<Vector3> GetFunnelPath(VoronoiNode[] p) {
        List<Vector3> fPath = new List<Vector3>();
        List<Vector3> linesForSlice = new List<Vector3>();
        List<Vector3> startSharedVerts = p[1].GetBorderPoints(p[0]);
        linesForSlice.Add(startSharedVerts[0]);
        linesForSlice.Add(startSharedVerts[1]);

        VoronoiNode Start = p[0];
        Vector3 focusPosition = p[0].Position;
        Vector3 focusNormal = p[0].normal;
        fPath.Add(focusPosition);

        for (int i = 1; i < p.Length-1; i++) {
            List<Vector3> priorSharedVerts = p[i-1].GetBorderPoints(p[i]);
            Vector3 left = priorSharedVerts[0];
            Vector3 right = priorSharedVerts[1];

            if (Vector3.Distance(left, focusPosition) < .01f || Vector3.Distance(right, focusPosition) < .01f) {
                continue;
            }

            List<Vector3> sharedVerts = p[i].GetBorderPoints(p[i+1]);
            Vector3 newPoint = sharedVerts[0];
            Vector3 oldPoint = sharedVerts[1];
            if (sharedVerts[0] == priorSharedVerts[0] || sharedVerts[0] == priorSharedVerts[1]) {
                newPoint = sharedVerts[1];
                oldPoint = sharedVerts[0];
            }

            bool crossedFunnelBound = true;
            if (!isInFunnel(NormRelativeVect(focusPosition, focusNormal, left), NormRelativeVect(focusPosition, focusNormal, right), NormRelativeVect(focusPosition, focusNormal, newPoint), out crossedFunnelBound)) {
                //add the point in prioSharedVerts that isnt in sharedVerts
                Vector3 pointToAdd = left;
                if (crossedFunnelBound) {
                    pointToAdd = right;
                }
                AddSlicedWaypoints(focusPosition, focusNormal, pointToAdd, linesForSlice, ref fPath);
                linesForSlice = new List<Vector3>();
                focusNormal = p[i].normal;
                focusPosition = pointToAdd;
            }

            if (oldPoint != focusPosition && newPoint != focusPosition) {
                linesForSlice.Add(oldPoint);
                linesForSlice.Add(newPoint);
            }
        }

        //check if last point is in line, add last point to list

        int lastNode = p.Length - 1;
        List<Vector3> fsharedVerts = p[lastNode].GetBorderPoints(p[lastNode-1]);

        Vector3 fleft = fsharedVerts[0];
        Vector3 fright = fsharedVerts[1];

        bool fcrossedFunnelBound = true;
        if (!isInFunnel(NormRelativeVect(focusPosition, focusNormal, fleft), NormRelativeVect(focusPosition, focusNormal, fright), NormRelativeVect(focusPosition, focusNormal, p[lastNode].Position), out fcrossedFunnelBound)) {
            //add the point in prioSharedVerts that isnt in sharedVerts
            Vector3 pointToAdd = fleft;
            if (fcrossedFunnelBound) {
                pointToAdd = fright;
            }
            AddSlicedWaypoints(focusPosition, focusNormal, pointToAdd, linesForSlice, ref fPath);
            fPath.Add(p[lastNode].Position);
        } else {
            AddSlicedWaypoints(focusPosition, focusNormal, p[lastNode].Position, linesForSlice, ref fPath);
        }
        
        return fPath;
    }

}

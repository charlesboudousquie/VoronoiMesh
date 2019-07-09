using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DEBUG_TYPE
{
    NONE,
    ALL,
    NORMALS,
    VISABILITY_POV,
    VISABILITY_HIDING,
    PATH,
    PROXIMITY
}
[System.Serializable]
public class NavMesh3 : MonoBehaviour
{
    public VoronoiNode[] nodes;
    public bool ComputeNeighbors;
    public float[] nodeVisability;
    public float[] distanceFromPlayer;
    public DEBUG_TYPE debug;

    void Start()
    {
        if (ComputeNeighbors) {
            //First pass create all the nodes and add them to the node[]
            MeshFilter mf = GetComponent<MeshFilter>();
            Mesh mesh = mf.mesh;
            int size = mesh.triangles.Length / 3;//always divisiable by 3
            nodes = new VoronoiNode[size];

            for (int i = 0; i < size; ++i) {
                VoronoiNode v = new VoronoiNode(i);
                Vector3 normals = new Vector3(0, 0, 0);
                for (int tri = 0; tri < 3; ++tri) {
                    int ver_index = mesh.triangles[3 * i + tri];
                    v.AddPoint(mesh.vertices[ver_index]);
                    normals += mesh.normals[ver_index];
                }
                v.SetNormal(normals / 3.0f);
                nodes[i] = v;
            }

            //Second pass find the neighbors

            float threshold = .05f;
            for (int i = 0; i < size; ++i) {
                VoronoiNode v1 = nodes[i];
                Vector3[] vert1 = v1.GetVertices();
                for (int j = i + 1; j < size; ++j) {
                    if (i == j) continue;
                    VoronoiNode v2 = nodes[j];
                    Vector3[] vert2 = v2.GetVertices();
                    int count = 0;
                    List<Vector3> neighborVerts1 = new List<Vector3>();
                    List<Vector3> neighborVerts2 = new List<Vector3>();
                    for (int n1 = 0; n1 < 3; ++n1) {
                        for (int n2 = 0; n2 < 3; ++n2) {
                            if (Vector3.Distance(vert1[n1], vert2[n2]) < threshold) {
                                neighborVerts1.Add(vert2[n2]);//triangle 1 neighbor verts for 2 are added
                                neighborVerts2.Add(vert1[n1]);//triangle 2 neighbor verts for 1 are added
                                ++count;
                            }
                        }
                    }
                    if (count == 2) {
                        v1.AddNeighbor(v2);
                        //v1.MatchingVertices.Add(v2.Id, neighborVerts1);
                        v2.AddNeighbor(v1);
                        //v2.MatchingVertices.Add(v1.Id, neighborVerts2);
                    }
                }
            }
            distanceFromPlayer = new float[size];
            nodeVisability = new float[size];
        }

        BroadcastMessage("OnMapReset");
        Invoke("UpdateVisability", 0.25f);
    }

    void Update()
    {
        
    }

    void UpdateVisability()
    {
        Camera cam = Camera.main;
        Vector3 cam_normal = cam.transform.forward.normalized;
        Vector3 cam_pos = cam.transform.position;
        float degrees_fov = cam.fieldOfView;
        foreach(VoronoiNode node in nodes)
        {
            float dis_sq = Vector3.SqrMagnitude(cam_pos-node.Position);
            distanceFromPlayer[node.Id] = dis_sq;

            Vector3 line_to_node = cam_pos - node.Position;
            line_to_node = line_to_node.normalized;
            //cam normal and line between cam and node
            //dot product  > 0  within the field of view potentially
            //else not in the field of view 
            if (DEBUG_TYPE.NORMALS == debug) {
                float d = Vector3.Dot(line_to_node, node.normal);
                if (d < 0) {
                    d = 0.0f;
                }
                Color c =  Color.Lerp(Color.black, Color.magenta, d);
                DrawTriangle(node, c);
            } else if(debug == DEBUG_TYPE.VISABILITY_HIDING)
            {
                float d = Vector3.Dot(line_to_node, node.normal);
                if (d < 0)
                {
                    DrawTriangle(node, Color.red);
                }
            }
            if (Vector3.Dot(cam_normal,line_to_node) > 0)
            {
                nodeVisability[node.Id] = 0;
                //move to next node
                switch (debug)
                {
                    case DEBUG_TYPE.VISABILITY_HIDING:
                        DrawTriangle(node, Color.red);
                        break;
                    default:
                        break;
                }

                continue;
            }
            
            //line between cam and node
            //dot product < 0  visable
            //else not visable
            if(Vector3.Dot(node.normal,line_to_node) > 0)
            {
                nodeVisability[node.Id] = 1.0f;
                switch (debug)
                {
                    case DEBUG_TYPE.VISABILITY_POV:
                        DrawTriangle(node, Color.yellow);
                        break;
                    default:
                        break;
                }
                
            }
            else
            {
                nodeVisability[node.Id] = 0.0f;
                switch (debug)
                {
                    case DEBUG_TYPE.VISABILITY_POV:
                        DrawTriangle(node, Color.blue);
                        break;
                    default:
                        break;
                }
            }
        }
        //last step invoke in a quarter of a second
        Invoke("UpdateVisability", 0.25f);
    }

    public VoronoiNode GetNode(int _id)
    {
        return nodes[_id];
    }

    public VoronoiNode GetNode(Vector3 position)
    {
        foreach (VoronoiNode vN in nodes)
        {
            if(vN.Position == position)
            {
                return vN;
            }
        }
        return null;
    }
    private void DrawTriangle(VoronoiNode node, Color c)
    {
        Vector3 nl0 = node.vertices[0] - node.Position;
        nl0 = nl0 * .9f + node.Position;
        Vector3 nl1 = node.vertices[1] - node.Position;
        nl1 = nl1 * .9f + node.Position;
        Vector3 nl2 = node.vertices[2] - node.Position;
        nl2 = nl2 * .9f + node.Position;
        Debug.DrawLine(nl0, nl1, c, .25f, false);
        Debug.DrawLine(nl2, nl0, c, .25f, false);
        Debug.DrawLine(nl2, nl1, c, .25f, false);
    }
}



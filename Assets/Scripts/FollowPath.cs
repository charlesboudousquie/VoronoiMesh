using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowPath : MonoBehaviour
{
    public List<Vector3> path;
    AStartNavMesh3 AStarNav;
    Vector3 currentGoal;
    bool navMeshInitialized = false;

    public float speed = 5.0f;
    float epsilon = 0.001f;
    int index = 0;

    private GameObject AStarObject;

    void MoveTo(Vector3 goal)
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, goal, speed * Time.deltaTime);
    }

    void InitializeNavmesh()
    {
        AStarNav = gameObject.GetComponent<AStartNavMesh3>();
        if (AStarNav.navMesh != null && AStarNav.navMesh.nodes != null)
        {
            NavMesh3 navmesh = AStarNav.navMesh;
            VoronoiNode begin = navmesh.nodes[0];
            VoronoiNode end = navmesh.nodes[navmesh.nodes.Length - 1];

            path = new List<Vector3>(AStarNav.GetPath(begin, end));
            navMeshInitialized = true;
            currentGoal = path[index];
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeNavmesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (navMeshInitialized == false)
        {
            InitializeNavmesh();
        }
        else
        {
            MoveTo(currentGoal);

            if (Vector3.Distance(currentGoal, gameObject.transform.position) <= epsilon)
            {
                if (index < path.Count - 1)
                {
                    index++;
                    currentGoal = path[index];
                }
            }
        }
    }
}

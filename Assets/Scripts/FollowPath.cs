using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FollowPath : MonoBehaviour
{
    public List<Vector3> path;
    public AStartNavMesh3 AStarNav;
    public NavMesh3 navMesh;
    Vector3 currentGoal;
    bool navMeshInitialized = false;

    public float speed = 5.0f;
    float epsilon = 0.01f;
    public int index = 0;

    private GameObject AStarObject;
    private VoronoiNode currentNode;

    public float jumpForce;
    public float jumpDegrade;
    private Vector3 jumpDirection;
    private float TempSpeed;
    private float waitTime;

    public MeshRenderer mr, wmr;
    public GameObject wings;

    void MoveTo(Vector3 goal)
    {
        Vector3 movementDirection = goal - transform.position;
        if (movementDirection != Vector3.zero) {
            Quaternion rot = Quaternion.LookRotation(goal - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 100 * Time.deltaTime);
        }

        if (waitTime <= 0) {
            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, goal, (speed + TempSpeed) * Time.deltaTime) + jumpDirection * Time.deltaTime;
            jumpDirection *= jumpDegrade;
        } else {
            waitTime -= Time.deltaTime;
        }
    }

    void InitializeNavmesh()
    {
        //AStarNav = gameObject.GetComponent<AStartNavMesh3>();
        if (navMesh != null && AStarNav.readyToPathfind)
        {

            currentNode = navMesh.nodes[0];
            transform.position = currentNode.Position;
            GenerateRandomPath();
            navMeshInitialized = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        wings.SetActive(false);
        speed *= Random.Range(.9f, 1.1f);
        transform.localScale = transform.localScale * Random.Range(.5f, 1.1f);
        Color myRandomColor = new Color(0, Random.Range(0.3f, 1.0f), Random.Range(0.3f, 1.0f));
        mr.material.SetColor("_EmissionColor", myRandomColor);
        wmr.material.SetColor("_EmissionColor", myRandomColor * .8f);
        TempSpeed = 0;
        waitTime = Random.Range(0, 2);
        jumpDirection = Vector3.zero;
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

            if (Vector3.SqrMagnitude(currentGoal - gameObject.transform.position) <= epsilon) {
                TempSpeed = 0;
                wings.SetActive(false);
                if (index > 0) {
                    index--;
                    waitTime = Random.Range(0, 5);
                    if (waitTime < 2) {
                        waitTime = 0;
                    }
                    currentGoal = path[index];
                } else {
                    GenerateRandomPath();
                }
            }
        }
    }

    void GenerateRandomPath() {
        int rand = Random.Range(0, 5);
        VoronoiNode begin = currentNode;
        if (rand == 1) {
            //jump
            wings.SetActive(true);
            TempSpeed = 5;
            jumpDirection = currentNode.normal * jumpForce;
            begin = navMesh.nodes[Random.Range(0, navMesh.nodes.Length - 1)];
        }
        VoronoiNode end = navMesh.nodes[Random.Range(0,navMesh.nodes.Length - 1)];
        path = new List<Vector3>(AStarNav.GetPath(begin, end));
        currentNode = end;
        index = path.Count-1;
        currentGoal = path[index];
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class EnemyProximityBehavior : MonoBehaviour
{
    //assign proximity-based behaviours in architecture
    //distance 4: move randomly
    //{ distance 3: stop and look at player }
    //distance 2: move away from player until obscured
    //{ distance 1: fly away from player }

    //public Rigidbody rb;
    private GameObject player;
    private List<GameObject> textObjects;
    public GameObject GoalObject;
    private GameObject playerDebugText;

    public AStartNavMesh3 AStarNavMesh;
    public NavMesh3 mesh3;

    public GameObject enemyBody;

    public float playerDistanceVar;

    private bool initialized = false;

    public bool debugDrawingOn = true;

    public bool scriptEnabled = true;
    public bool randomMovementEnabled = true;
    public bool lookAtPlayerEnabled = true;
    public bool hideFromPlayerEnabled = true;
    public bool flyAwayFromPlayerEnabled = true;

    enum State { NONE, RANDOM_MOVEMENT, LOOKING, HIDING, FLYING };

    float epsilon = 0.001f;

    State currentState = State.NONE;

    public float speed = 2.0f;

    public float hidingDesire = 0.1f;
    public float minFleeDist = 1.0f;
    public float maxFleeDist = 4.0f;

    private VoronoiNode lastPosition;

    public float maxDistance = 4, moveRandomlyRadius = 3, lookAtPlayerRadius = 2, hideFromPlayerRadius = 1;

    TextMesh goalTextMesh;

    int pathListIndex;
    VoronoiNode targetNode;
    List<Vector3> currentPath;

    public GameObject wings;
    public MeshRenderer bmr, wmr;

    // depending on the state the goal will be displayed
    // showing what the current objective of the AI is
    void DisplayGoal()
    {
        goalTextMesh.transform.position = targetNode.Position;
        string currentGoal = "Current Goal: " + currentState.ToString();
        goalTextMesh.text = currentGoal;
        goalTextMesh.fontSize = 10;
    }

    void StopAndLookAtPlayer()
    {
        // get player position
        Vector3 playerPos = player.transform.position;

        // make our object look at player
        this.transform.LookAt(playerPos);
    }

    void FlyAwayFromPlayer()
    {

        wings.SetActive(true);

        // get player position
        Vector3 playerPos = player.transform.position;

        Vector3 vecToPlayer = playerPos - this.transform.position;
        vecToPlayer.Normalize();

        // set new destination
        // new destination is a node that is in the opposite direction of the player

        // first find random points
        VoronoiNode[] nodes = mesh3.nodes;
        for (int i = 0; i < nodes.Length; i++)
        {
            float distance = Vector3.Distance(nodes[i].Position, playerPos);
            if (distance >= minFleeDist && distance <= maxFleeDist)
            {
                // dot the diff vector with the vector between the node and the enemy
                Vector3 vecToNode = nodes[i].Position - this.transform.position;
                vecToNode.Normalize();

                // if node is in opposite direction of player
                // then it is acceptable
                float dotProduct = Vector3.Dot(vecToPlayer, vecToNode);
                if (dotProduct < 0)
                {
                    targetNode = nodes[i];
                    return;
                }
            }
        }
    }

    //VoronoiNode NewRandomPosition()
    //{
    //    // if nodes exist then choose random spot on sphere
    //    if(mesh3.nodes != null)
    //    {
    //        int randomIndex = Random.Range(0, mesh3.nodes.Length - 1);
    //        return mesh3.nodes[randomIndex];
    //    }
    //    return null;
    //    //Vector3 newPos = gameObject.transform.position;
    //    //newPos.x += Random.Range(-10, 10);
    //    //newPos.z += Random.Range(-10, 10);
    //    //return newPos;
    //}

    int RandomIndex()
    {
        return Random.Range(0, mesh3.nodes.Length - 1);
    }

    void MoveRandomly()
    {
        targetNode = mesh3.nodes[RandomIndex()];
    }

    VoronoiNode GetClosestNode(Vector3 position)
    {
        // todo
        return mesh3.nodes[0];
    }

    VoronoiNode GetNode(int id)
    {
        return mesh3.GetNode(id);
    }

    Vector3 GetNodePos(int id)
    {
        return mesh3.GetNode(id).Position;
    }

    void InitializePath()
    {
        if (AStarNavMesh.readyToPathfind == true)
        {
            lastPosition = mesh3.nodes[RandomIndex()];
            this.transform.position = lastPosition.Position;
            targetNode = mesh3.nodes[RandomIndex()];
            SetNewPath();
            initialized = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        GoalObject = new GameObject();
        goalTextMesh = GoalObject.AddComponent<TextMesh>();
        GoalObject.name = "Goal";

        playerDebugText = new GameObject();
        playerDebugText.AddComponent<TextMesh>();
        textObjects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            textObjects.Add(new GameObject());
            textObjects[i].AddComponent<TextMesh>();
            textObjects[i].name = "Debug Radius Line";
        }

        InitializePath();

        
        speed *= Random.Range(.9f, 1.1f);
        transform.localScale = transform.localScale * Random.Range(.5f, 1.1f);
        Color myRandomColor = new Color(0, Random.Range(0.3f, 1.0f), Random.Range(0.3f, 1.0f));
        bmr.material.SetColor("_EmissionColor", myRandomColor);
        wmr.material.SetColor("_EmissionColor", myRandomColor * .8f);

    }

    bool SeesPlayer()
    {
        RaycastHit rayHit;
        Vector3 rayDirection = player.transform.position - this.transform.position;
        rayDirection.Normalize();
        if (Physics.Raycast(transform.position, rayDirection, out rayHit))
        {
            if (rayHit.transform == player.transform)
            {
                // player was detected
                return true;
            }
        }

        return false;
    }

    void DrawDebugDistanceText(float distance)
    {
        TextMesh text = textObjects[0].GetComponent<TextMesh>();
        Vector3 debugDir = new Vector3(0, 0, 1);
        text.transform.position = this.transform.position + debugDir * distance;
        text.text = "Distance " + distance.ToString();
    }

    void ChangeState()
    {
        // find new state
        //float distance = playerDistanceVar;
        //float distance = Vector3.Distance(player.transform.position, this.transform.position);
        
        if (playerDistanceVar > moveRandomlyRadius)
        {
            currentState = State.RANDOM_MOVEMENT;
        }
        else if (playerDistanceVar > lookAtPlayerRadius)
        {
            currentState = State.LOOKING;
        }
        else if (playerDistanceVar > hideFromPlayerRadius)
        {
            currentState = State.HIDING;
        }
        else
        {
            currentState = State.FLYING;
        }
    }
    void DoStateAction()
    {
        switch (currentState)
        {
            case State.RANDOM_MOVEMENT:
                MoveRandomly();
                SetNewPath();
                break;
            case State.LOOKING:
                StopAndLookAtPlayer();
                break;
            case State.HIDING:
                SetNewPath();
                break;
            case State.FLYING:
                FlyAwayFromPlayer();
                SetNewPath();
                break;
            default:
                break;
        }
    }
    void SetNewPath()
    {
        if (currentState == State.HIDING)
        {
            currentPath = AStarNavMesh.GetPathToSafeSpot(lastPosition, hidingDesire, out lastPosition);
        }
        else
        {
            VoronoiNode end = mesh3.GetNode(targetNode.Position);
            currentPath = AStarNavMesh.GetPath(lastPosition, end);
            lastPosition = end;
        }

        if (currentPath.Count == 0)
        {
            currentState = State.NONE;
        }
        else
        {
            pathListIndex = currentPath.Count - 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptEnabled == false) { return; }
        if (initialized == false) { InitializePath(); return; }
        playerDistanceVar = Vector3.Distance(this.transform.position, player.transform.position);

        if (debugDrawingOn == true)
        {
            DisplayGoal();
            DebugDrawing();
        }

        // UNCOMMENT THIS WHEN DONE

        //distance 4: move randomly
        //{ distance 3: stop and look at player }
        //distance 2: move away from player until obscured
        //{ distance 1: fly away from player }
        if (currentState == State.NONE || currentState == State.LOOKING)
        {
            ChangeState();
            DoStateAction();
        }

        // traverse between our current node and target node
        if (currentState != State.NONE && currentState != State.LOOKING)
        {
            
            Vector3 movementDirection = currentPath[pathListIndex] - transform.position;
            if (movementDirection != Vector3.zero) {
                Quaternion rot = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, 100 * Time.deltaTime);
            }
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, currentPath[pathListIndex], speed * Time.deltaTime);

            if (Vector3.Distance(this.transform.position, currentPath[pathListIndex]) <= epsilon)
            {
                wings.SetActive(false);
                // update current position node
                pathListIndex--;
                if (pathListIndex == -1)
                {
                    currentState = State.NONE;
                }
            }
        }
    }

    void DebugDrawing()
    {
        Vector3 ourPos = this.transform.position;
        Vector3 forwardVector = this.transform.forward;
        Debug.DrawLine(ourPos, ourPos + 5 * forwardVector, Color.black);

        Vector3 debugDirection = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 playerPos = player.transform.position;

        // draw debug lines
        Debug.DrawLine(ourPos, ourPos + debugDirection * maxDistance, Color.green);
        DrawDebugDistanceText(maxDistance);

        Debug.DrawLine(ourPos, ourPos + debugDirection * moveRandomlyRadius, Color.red);
        DrawDebugDistanceText(moveRandomlyRadius);

        Debug.DrawLine(ourPos, ourPos + debugDirection * lookAtPlayerRadius, Color.blue);
        DrawDebugDistanceText(lookAtPlayerRadius);

        Debug.DrawLine(ourPos, ourPos + debugDirection * hideFromPlayerRadius, Color.white);
        DrawDebugDistanceText(hideFromPlayerRadius);

        //enemyBody.GetComponent<Renderer>().material.shader = Shader.Find("_Color");
        if (SeesPlayer())
        {
            Debug.DrawLine(ourPos, playerPos, Color.red);
            enemyBody.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
        }
        else
        {
            Debug.DrawLine(ourPos, playerPos, Color.blue);
            enemyBody.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
        }

        TextMesh playerTextDistance = playerDebugText.GetComponent<TextMesh>();
        playerTextDistance.text = "Distance to Player: " + Vector3.Distance(playerPos, ourPos).ToString();
        playerTextDistance.color = Color.red;
        playerTextDistance.fontSize = 8;
        playerTextDistance.transform.position = (playerPos + ourPos) / 2.0f;
    }
}


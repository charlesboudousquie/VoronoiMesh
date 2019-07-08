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

    public Rigidbody rb;
    private GameObject player;
    private List<GameObject> textObjects;
    private GameObject GoalObject;
    private GameObject playerDebugText;
    Renderer rend;

    NavMesh3 mesh3;

    public bool scriptEnabled = true;
    public bool randomMovementEnabled = true;
    public bool lookAtPlayerEnabled = true;
    public bool hideFromPlayerEnabled = true;
    public bool flyAwayFromPlayerEnabled = true;

    enum State { NONE, RANDOM_MOVEMENT, LOOKING, HIDING, FLYING };

    State currentState = State.NONE;
    private bool stateChanged = false;

    public float speed = 10.0f;

    public float minFleeDist = 5.0f;
    public float maxFleeDist = 20.0f;


    public Timer ActionCoolDowntimer;


    public float maxDistance = 40, moveRandomlyRadius = 30, lookAtPlayerRadius = 20, hideFromPlayerRadius = 10;

    Vector3 targetPosition;

    // depending on the state the goal will be displayed
    // showing what the current objective of the AI is
    void DisplayGoal()
    {
        TextMesh mesh = GoalObject.GetComponent<TextMesh>();
        mesh.transform.position = targetPosition;
        string currentGoal = "Current Goal: " + currentState.ToString();
        mesh.text = currentGoal;
    }

    void StopAndLookAtPlayer()
    {
        // get player position
        Vector3 playerPos = player.transform.position;

        // make our object look at player
        this.transform.LookAt(playerPos);

        // stop our object
        targetPosition = this.transform.position;
        rb.velocity = new Vector3(0, 0, 0);
    }

    void FlyAwayFromPlayer()
    {
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
                    targetPosition = nodes[i].Position;
                    return;
                }
            }
        }
    }

    Vector3 NewRandomPosition()
    {
        Vector3 newPos = gameObject.transform.position;

        newPos.x += Random.Range(-10, 10);
        newPos.z += Random.Range(-10, 10);

        return newPos;
    }

    void MoveRandomly()
    {
        targetPosition = NewRandomPosition();
    }

    // Start is called before the first frame update
    void Start()
    {
        ActionCoolDowntimer = new Timer();
        mesh3 = gameObject.GetComponent<AStartNavMesh3>().navMesh;
        player = GameObject.Find("Player");

        GoalObject = new GameObject();
        GoalObject.AddComponent<TextMesh>();
        GoalObject.name = "Goal";

        playerDebugText = new GameObject();
        playerDebugText.AddComponent<TextMesh>();

        targetPosition = NewRandomPosition();
        rend = this.GetComponent<Renderer>();

        textObjects = new List<GameObject>();
        for (int i = 0; i < 4; i++)
        {
            textObjects.Add(new GameObject());
            textObjects[i].AddComponent<TextMesh>();
            textObjects[i].name = "Debug Radius Line";
        }
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
        State previousState = currentState;

        // find new state
        float distance = Vector3.Distance(player.transform.position, this.transform.position);
        if (distance > maxDistance)
        {
            currentState = State.NONE;
        }
        else if (distance > moveRandomlyRadius)
        {
            currentState = State.RANDOM_MOVEMENT;
        }
        else if (distance > lookAtPlayerRadius)
        {
            currentState = State.LOOKING;
        }
        else if (distance > hideFromPlayerRadius)
        {
            currentState = State.HIDING;
        }
        else
        {
            currentState = State.FLYING;
        }

        // determine if state changed
        if (currentState != previousState)
        {
            stateChanged = true;
        }
        else
        {
            stateChanged = false;
        }
    }
    void DoStateAction()
    {
        switch (currentState)
        {
            case State.RANDOM_MOVEMENT:
                if (ActionCoolDowntimer.LimitReached())
                {
                    MoveRandomly();
                    ActionCoolDowntimer.Reset();
                }
                break;
            case State.LOOKING:
                StopAndLookAtPlayer();
                break;
            case State.HIDING:
                // hide from player
                ActionCoolDowntimer.Reset();
                break;
            case State.FLYING:
                if (ActionCoolDowntimer.LimitReached())
                {
                    FlyAwayFromPlayer();
                    ActionCoolDowntimer.Reset();
                }
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptEnabled == false)
        {
            return;
        }

        DisplayGoal();
        
        Vector3 ourPos = this.transform.position;
        Vector3 debugDirection = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 forwardVector = this.transform.forward;
        Debug.DrawLine(ourPos, ourPos + 5 * forwardVector, Color.black);

        // draw debug lines
        Debug.DrawLine(ourPos, ourPos + debugDirection * maxDistance, Color.green);
        DrawDebugDistanceText(maxDistance);

        Debug.DrawLine(ourPos, ourPos + debugDirection * moveRandomlyRadius, Color.red);
        DrawDebugDistanceText(moveRandomlyRadius);

        Debug.DrawLine(ourPos, ourPos + debugDirection * lookAtPlayerRadius, Color.blue);
        DrawDebugDistanceText(lookAtPlayerRadius);

        Debug.DrawLine(ourPos, ourPos + debugDirection * hideFromPlayerRadius, Color.white);
        DrawDebugDistanceText(hideFromPlayerRadius);
        
        Vector3 playerPos = player.transform.position;

        ChangeState();
        // 4 cases
        //distance 4: move randomly
        //{ distance 3: stop and look at player }
        //distance 2: move away from player until obscured
        //{ distance 1: fly away from player }
        DoStateAction();
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, speed * Time.deltaTime);

        rend.material.shader = Shader.Find("_Color");
        if (SeesPlayer())
        {
            Debug.DrawLine(ourPos, playerPos, Color.red);
            rend.material.SetColor("_Color", Color.red);
        }
        else
        {
            Debug.DrawLine(ourPos, playerPos, Color.blue);
            rend.material.SetColor("_Color", Color.blue);
        }
        TextMesh playerTextDistance = playerDebugText.GetComponent<TextMesh>();
        playerTextDistance.text = "Distance to Player: " + Vector3.Distance(playerPos, ourPos).ToString();
        playerTextDistance.transform.position = (playerPos + ourPos) / 2.0f;
    }
}

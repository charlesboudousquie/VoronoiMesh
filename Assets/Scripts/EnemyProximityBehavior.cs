using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class EnemyProximityBehavior : MonoBehaviour
{

    enum State { NONE, RANDOM_MOVEMENT, LOOKING, HIDING, FLYING };

    public GameObject player, enemyBody, wings;
    public AStartNavMesh3 AStarNavMesh;
    public NavMesh3 mesh3;
    public float RandomVariation = .1f;
    public float speed = 2.0f, rotSpeed = 100, hidingDesire = 0.1f, jumpDecay = .99f, moveRandomlyRadius = 3, lookAtPlayerRadius = 2, hideFromPlayerRadius = 1;
    public MeshRenderer bmr, wmr;

    private bool initialized = false;
    private float epsilon = 0.001f, tempSpeed;
    private State currentState = State.NONE;
    private VoronoiNode lastPosition;
    private int pathListIndex;
    private VoronoiNode targetNode;
    private List<Vector3> currentPath;
    private Vector3 jumpDirection;
    private Color SafeColor, LookColor, HideColor, FlyColor;
    private int numTimesToStop;

    void StopAndLookAtPlayer()
    {
        // get player position
        numTimesToStop = Random.Range(4, 10);
        // make our object look at player
        CheckIfStillInView();
    }

    void CheckIfStillInView() {
        if (currentState == State.LOOKING) {
            Vector3 playerPos = player.transform.position;
            this.transform.LookAt(playerPos);
            ChangeState();
            if (currentState != State.LOOKING) {
                DoStateAction();
            } else {
                numTimesToStop--;
                if (numTimesToStop < 0) {
                    currentState = State.RANDOM_MOVEMENT;
                    currentPath = AStarNavMesh.GetSafeRandomPath(lastPosition, hidingDesire, out lastPosition);
                    CheckForBadPath();
                } else {
                    Invoke("CheckIfStillInView", .5f);
                }
            }
        }
    }

    void FlyAwayFromPlayer()
    {
        //launch away from current surface and land on a node far from the player
        wings.SetActive(true);
        jumpDirection = lastPosition.normal * 2 * tempSpeed;
        Vector3 playerPos = player.transform.position;
        lastPosition = AStarNavMesh.GetFurthestNode(playerPos, 20);
        currentPath = new List<Vector3>();
        currentPath.Add(lastPosition.Position);
        pathListIndex = 0;
    }

    int RandomIndex()
    {
        return Random.Range(0, mesh3.nodes.Length - 1);
    }

    void InitializePath()
    {
        if (AStarNavMesh.readyToPathfind == true)
        {
            lastPosition = mesh3.nodes[RandomIndex()];
            this.transform.position = lastPosition.Position;
            currentState = State.NONE;
            initialized = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializePath();

        //give each agent a slightly different set of parameters
        speed *= Random.Range(1 - RandomVariation, 1 + RandomVariation);
        tempSpeed = speed;
        transform.localScale = transform.localScale * Random.Range(.5f, 1.1f);
        moveRandomlyRadius *= Random.Range(1 - RandomVariation, 1 + RandomVariation);
        lookAtPlayerRadius *= Random.Range(1 - RandomVariation, 1 + RandomVariation);
        hideFromPlayerRadius *= Random.Range(1 - RandomVariation, 1 + RandomVariation);

        float lowColor1 = Random.Range(0.0f, 0.5f);
        float highColor1 = Random.Range(0.5f, 1.0f);
        float lowColor2 = Random.Range(0.0f, 0.5f);
        float highColor2 = Random.Range(0.5f, 1.0f);
        SafeColor = new Color(lowColor1, highColor1, lowColor2);
        LookColor = new Color(lowColor1, lowColor2, highColor1);
        HideColor = new Color(highColor1, highColor2, lowColor1);
        FlyColor = new Color(highColor1, lowColor1, lowColor2);
        }

    void ChangeState()
    {
        // find new state
        float playerDistanceVar = Vector3.SqrMagnitude(this.transform.position - player.transform.position);
        State priorState = currentState;
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
        if (currentState != priorState) {
            ChangeMyColor();
        }
    }

    void ChangeMyColor() {
        Color myRandomColor = Color.white;
        switch (currentState) {
            case (State.RANDOM_MOVEMENT):
                myRandomColor = SafeColor;
                break;
            case (State.LOOKING):
                myRandomColor = LookColor;
                break;
            case (State.HIDING):
                myRandomColor = HideColor;
                break;
            case (State.FLYING):
                myRandomColor = FlyColor;
                break;
            default:
                break;
        }
        bmr.material.SetColor("_EmissionColor", myRandomColor);
        wmr.material.SetColor("_EmissionColor", myRandomColor * .8f);
    }

    void DoStateAction()
    {
        switch (currentState)
        {
            case State.RANDOM_MOVEMENT:
                GetPathToRandom();
                break;
            case State.LOOKING:
                StopAndLookAtPlayer();
                break;
            case State.HIDING:
                GetPathToHidden();
                break;
            case State.FLYING:
                FlyAwayFromPlayer();
                break;
            default:
                break;
        }
    }

    void GetPathToHidden() {
        currentPath = AStarNavMesh.GetPathToSafeSpot(lastPosition, hidingDesire, out lastPosition);
        CheckForBadPath();
    }

    void GetPathToRandom() {
        VoronoiNode end = mesh3.nodes[RandomIndex()];
        currentPath = AStarNavMesh.GetPath(lastPosition, end);
        lastPosition = end;
        CheckForBadPath();
    }

    void CheckForBadPath() {
        if (currentPath.Count == 0) {
            currentState = State.NONE;
        } else {
            pathListIndex = currentPath.Count - 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized == false) { InitializePath(); return; }

        if (currentState == State.NONE)
        {
            ChangeState();
            DoStateAction();
        }
        else if (currentState != State.LOOKING)
        {
            Vector3 movementDirection = currentPath[pathListIndex] - transform.position;
            if (movementDirection != Vector3.zero) {
                Quaternion rot = Quaternion.LookRotation(movementDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotSpeed * Time.deltaTime);
            }

            if (wings.activeInHierarchy) {
                this.transform.position = Vector3.MoveTowards(this.transform.position, currentPath[pathListIndex], (speed + tempSpeed) * Time.deltaTime) + jumpDirection * Time.deltaTime;
                jumpDirection *= jumpDecay;
            } else {
                this.transform.position = Vector3.MoveTowards(this.transform.position, currentPath[pathListIndex], speed * Time.deltaTime);
            }

            if (Vector3.SqrMagnitude(this.transform.position - currentPath[pathListIndex]) <= epsilon)
            {
                wings.SetActive(false);
                // update current position node
                
                pathListIndex--;
                if (pathListIndex == -1)
                {
                    currentState = State.NONE;
                }
                else
                {
                    //we are pathfinding, but our state has changed, recalc current node and 
                    EnemyProximityBehavior.State priorState = currentState;
                    ChangeState();
                    if (currentState != priorState) {
                        lastPosition = AStarNavMesh.GetClosestNode(transform.position);
                        DoStateAction();
                    }
                }
            }
        }
    }
}


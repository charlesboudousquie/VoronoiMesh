using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
    Renderer rend;

    float timer = 0.0f;

    float speed = 10.0f;

    public float moveRandomlyRadius = 15, lookAtPlayerRadius = 10, hideFromPlayerRadius = 5;

    Vector3 targetPosition;

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

        Vector3 diff = playerPos - this.transform.position;
        diff.Normalize();

        // set new destination
        targetPosition = this.transform.position - 10.0f * diff;
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
        player = GameObject.Find("Player");
        targetPosition = NewRandomPosition();
        rend = this.GetComponent<Renderer>();
        textObjects = new List<GameObject>();
        for(int i = 0; i < 4; i++)
        {
            textObjects.Add(new GameObject());
            textObjects[i].AddComponent<TextMesh>();
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

    // Update is called once per frame
    void Update()
    {
        Vector3 ourPos = this.transform.position;
        Vector3 debugDirection = new Vector3(0.0f, 0.0f, 1.0f);
        Vector3 forwardVector = this.transform.forward;
        Debug.DrawLine(ourPos, ourPos + 5 * forwardVector, Color.black);

        // draw debug lines
        Debug.DrawLine(ourPos, ourPos + debugDirection * 20.0f, Color.green);
        TextMesh mesh1 = textObjects[0].GetComponent<TextMesh>();
        mesh1.transform.position = ourPos + debugDirection * 20.0f;
        mesh1.text = "Distance 20";
        
        Debug.DrawLine(ourPos, ourPos + debugDirection * moveRandomlyRadius, Color.red);
        TextMesh mesh2 = textObjects[1].GetComponent<TextMesh>();
        mesh2.transform.position = ourPos + debugDirection * moveRandomlyRadius;
        mesh2.text = "Distance 15";

        Debug.DrawLine(ourPos, ourPos + debugDirection * lookAtPlayerRadius, Color.blue);
        TextMesh mesh3 = textObjects[2].GetComponent<TextMesh>();
        mesh3.transform.position = ourPos + debugDirection * lookAtPlayerRadius;
        mesh3.text = "Distance 10";

        Debug.DrawLine(ourPos, ourPos + debugDirection * hideFromPlayerRadius, Color.white);
        TextMesh mesh4 = textObjects[3].GetComponent<TextMesh>();
        mesh4.transform.position = ourPos + debugDirection * hideFromPlayerRadius;
        mesh4.text = "Distance 5";

        timer += Time.deltaTime;

        rend.material.shader = Shader.Find("_Color");

        Vector3 playerPos = player.transform.position;

        if (SeesPlayer())
        {
            Debug.DrawLine(ourPos, playerPos, Color.red);

            rend.material.SetColor("_Color", Color.red);
            float distance = Vector3.Distance(ourPos, playerPos);
            // 4 cases
            //distance 4: move randomly
            //{ distance 3: stop and look at player }
            //distance 2: move away from player until obscured
            //{ distance 1: fly away from player }

            // if too far away dont move
            if (distance >= 20.0f)
            {
                targetPosition = this.transform.position;
            }
            else if (distance >= moveRandomlyRadius)
            {
                if (timer > 2.0f)
                {
                    timer = 0;
                    MoveRandomly();
                }
            }
            else if (distance >= lookAtPlayerRadius)
            {
                StopAndLookAtPlayer();
            }
            else if (distance >= hideFromPlayerRadius)
            {

            }
            else
            {
                FlyAwayFromPlayer();
            }

            gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            Debug.DrawLine(ourPos, playerPos, Color.blue);
            rend.material.SetColor("_Color", Color.blue);
        }
    }
}

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
    Renderer rend;

    float timer = 0.0f;

    float speed = 10.0f;

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
        //Handles.Label(ourPos + 5 * forwardVector, "Forward Vector");

        // draw debug lines
        Debug.DrawLine(ourPos, ourPos + debugDirection * 20.0f, Color.green);
        TextMesh mesh1 = gameObject.AddComponent<TextMesh>();
        mesh1.transform.position = ourPos + debugDirection * 20.0f;
        mesh1.text = "Distance 20";


        

        Debug.DrawLine(ourPos, ourPos + debugDirection * 15.0f, Color.red);
        TextMesh mesh2 = gameObject.AddComponent<TextMesh>();
        mesh2.transform.position = ourPos + debugDirection * 15;
        mesh2.text = "Distance 15";

        Debug.DrawLine(ourPos, ourPos + debugDirection * 10.0f, Color.blue);
        TextMesh mesh3 = gameObject.AddComponent<TextMesh>();
        mesh3.transform.position = ourPos + debugDirection * 10;
        mesh3.text = "Distance 10";

        Debug.DrawLine(ourPos, ourPos + debugDirection * 5.0f, Color.white);
        TextMesh mesh4 = gameObject.AddComponent<TextMesh>();
        mesh4.transform.position = ourPos + debugDirection * 5;
        mesh4.text = "Distance 5";

        timer += Time.deltaTime;
        //int shaderID = rend.material.shader.GetInstanceID();
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
                //Handles.Label(ourPos, "Doing Nothing");

                targetPosition = this.transform.position;
            }
            else if (distance >= 15.0f)
            {
                //Handles.Label(ourPos, "Moving Randomly");

                if (timer > 2.0f)
                {
                    timer = 0;
                    MoveRandomly();
                }
            }
            else if (distance >= 10.0f)
            {
                //Handles.Label(ourPos, "Looking At Player");
                StopAndLookAtPlayer();
            }
            else if (distance >= 5.0f)
            {
                //Handles.Label(ourPos, "Moving From Player Until Hidden");

            }
            else
            {
                //Handles.Label(ourPos, "Flying away from Player");

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

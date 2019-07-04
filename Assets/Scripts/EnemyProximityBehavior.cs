using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProximityBehavior : MonoBehaviour
{
    //assign proximity-based behaviours in architecture
    //distance 4: move randomly
    //{ distance 3: stop and look at player }
    //distance 2: move away from player until obscured
    //{ distance 1: fly away from player }

    float timer = 0.0f;

    float speed = 10.0f;

    Vector3 targetPosition;

    Vector3 NewPosition()
    {
        Vector3 newPos = gameObject.transform.position;

        newPos.x += Random.Range(-10, 10);
        newPos.z += Random.Range(-10, 10);

        return newPos;
    }

    void MoveRandomly()
    {
        targetPosition = NewPosition();
    }


    // Start is called before the first frame update
    void Start()
    {
        targetPosition = NewPosition();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, targetPosition, speed * Time.deltaTime);
        
        timer += Time.deltaTime;

        if(timer > 2.0f)
        {
            timer = 0;
            MoveRandomly();
        }
    }
}

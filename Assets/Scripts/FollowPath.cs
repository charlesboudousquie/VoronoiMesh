using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public List<Vector3> path;

    Vector3 currentGoal;

    float epsilon = 0.001f;
    int index = 0;

    void MoveTo(Vector3 goal)
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, goal, Time.deltaTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        path = new List<Vector3>();

        path.Add(new Vector3(10, 0, 0));
        currentGoal = path[index];
    }

    // Update is called once per frame
    void Update()
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public Stack<Vector3> path;

    Vector3 currentGoal;

    float epsilon = 0.001f;

    void MoveTo(Vector3 goal)
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, goal, Time.deltaTime);
    }

    // Start is called before the first frame update
    void Start()
    {
        path = new Stack<Vector3>();
        //path.Push(new Vector3(0,0,0));
        path.Push(new Vector3(10, 0, 0));
        //path.Push(new Vector3(0,0,0));
        //path.Push(new Vector3(0,0,0));
        //path.Push(new Vector3(0,0,0));
        currentGoal = path.Pop();
    }

    // Update is called once per frame
    void Update()
    {
        MoveTo(currentGoal);

        if (Vector3.Distance(currentGoal, gameObject.transform.position) <= epsilon)
        {
            if (path.Count > 0)
            {
                currentGoal = path.Pop();
            }
        }
    }
}

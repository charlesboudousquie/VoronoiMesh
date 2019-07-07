using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float maxTime = 2;
    private float currentTime = 0;

    public bool LimitReached()
    {
        return currentTime >= maxTime;
    }

    public void Reset()
    {
        currentTime = 0;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
    }
}

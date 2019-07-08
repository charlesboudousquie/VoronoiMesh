using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingSpinner : MonoBehaviour
{
    public float speed;
    private Vector3 rotateOrientation;
    // Start is called before the first frame update
    void Start()
    {
        rotateOrientation = new Vector3(0, 0, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localRotation = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z + speed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFlyMovement : MonoBehaviour
{
    public float thrustSpeed, strafeSpeed, rotateSpeed;
    private Vector3 mousePos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            transform.Translate(Vector3.forward * thrustSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S)) {
            transform.Translate(-Vector3.forward * thrustSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.Translate(-Vector3.right * strafeSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.Translate(Vector3.right * strafeSpeed * Time.deltaTime);
        }
        if (Input.GetMouseButtonDown(0)) {
            mousePos = Input.mousePosition;
        } else if (Input.GetMouseButton(0)) {
            Vector3 mouseDelta = Input.mousePosition - mousePos;
            transform.RotateAround(Vector3.up, -mouseDelta.x * rotateSpeed * Time.deltaTime);
            transform.RotateAround(transform.right, mouseDelta.y * rotateSpeed * Time.deltaTime);
            mousePos = Input.mousePosition;
        }
    }
}

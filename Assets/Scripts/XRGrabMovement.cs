using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRGrabMovement : MonoBehaviour
{
    private Vector3 LastPos;
    public Transform lController, rController;
    private bool lGripped, rGripped;
    private Vector3 p0, p1, p2, p3, releaseDirection;
    public float releaseDecay, releaseScale, positionInterval;
    // Start is called before the first frame update
    void Start()
    {
        lGripped = false;
        rGripped = false;
        p0 = p1 = p2 = p3 = transform.position;
        SetMovementOffset();
    }

    bool isLeftDown() {
        return Input.GetAxis("LTrigger_Quest") > .9 || Input.GetAxis("LTrigger_Rift") > .9;
    }

    bool isRightDown() {
        return Input.GetAxis("RTrigger_Quest") > .9 || Input.GetAxis("RTrigger_Rift") > .9;
    }

    private void SetMovementOffset() {
        p3 = p2;
        p2 = p1;
        p1 = p0;
        p0 = transform.position;
        Invoke("SetMovementOffset", positionInterval);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetJoystickNames().Length > 0) {
            if (isRightDown() && isLeftDown()) {
                Vector3 currPosition = (rController.position + lController.position) / 2;
                if (lGripped && rGripped) {
                    transform.Translate(-(currPosition - LastPos));
                } else {
                    lGripped = true;
                    rGripped = true;
                }
                LastPos = currPosition;
            } else if (isRightDown()) {
                Vector3 currPosition = rController.position;
                if (rGripped) {
                    transform.Translate(-(currPosition - LastPos));
                } else {
                    rGripped = true;
                }
                LastPos = currPosition;
                lGripped = false;
            } else if (isLeftDown()) {
                Vector3 currPosition = lController.position;
                if (lGripped) {
                    transform.Translate(-(currPosition - LastPos));
                } else {
                    lGripped = true;
                }
                LastPos = currPosition;
                rGripped = false;
            } else {
                if (lGripped || rGripped) {
                    lGripped = false;
                    rGripped = false;
                    releaseDirection = (p1 - p3) * releaseScale;
                }
                transform.position += releaseDirection * Time.deltaTime;
                releaseDirection *= releaseDecay;
            }
        }
    }
}

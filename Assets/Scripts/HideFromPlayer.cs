using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideFromPlayer : MonoBehaviour
{
    private GameObject player;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    bool CheckIfPlayerVisible()
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

    void SeekCover()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 ourPos = this.gameObject.transform.position;



    }

    // Update is called once per frame
    void Update()
    {
        // if palyer is visible then we should seek out nearest area of cover
        if(CheckIfPlayerVisible() == true)
        {

        }
    }
}

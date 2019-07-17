using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMgmt : MonoBehaviour {
    public NavMesh3 navMesh;
    public EnemyManager em;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //just mesh
            em.numEnemies = 0;
            navMesh.debug = DEBUG_TYPE.NONE;
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            //terrain analysis
            em.numEnemies = 0;
            navMesh.debug = DEBUG_TYPE.VISABILITY_POV;
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            //one agent
            em.numEnemies = 1;
            navMesh.debug = DEBUG_TYPE.NONE;
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            //many agents
            em.numEnemies = 3000;
            navMesh.debug = DEBUG_TYPE.NONE;
        }
    }
}

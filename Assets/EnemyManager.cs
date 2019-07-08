using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;
    public int numEnemies;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numEnemies; i++) {
            Instantiate(enemy, this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemy;
    public int numEnemies;
    private float internalNumEnemies;
    private GameObject[] enemies;
    // Start is called before the first frame update
    void Start()
    {
        InitEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        if (numEnemies != internalNumEnemies) {
            for (int i = 0; i < internalNumEnemies; i++) {
                 Destroy(enemies[i]);
            }
            InitEnemies();
        }
    }

    private void InitEnemies() {
        internalNumEnemies = numEnemies;
        enemies = new GameObject[numEnemies];
        for (int i = 0; i < numEnemies; i++) {
            enemies[i] = Instantiate(enemy, this.transform) as GameObject;
            enemies[i].SetActive(true);
        }
    }
}

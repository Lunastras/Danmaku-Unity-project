using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //if not set in editor, it will self destruct
    public GameObject[] enemiesToSpawn = null;

    //misc arrays
    //default length = 1
    public int[] numberPerEnemy = null; //def value = 1;
    public float[] respawnsTime = null; //def value = 0.5f;
    public float[] timeBetweenEnemies = null; //def value = 1;

    public float zRotationSpawn;
    //public int 

    // Start is called before the first frame update
    void Start()
    {
   
        if(enemiesToSpawn != null)
        {
            int length = enemiesToSpawn.Length;

            if(numberPerEnemy == null)
            {
                numberPerEnemy = new int[1];
                numberPerEnemy[0] = 1;
            }

            if (respawnsTime == null)
            {
                respawnsTime = new float[1];
                respawnsTime[0] = 0.5f;
            }

            if (timeBetweenEnemies == null)
            {
                timeBetweenEnemies = new float[1];
                timeBetweenEnemies[0] = 1;
            }

            StartCoroutine(SpawnEnemies());

        } else {
            Destroy(gameObject);
        }
    }

    IEnumerator SpawnEnemies()
    {
        for(int i = 0; i < enemiesToSpawn.Length; i++)
        {
            yield return new WaitForSeconds(timeBetweenEnemies[i % timeBetweenEnemies.Length]);

            for (int j = 0; j < (numberPerEnemy[i % numberPerEnemy.Length]); j++)
            {
                yield return new WaitForSeconds(respawnsTime[i % respawnsTime.Length]);
                GameObject spawnedEnemy = Instantiate(enemiesToSpawn[i]);
                spawnedEnemy.transform.position = transform.position;
            }
        }
    }

    
}



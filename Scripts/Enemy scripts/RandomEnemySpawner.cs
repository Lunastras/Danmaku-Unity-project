using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemySpawner : MonoBehaviour
{
    public bool isActive;
    private GameObject[] enemiesList;
    public int movementCoef = 1;
    public int rotationCoef = 1;
    public float initialZRotation = -90;
    public float delayBetweenSpawnsAvg = 8;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.gameManager;
        enemiesList = FindObjectOfType<EnemyManager>().enemiesToSpawn;
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while(!gameManager.IsGameOver())
        {
            float delayTillNextSpawn = delayBetweenSpawnsAvg * ((float)GameManager.GetRandomInt() / 100.0f) + 2f;
            int enemyIndexToSpawn = Mathf.RoundToInt(((float)GameManager.GetRandomInt() * ((float)enemiesList.Length / 100.0f)));
            enemyIndexToSpawn = Mathf.Clamp(enemyIndexToSpawn, 0, enemiesList.Length - 1);

            GameObject enemy = Instantiate(enemiesList[enemyIndexToSpawn]);
            ObjectMovement movement = enemy.GetComponent<ObjectMovement>();
            Transform enemyTransform = enemy.transform;

            movement.zRotation = initialZRotation;
            enemyTransform.position = transform.position;
            movement.SetOrientation(rotationCoef, movementCoef);

            yield return new WaitForSeconds(delayTillNextSpawn);
        }
    }

    
}

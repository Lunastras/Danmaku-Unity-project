using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public int health = 100;
    public int points;

    //number of upgrades to spawn when killed
    //0 = small
    //1 = medium
    //2 = big
    public int[] numOfCollectiblesToSpawn;
    public GameObject[] collectibles;
    public GameObject deathParticles = null;

    private AudioManager audio;



    // Start is called before the first frame update
    void Start()
    {
        GameManager.gameManager.EnemySpawned(this.gameObject);
        audio = AudioManager.instance;
    }



    public void TakeDamage(int dmgVal)
    {
        health -= dmgVal;
        audio.Play("damage");

        if (health < 0)
        {
            for(int i = 0; i < collectibles.Length; i++)
            {
                for(int j = 0; j < numOfCollectiblesToSpawn[i]; j++)
                {
                    Instantiate(collectibles[i]).transform.position = transform.position;
                }
            }

            if(GameManager.gameManager.isTutorial)
            {
                FindObjectOfType<TutorialManager>().PhaseFinished();
            }

            GameManager.gameManager.EnemyDestroyed(this.gameObject);
            FindObjectOfType<PlayerStats>().AddScorePoints(points);

            GameObject particle = Instantiate(deathParticles);
            particle.transform.position = transform.position;
            audio.Play("enemyDie1");


            Destroy(gameObject);
        }   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        //bullets and enemy border
        if (other.gameObject.layer == 12)
        {
            Destroy(gameObject);
        }    
    }
}

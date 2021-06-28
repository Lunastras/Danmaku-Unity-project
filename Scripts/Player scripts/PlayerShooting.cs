using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bullet;
    public GameObject bulletWhenCrouched;
    public float fireRate = 0.1f;
    public int rotationSense = 1; //1 or -1
    public int movementSense = 1; //1 or -1
    public BulletTypes normalBulletType;
    public BulletTypes bulletTypeWhenCrouched;
    private AudioManager audio;


    private bool canFire = true;
    private GameManager gameManager;
    private PlayerController player;

    private bool isFiring = false;
    private bool isCrouching = false;

    void Start()
    {
        gameManager = GameManager.gameManager;
        player = FindObjectOfType<PlayerController>();
        audio = AudioManager.instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!player.canMove)
            return;           

        isCrouching = gameManager.isCrouching;
        isFiring = gameManager.isFiring;

        BulletTypes bulletTypeToFire;
        GameObject bulletToFire;

        if (isCrouching)
        {
            bulletTypeToFire = bulletTypeWhenCrouched;
            bulletToFire = bullet;
        } else
        {
            bulletTypeToFire = normalBulletType;
            bulletToFire = bulletWhenCrouched;
        }

        if (isFiring && canFire)
        {
            audio.Play("playerShoot");
            GameObject createdBullet = GameObject.Instantiate(bulletToFire);
            if(createdBullet != null)
            {
                Vector3 position = transform.position;
                canFire = false;
                StartCoroutine(FireCoolDown());
                Color color = new Color(0.4f, 1, 0.69f);
                color.a = 0.5f;
                createdBullet.GetComponent<SpriteRenderer>().color = color;


                createdBullet.layer = 8; //player bullet layer
                createdBullet.transform.position = position;

                BulletBehaviour bullet = createdBullet.GetComponent<BulletBehaviour>();
                bullet.SetZRotation(transform.rotation.eulerAngles.z + 90);
                bullet.SetOrientation(rotationSense, movementSense);

                switch (bulletTypeToFire)
                {
                    case BulletTypes.Aiming:
                        Transform enemyTransform = GetClosestEnemyTransform(position);

                        if(enemyTransform != null)
                        {
                            Vector3 targetPos = GetClosestEnemyTransform(position).position;
                            Vector3 directionToTarget = (targetPos - position).normalized;
                            bullet.SetZRotation(Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg);
                        }
                        break;

                    case BulletTypes.Homing:
                        bullet.SetTarget(GetClosestEnemyTransform(position));
                        break;
                }
            }   
        }
    }

    /*
     * Get Closest enemy transform in respect to the given position.
     */
    private Transform GetClosestEnemyTransform(Vector3 position)
    {
        // GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<EnemyBehaviour> enemies = new List<EnemyBehaviour>(FindObjectsOfType<EnemyBehaviour>());

        if (enemies.Count > 0)
        {
            //get an initial value
            float shortestDistance = Vector3.Distance(position, enemies[0].transform.position);
            int indexOfClosestEnemy = 0;

            for (int i = 1; i < enemies.Count; i++)
            {
                float distance = Vector3.Distance(position, enemies[i].transform.position);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    indexOfClosestEnemy = i;
                }
            }

            return enemies[indexOfClosestEnemy].transform;
        }

        return null;
    }

    private IEnumerator FireCoolDown()
    {
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}

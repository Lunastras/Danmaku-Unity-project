using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemyEmitter : MonoBehaviour
{
    //All of these arrays have the same length
    //the index represents the respective bullet type

    public float initialDelay = 1.0f;
    public float delayBetweenRounds = 0.5f;

    //MUST be initialized
    public GameObject[] bulletsToSpawn;

    //Optional
    [Header("Bullets Spawning Behaviour")]
    public int[] numOfBulletsToSpawn;
    public float[] fireRate;
    public BulletTypes[] bulletTypes; //Aiming and homing are the same for enemies
    public float[] rotationSpeedCoef;
    public int[] timesToRepeatStep; //how many times the current bullet pattern will repeat.

    [Header("Spawning Rotation Controls")]
    //variables for controlling rotation after each shot
    public float rotationSpeed = 0;
    public float rotationAcceleration = 0;
    public float maxAbsRotationSpeed = 5;
    public bool restartRotationForReps = false;

    private float currentZRotationOffset = 0;
    private Transform player;
    private AudioManager audio;


    [Header("Bullet fields overrides (optional)")]
    public float bulletSpeed = -1;
    public float bulletAcceleration = 0;
    public float zRotationSpeed = 0;
    public float zRotationAcceleration = 0;
    public float rotationTime = -1;
    public float minSpeed = -1;
    public float maxSpeed = -1;


    [Header("Tutorial values")]
    public bool waitsForSignal = false;
    private bool restartPhase = false;
    private bool signaled = false;
    private TutorialManager tutorialManager = null;


    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;

        //instantiate the arrays if they are null
        if (numOfBulletsToSpawn.Length == 0)
        {
            numOfBulletsToSpawn = new int[1] { 1 };
        }

        if (fireRate.Length == 0)
        {
            fireRate = new float[1] { 0 };
        }

        if (rotationSpeedCoef.Length == 0)
        {
            rotationSpeedCoef = new float[1] { 1 };
        }

        if (timesToRepeatStep.Length == 0)
        {
            timesToRepeatStep = new int[1] { 1 };
        }

        if (bulletTypes.Length == 0)
        {
            bulletTypes = new BulletTypes[1] { BulletTypes.Normal };
        }

        if(GameManager.gameManager.isTutorial)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }
        audio = AudioManager.instance;
        StartCoroutine(StartFiring());
    }

    private IEnumerator StartFiring()
    {
        yield return new WaitForSeconds(initialDelay);

        for(int i = 0; i < bulletsToSpawn.Length; i++)
        {
            //skip the signal if the round restarted
            if(!restartPhase)
            {
                if (waitsForSignal)
                {
                    yield return new WaitUntil(() => signaled);
                    signaled = false;
                }
            }

            restartPhase = false;

            for (int x = 0; x < timesToRepeatStep[i % timesToRepeatStep.Length]; x++)
            {
                if (restartPhase)
                    break;

                for (int j = 0; j < numOfBulletsToSpawn[i % numOfBulletsToSpawn.Length]; j++)
                {
                    if (restartPhase)
                        break;

                    audio.Play("enemyShoot");
                    GameObject bullet = Instantiate(bulletsToSpawn[i]);
                    bullet.layer = 13; //enemy bullet layer
                    bullet.transform.position = transform.position;

                    BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
                    SetBulletParameters(bulletBehaviour);

                    float zOffset = currentZRotationOffset * rotationSpeedCoef[i % rotationSpeedCoef.Length];

                    switch (bulletTypes[i % bulletTypes.Length])
                    {
                        case BulletTypes.Normal:
                            float zRotation = transform.rotation.eulerAngles.z + zOffset;
                            bulletBehaviour.SetZRotation(zRotation);
                            break;

                        //homing and aiming
                        //homing cannot be used by enemies because it would be broken
                        default:
                            //change offset with respect to the position of the player
                            float offsetOrientation = Mathf.Sign(player.position.x - transform.position.x);
                            Vector3 targetPos = player.position;
                            Vector3 directionToTarget = (targetPos - transform.position).normalized;
                            bulletBehaviour.SetZRotation(Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg + zOffset * offsetOrientation);
                            break;
                    }

                    currentZRotationOffset += rotationSpeed;
                    rotationSpeed += rotationAcceleration;
                    rotationSpeed = Mathf.Sign(rotationSpeed) * Mathf.Min(Mathf.Abs(rotationSpeed), maxAbsRotationSpeed);

                    if (fireRate[i % fireRate.Length] > 0.0003)
                    {
                        yield return new WaitForSeconds(fireRate[i % fireRate.Length]);
                    }
                }

                yield return new WaitForSeconds(delayBetweenRounds);

                //resets after every bullet type
                if(restartRotationForReps)
                    currentZRotationOffset = 0; 

                if(rotationAcceleration != 0)
                {
                    rotationSpeed = 0;
                }
            }

            //tutorial 
            if(restartPhase)
            {
                //phase failed, repeat it
                i--;
                signaled = true;
                yield return new WaitForSeconds(2);
            }else if (tutorialManager != null) //phase passed
            {
                yield return new WaitForSeconds(3);
                signaled = false;
                tutorialManager.PhaseFinished();
                restartPhase = false;
            }

            currentZRotationOffset = 0;
        }

        Destroy(this);
    }

    public void Signal()
    {
        signaled = true;
    }

    public void RestartPhase()
    {
        Debug.Log("ahh player DEIED Emitter");
        restartPhase = true;
    }

    /**
     * Overrides the values in the prefab if they are not default
     */
    private void SetBulletParameters(BulletBehaviour bullet)
    {
        if (bulletSpeed > -0.9) bullet.speed = bulletSpeed;
        if (rotationTime > -0.9) bullet.rotationTime = rotationTime;
        if (maxSpeed > -0.9) bullet.maxSpeed = maxSpeed;
        if (minSpeed > -0.9) bullet.speed = bulletSpeed;
        if (Mathf.Abs(bulletAcceleration) > 0.001) bullet.speedAcceleration = bulletAcceleration;
        if (Mathf.Abs(zRotationSpeed) > 0.001) bullet.zRotationSpeed = zRotationSpeed;
        if (Mathf.Abs(zRotationAcceleration) > 0.001) bullet.zRotationAcceleration = zRotationAcceleration;
    }
}

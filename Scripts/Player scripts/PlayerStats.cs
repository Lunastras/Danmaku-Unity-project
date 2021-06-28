using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public int numOfLives = 3; //default
    public float weaponPoints = 0; //current weapon score.

    public GameObject[] weaponTiers; //the gameobjects containing the firing origins

    //THIS LIST HAS weaponTiers.length - 1 ELEMENTS, as we don't need to know the threshold for the first level
    public float[] weaponTierRequiredPoints; //list of required weaponPoints for given tier
    

    private int weaponLevel = 0; //first level
    private AudioManager audio;
    private GameManager gameManager;
    public bool isInvulnerable = false;

    public Text weaponScoreText;
    public Text livesText;
    public Text scoreText;
    public GameObject playerDeathParticles = null;


    public int score = 0;

    public TutorialManager tutorialManager = null;



    void Start()
    {
        //activate current turret
        for(int i = 0; i < weaponTiers.Length; i++)
        {
            weaponTiers[i].SetActive(i == weaponLevel);
        }

        UpdateGui();
        gameManager = GameManager.gameManager;

        if(gameManager.isTutorial)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }

        UpdateGui();

        audio = AudioManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Kill()
    { 
        if (isInvulnerable)
            return;

        numOfLives--;
        audio.Play("playerDeath");

        GameObject particle = Instantiate(playerDeathParticles);
        particle.transform.position = transform.position;

        if(GameManager.gameManager.isTutorial)
        {
            if(tutorialManager == null)
            {
                tutorialManager = FindObjectOfType<TutorialManager>();
            }
            tutorialManager.RestartPhase();
        }

        if (numOfLives < 0 && !GameManager.gameManager.isTutorial)
        {
            gameManager.GameOver();
        } else
        {
            gameManager.PlayerRespawn();
            UpdateGui();
        } 
    }

    public void AddScorePoints(int points)
    {
        score += points;
        UpdateGui();
    }

    public void AddWeaponPoints(float val)
    {
        weaponPoints += val;
        audio.Play("graze");

        //check if we are on the final tier already.
        if (weaponLevel < weaponTierRequiredPoints.Length)
        {
            //weaponTierRequiredPoints[weaponLevel] are the points required for the next level
            if (weaponPoints >= weaponTierRequiredPoints[weaponLevel])
            {
                audio.Play("powerup");

                //disable old turret, activate new one.
                weaponTiers[weaponLevel].SetActive(false);
                weaponTiers[++weaponLevel].SetActive(true);
            }

        }

        UpdateGui();
    }

    private void UpdateGui()
    {
        //bool ebola = gameManager.isTutorial;
        livesText.text = GameManager.gameManager.isTutorial ? "Infinite" : numOfLives.ToString();
        scoreText.text = score.ToString();

        weaponScoreText.text = ((weaponLevel == weaponTierRequiredPoints.Length ? "FULL" : (weaponPoints.ToString() + '/' + weaponTierRequiredPoints[weaponLevel])));
    }
}

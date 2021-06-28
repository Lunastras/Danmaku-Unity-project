using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //current instance
    public static GameManager gameManager;

    private GameObject player;
    private PlayerController playerController;
    private GameObject respawnInvulnerability;

    public float respawnTime = 1.5f;
    public float respawnSpeed = 5;

    private bool respawningPlayer = false;
    private List<GameObject> activeEnemies;

    private bool isPaused;
    private bool pauseButtonReleased = true;
    private bool gameOver = false;
    private bool isRespawning = false;

    //whether or not the game is a playback
    private bool isPlayBack;
    public List<InputForFrame> inputs;
    private int currentInputIndex = 0;

    public Vector2 input = Vector2.zero;
    public bool isFiring = false;
    public bool isCrouching = false;

    //for use of the tutorial only
    public bool canMoveTutorial = true;
    public bool canFireTutorial = true;
    public bool canCrouchTutorial = true;
    public bool isTutorial = false;


    public GameObject pauseScreen;
    public GameObject gameOverScreen;
    public GameObject overlayUI;
    public Image imageFade;
    public GameObject replayScreen;
    private string profileName;




    //A list of randomly ordered numbers from 0 to 99
    //used for rng in order to make sure we have the same seeds and outcomes
    private static int currentRandomIndex = 0;
    private static int[] shuffledList = { 18, 83, 27, 78, 11, 70, 68, 19,
                                          33, 56, 82, 13, 53, 49, 40, 54,
                                          8, 9, 67, 2, 73, 91, 50, 24, 37,
                                          52, 99, 4, 92, 45, 47, 62, 93, 66,
                                          20, 76, 71, 7, 31, 51, 97, 10, 42,
                                          48, 0, 38, 5, 16, 94, 1, 25, 39, 58,
                                          36, 86, 6, 95, 15, 84, 22, 79, 21, 80,
                                          44, 88, 65, 69, 81, 74, 63, 3, 29, 57,
                                          77, 85, 26, 23, 64, 60, 46, 90, 28, 96,
                                          98, 32, 12, 59, 55, 87, 89, 43, 30, 72,
                                          14, 17, 61, 41, 75, 34, 35 };


    // Start is called before the first frame update
    void Awake()
    {
        gameManager = this;

        activeEnemies = new List<GameObject>();

        //if it's null, we assume it is a normal play done in 
        //editor mode
        if(GameSetupData.instance == null)
        {
            GameSetupData.instance = new GameSetupData();
            inputs = new List<InputForFrame>();
            isPlayBack = false;
            currentRandomIndex = 0;
        } else
        {
            inputs = GameSetupData.instance.inputs;
            currentRandomIndex = GameSetupData.instance.initialRandIndex;
            isPlayBack = GameSetupData.instance.isPlayBack;
        }

        Cursor.visible = false;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().gameObject;
        respawnInvulnerability = GameObject.Find("Respawn Invulnerability");
        respawnInvulnerability.SetActive(false);
        playerController = player.GetComponent<PlayerController>();

        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        replayScreen.SetActive(false);
  

        if(!LoadingScreenManager.currentlyLoading)
        {
            overlayUI.SetActive(false);
            imageFade.gameObject.SetActive(false);
        }
        
        StartCoroutine(GameStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
            return;

        if (Input.GetAxisRaw("Pause") > 0.1f)
        {
            if(pauseButtonReleased)
            {
                pauseButtonReleased = false;
                PauseToggle();
            }
        } else
        {
            pauseButtonReleased = true;
        }

        if(respawningPlayer)
        {
            float currentY = player.transform.position.y + respawnSpeed * Time.deltaTime;

            if (currentY > -4)
            {
                RespawnFinished();
                currentY = -4;
            }

            player.transform.position = new Vector3(player.transform.position.x, currentY);
        }   
    }

    private void FixedUpdate()
    {
        if(playerController.canMove)
        {
            InputForFrame inputRaw;

            if (isPlayBack) //use playback inputs
            {
                inputRaw = gameManager.GetCurrentInput();
                input = new Vector2(inputRaw.x, inputRaw.y);
                isCrouching = inputRaw.isCrouching;
                isFiring = inputRaw.isFiring;
            }
            else //take player input
            {
                input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));               
                isCrouching = (Input.GetAxisRaw("Crouch") > 0.1f) && canCrouchTutorial;
                isFiring = (Input.GetAxis("Fire1") > 0.1f) && canFireTutorial;

                inputRaw.x = input.x;
                inputRaw.y = input.y;
                inputRaw.isFiring = isFiring;
                inputRaw.isCrouching = isCrouching;

                inputs.Add(inputRaw);
            }
        }    
    }

    /**
     * Get random int from 0 to 99
     */
    public static int GetRandomInt()
    {
        int numToReturn = shuffledList[currentRandomIndex];

        currentRandomIndex++;
        currentRandomIndex %= shuffledList.Length;

        return numToReturn;  
    }

    public void PlayerRespawn()
    {
        if (gameOver || isRespawning)
            return;

        isRespawning = true;
        player.GetComponent<CircleCollider2D>().enabled = false;
        player.GetComponent<SpriteRenderer>().enabled = false;
        playerController.canMove = false;
        player.GetComponent<PlayerStats>().isInvulnerable = true;
        playerController.SetInput(Vector3.zero);

        StartCoroutine(PlayerRespawnCoroutine());
    }

    private IEnumerator PlayerRespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime / 2);
        player.transform.position = new Vector3(-2.0025f, -6);
        respawnInvulnerability.SetActive(true);
        player.GetComponent<SpriteRenderer>().enabled = true;
        playerController.SetInput(new Vector3(0, 1));

        player.GetComponent<ColourBlinks>().BlinkForGivenDuration(respawnTime / 2.6f + respawnTime / 2);
        yield return new WaitForSeconds(respawnTime / 2.6f);

        playerController.canMove = true;
        player.GetComponent<CircleCollider2D>().enabled = true;

        
        yield return new WaitForSeconds(respawnTime / 2);
        respawnInvulnerability.SetActive(false);
        player.GetComponent<PlayerStats>().isInvulnerable = false;
        isRespawning = false;
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(0);
        RandomEnemySpawner[] spawners = FindObjectsOfType<RandomEnemySpawner>();
        playerController.canMove = true;

        foreach(RandomEnemySpawner spawner in spawners)
        {
            spawner.isActive = true;
        }
    }

   

    public void Quit()
    {
        gameOver = true;
        pauseScreen.SetActive(false);
        overlayUI.SetActive(true);
        imageFade.gameObject.SetActive(true);
        Cursor.visible = false;
        StartCoroutine(QuitRoutine());
    }

    private IEnumerator QuitRoutine()
    {
        
        Color fixedColor = imageFade.color;
        fixedColor.a = 1;
        imageFade.color = fixedColor;

        imageFade.CrossFadeAlpha(0, 0, true);
        imageFade.CrossFadeAlpha(1, 1, true);
        Time.timeScale = 1;
        yield return new WaitForSeconds(1);     
        SceneManager.LoadScene(0);
    }

    public void PauseToggle()
    {
        if (gameOver || LoadingScreenManager.currentlyLoading)
            return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0.0000000000000000000001f : 1;  
        pauseScreen.SetActive(isPaused);
        overlayUI.SetActive(isPaused);
        imageFade.gameObject.SetActive(false);
        Cursor.visible = isPaused;
    }

    public void replayToggle()
    {
        replayScreen.SetActive(!replayScreen.activeSelf);
        gameOverScreen.SetActive(!gameOverScreen.activeSelf);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public bool IsPlayBack()
    {
        return isPlayBack;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void SaveReplay()
    {
        //check if the values are valid
        if(GameSetupData.instance.initialRandIndex >= 0 &&
            GameSetupData.instance.initialRandIndex <= 99)
        {
            ReplayProfile profile;
            profile.initialRandIndex = GameSetupData.instance.initialRandIndex;
            profile.inputs = inputs;
            profile.score = player.GetComponent<PlayerStats>().score;
            profile.name = profileName;

            SaveData saveData = GameData.gameData.saveData;

            if(saveData.profiles == null)
            {
                saveData.profiles = new List<ReplayProfile>();
            }

            saveData.profiles.Add(profile);

            GameData.gameData.Save();
        }

        Quit();
        //Debug.Log("Saved profile with name " + profile.name);
    }

    public void SaveProfileName(string s)
    {
        Debug.Log("UPDATED:" + s);
        profileName = s;
    }


    public InputForFrame GetCurrentInput()
    {
        currentInputIndex = Mathf.Min(currentInputIndex, inputs.Count - 1);
        return inputs[currentInputIndex++];
    }


    private void RespawnFinished()
    {
        respawningPlayer = false;
    }

    public void GameOver()
    {
        if (gameOver)
            return;

        gameOver = true;
        player.transform.position = new Vector3(-2.0025f, -30);

        
        StartCoroutine(GameOverRoutine());
        
    }

    private IEnumerator GameOverRoutine()
    {
        RandomEnemySpawner[] spawners = FindObjectsOfType<RandomEnemySpawner>();
        // playerController.canMove = false;

        foreach (RandomEnemySpawner spawner in spawners)
        {
            spawner.isActive = false;
        }

        yield return new WaitForSeconds(1);


        if (isPlayBack)
        {
            gameOverScreen.SetActive(false);
            Quit();
        }
        else
        {
            Cursor.visible = true;
            overlayUI.SetActive(true);
            gameOverScreen.SetActive(true);
            imageFade.gameObject.SetActive(false);
        }
    }

    private IEnumerator RetryRoutine()
    {
        imageFade.gameObject.SetActive(true);
        Color fixedColor = imageFade.color;
        fixedColor.a = 1;
        imageFade.color = fixedColor;

        imageFade.CrossFadeAlpha(0, 0, true);
        imageFade.CrossFadeAlpha(1, 1, true);

        GameSetupData gameSetupData = GameSetupData.instance;
        gameSetupData.initialRandIndex = Mathf.RoundToInt(Random.Range(0, 99));
        gameSetupData.isPlayBack = false;
        gameSetupData.inputs = new List<InputForFrame>();

        Time.timeScale = 1;
        yield return new WaitForSeconds(1);
        LoadingScreenManager.LoadScene(3);
    }

    public void Retry()
    {
        StartCoroutine(RetryRoutine());
    }


    public void EnemySpawned(GameObject enemy)
    {
        activeEnemies.Add(enemy);
    }

    public void EnemyDestroyed(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    public List<GameObject> GetActiveEnemies()
    {
        return activeEnemies;
    }
}

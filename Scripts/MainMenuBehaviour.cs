using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainMenuBehaviour : MonoBehaviour
{
    public int firstLevelIndex = 4;
    public Image fadeOverlay;
    public float fadeDelay = 1.0f;
    public GameObject mainScreen;
    public GameObject replayScreen;

    public GameObject replaysParent;
    public GameObject replayPrefab;
    private Vector3 initialReplayParentPos;
    private float replayYoffset = 0;
    private float scrollSpeed = 500f;

    private SaveData _save;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        _save = GameData.gameData.saveData;
        Color fixedColor = fadeOverlay.color;
        fixedColor.a = 1;
        fadeOverlay.color = fixedColor;
        fadeOverlay.CrossFadeAlpha(1f, 0f, true);
        ListReplays();

        initialReplayParentPos = replaysParent.transform.position;

        replayScreen.SetActive(false);
        mainScreen.SetActive(true);

        StartCoroutine(StartMenu()); 
    }

    public void ListReplays()
    {
        replayYoffset = 0;

        for (int i = 0; i < replaysParent.transform.childCount; i++)
        {
            Destroy(replaysParent.transform.GetChild(i).gameObject);
        }

        List<ReplayProfile> profiles = _save.profiles;

        for(int i = 0; i < profiles.Count; i++)
        {
            GameObject tab = Instantiate(replayPrefab, replaysParent.transform);
            float tabHeight = tab.GetComponent<RectTransform>().rect.height;
            float newPosY = 540 - ((i) * (tabHeight) + tabHeight / 2.0f);

            Debug.Log(newPosY);

            tab.transform.localPosition = new Vector3(0, newPosY);
            tab.GetComponent<ReplayBehaviour>().SetReplayProfile(i);
        }

    }

    private IEnumerator StartMenu()
    {
        yield return new WaitForSeconds(fadeDelay);

        fadeOverlay.CrossFadeAlpha(0f, fadeDelay, true);

        yield return new WaitForSeconds(fadeDelay);

        fadeOverlay.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (replayScreen.activeSelf)
        {
            float mouseWheelInput = Input.GetAxisRaw("Mouse ScrollWheel");
            Debug.Log(mouseWheelInput);
            replayYoffset -= mouseWheelInput * scrollSpeed;

            float scrollUpperLimit = replayPrefab.GetComponent<RectTransform>().rect.height
                * GameData.gameData.saveData.profiles.Count
                - replaysParent.GetComponent<RectTransform>().rect.height;

            
            replayYoffset = Mathf.Min(replayYoffset, scrollUpperLimit);
            replayYoffset = Mathf.Max(replayYoffset, 0);

            Vector3 newPosition = initialReplayParentPos;
            newPosition.y += replayYoffset;
            Debug.Log(newPosition);

            replaysParent.transform.position = newPosition;
        }
    }

    public void SwitchScreen()
    {
        AudioManager.instance.Play("Selection");
        mainScreen.SetActive(!mainScreen.activeSelf);
        replayScreen.SetActive(!replayScreen.activeSelf);
    }

    public void StartGame()
    {
        GameSetupData gameSetupData = new GameSetupData();
        gameSetupData.initialRandIndex = Mathf.RoundToInt(Random.Range(0, 99));
        gameSetupData.isPlayBack = false;
        gameSetupData.inputs = new List<InputForFrame>();
        gameSetupData.isTutorial = false;
        GameSetupData.instance = gameSetupData;

        StartCoroutine(ChoiceMade(3));
    }

    public void StartReplay(int replayIndex)
    {
        List<ReplayProfile> profiles = GameData.gameData.saveData.profiles;
        
        if (replayIndex <= profiles.Count)
        {
            ReplayProfile profile = GameData.gameData.saveData.profiles[replayIndex];

            GameSetupData gameSetupData = new GameSetupData(); ;
            gameSetupData.initialRandIndex = profile.initialRandIndex;
            gameSetupData.isPlayBack = true;
            gameSetupData.inputs = profile.inputs;
            gameSetupData.isTutorial = false;

            GameSetupData.instance = gameSetupData;

            StartCoroutine(ChoiceMade(3));
        }
    }

    public void Tutorial()
    {
        GameSetupData gameSetupData = new GameSetupData();
        gameSetupData.initialRandIndex = Mathf.RoundToInt(Random.Range(0, 99));
        gameSetupData.isPlayBack = false;
        gameSetupData.inputs = new List<InputForFrame>();
        gameSetupData.isTutorial = true;
        GameSetupData.instance = gameSetupData;

        StartCoroutine(ChoiceMade(4));
    }


    private IEnumerator ChoiceMade(int levelIndex)
    {
        AudioManager.instance.Play("Selection");

        fadeOverlay.gameObject.SetActive(true);

        Color fixedColor = fadeOverlay.color;
        fixedColor.a = 1;
        fadeOverlay.color = fixedColor;
        fadeOverlay.CrossFadeAlpha(0f, 0f, true);

        fadeOverlay.CrossFadeAlpha(1f, fadeDelay, true);

        yield return new WaitForSeconds(fadeDelay);

        LoadingScreenManager.LoadScene(levelIndex);             
    }

    public void QuitButton()
    {
        Application.Quit();
    }
}

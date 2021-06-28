using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private GameManager gameManager;
    public BulletEnemyEmitter emitter;

    public string[] dialogueList;
    private int currentDialogueLine = 0;

    public Text dialogueText;
    public GameObject dialogueScreen;

    private bool canProgress = false;
    private bool phaseFinished = false;
    


    // Update is called once per frame
    void Start()
    {
        gameManager = GameManager.gameManager;

        //this should be true from the beginning 
        //but just to make sure, the manager will also 
        //set the value to true
        gameManager.isTutorial = true;
        dialogueScreen.SetActive(false);

        gameManager.canCrouchTutorial = false;
        gameManager.canFireTutorial = false;
        gameManager.canMoveTutorial = false;
        StartCoroutine(StartTutorial());
    }

    private IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(1);

        //****phase 1*****
        gameManager.canMoveTutorial = true;

        int phaseLines = 3;
        dialogueScreen.SetActive(true);
        Cursor.visible = true;
        for (int i = currentDialogueLine; i < currentDialogueLine + phaseLines; i++)
        {
            dialogueText.text = dialogueList[i];
            yield return new WaitUntil(() => canProgress);
            canProgress = false;
        }
        dialogueScreen.SetActive(false);
        Cursor.visible = false;
        currentDialogueLine += phaseLines;



        yield return new WaitForSeconds(1);

        emitter.Signal();

        phaseFinished = false;
        yield return new WaitUntil(() => phaseFinished);
        phaseFinished = false;

        yield return new WaitForSeconds(2);


        //****phase 2****

        gameManager.canCrouchTutorial = true;

        phaseLines = 3;
        dialogueScreen.SetActive(true);
        Cursor.visible = true;
        for (int i = currentDialogueLine; i < currentDialogueLine + phaseLines; i++)
        {
            dialogueText.text = dialogueList[i];
            yield return new WaitUntil(() => canProgress);
            canProgress = false;
        }
        dialogueScreen.SetActive(false);
        Cursor.visible = false;
        currentDialogueLine += phaseLines;



        yield return new WaitForSeconds(1);

        emitter.Signal();

        phaseFinished = false;
        yield return new WaitUntil(() => phaseFinished);
        phaseFinished = false;

        yield return new WaitForSeconds(2);

        //****phase 3****

        gameManager.canFireTutorial = true;

        phaseLines = 3;
        dialogueScreen.SetActive(true);
        Cursor.visible = true;
        for (int i = currentDialogueLine; i < currentDialogueLine + phaseLines; i++)
        {
            dialogueText.text = dialogueList[i];
            yield return new WaitUntil(() => canProgress);
            canProgress = false;
        }
        dialogueScreen.SetActive(false);
        Cursor.visible = false;
        currentDialogueLine += phaseLines;


        yield return new WaitForSeconds(1);
        emitter.gameObject.layer = 10;

        emitter.Signal();

        phaseFinished = false;
        yield return new WaitUntil(() => phaseFinished);
        phaseFinished = false;

        yield return new WaitForSeconds(2);

        
        gameManager.Quit();
    }



    public void RestartPhase()
    {
        Debug.Log("ahh player DEIED tutorial");
        
        emitter.gameObject.GetComponent<EnemyBehaviour>().health = 100;
        emitter.RestartPhase();
    }

    public void PhaseFinished()
    {
        phaseFinished = true;
    }





    public void ProgressButtonPressed()
    {
        Debug.Log("Butonas presat");
        canProgress = true;
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourBlinks : MonoBehaviour
{
    public Color blinkingColour;
    public float blinkTime = 0.1f;
    public float timeBetweenBlinks = 0.1f;

    private Color originalColor;
    private SpriteRenderer sprt;
    private bool isBlinking = false;


    void Start()
    {
        sprt = GetComponent<SpriteRenderer>();
        originalColor = sprt.color;
    }

    /**
     * Blink n times, where n is the argument
     */
    public void BlinkNTimes(int n)
    {
        StartCoroutine(NTimesBlink(n));
    }

    /**
     * Blink for a given number of seconds.
     */
    public void BlinkForGivenDuration(float time)
    {
        isBlinking = true;
        StartCoroutine(BlinkingDurationTimeout(time));
        StartCoroutine(BlinkingDurationCode());
    }

    /**
     * Start making the sprite blink for a given number of seconds
     */
    private IEnumerator BlinkingDurationTimeout(float time)
    { 
        yield return new WaitForSeconds(time);
        isBlinking = false;
    }

    /**
    * Start making the sprite blink for a given number of seconds
    */
    private IEnumerator BlinkingDurationCode()
    {
       while(isBlinking)
        {
            sprt.color = blinkingColour;
            yield return new WaitForSeconds(blinkTime);
            sprt.color = originalColor;
            yield return new WaitForSeconds(timeBetweenBlinks);
        }

        sprt.color = originalColor;
    }

    /**
    * Start making the sprite blink for a given number of seconds
    */
    private IEnumerator WaitFor(float time)
    {
        isBlinking = true;
        yield return new WaitForSeconds(time);
        isBlinking = false;
    }

    private IEnumerator NTimesBlink(int n)
    {
        for(int i = 0; i < n; i++)
        {
            sprt.color = blinkingColour;
            yield return new WaitForSeconds(blinkTime);
            sprt.color = originalColor;
            yield return new WaitForSeconds(timeBetweenBlinks);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalSpeed = 10f;
    public float crawlSpeedCoef = 0.4f; //value between 0 and 1

    public float speedSmoothTime = 0.1f;
    public bool canMove = false;

    private Vector2 speedSmoothVelocity;
    private Vector2 currentSpeed;
    private Rigidbody2D rb;
    private GameManager gameManager;
    private Vector2 input = Vector2.zero;

    private bool isCrouching;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameManager.gameManager;
    }


    public void SetInput(Vector3 input)
    {
        this.input = input;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            input = gameManager.input;
            isCrouching = gameManager.isCrouching;  
        }

        Vector2 inputDir = input.normalized;

        Vector2 targetSpeed = normalSpeed * inputDir;
        currentSpeed = Vector2.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        rb.velocity = currentSpeed * (isCrouching ? crawlSpeedCoef : 1);
    }
}

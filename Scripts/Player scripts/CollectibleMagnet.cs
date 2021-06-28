using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleMagnet : MonoBehaviour
{
    public float speed = 15;

    private bool isFalling = false;

    //When spawned move shortly in a random direction
    private Vector2 initialDirection;    

    private Transform target = null;

    private void Start()
    {
        float initialSpawnRotation = ((float)GameManager.GetRandomInt() / 99.0f * 359 ) * Mathf.Deg2Rad;
        initialDirection = new Vector2(Mathf.Cos(initialSpawnRotation), Mathf.Sin(initialSpawnRotation));

        StartCoroutine(InitialMovementDuration(Mathf.Lerp(0.08f, 0.2f, (float)GameManager.GetRandomInt() / 99.0f)));
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movementDir = Vector3.zero;
     
        if (target != null)
        {
            movementDir = (target.position - transform.position).normalized;

        } else if(isFalling)
        {
            movementDir = new Vector3(0, -1, 0) / 11.0f;
        } else
        {
            movementDir = initialDirection / 3f;
        }

        transform.position += movementDir * speed * Time.deltaTime;
    }

    private IEnumerator InitialMovementDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        isFalling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //make sure item gets magnetic only for the player and not the outer borders.
        if(collision.GetComponent<PlayerController>() != null)
        {
            target = collision.transform;
        }
    }
}

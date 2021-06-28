using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float speed = 5;
    public float zRotation = 90;

    //misc for special bullets
    public float zRotationSpeed = 0;
    public float speedAcceleration = 0;
    public float zRotationAcceleration = 0;

    //negative number means disabled
    //duration of the rotation of the bullet
    public float rotationTime = -1;

    public float minSpeed = 0;
    public float maxSpeed = 100;

    //Can be used to invert/lower/multiply movement/rotation
    public int rotationCoef = 1; 
    public int movementCoef = 1; 

    //if null, it will do its normal routine
    //if it has a value, it will approach it.
    private Transform target = null;
    private Vector3 movementDir;



    // Start is called before the first frame update
    void Start()
    {
        if (rotationTime > 0)
        {
            StartCoroutine(RotationStop());
        }

        //Destroy(gameObject, 20);
    }

    // Update is called once per frame
    void Update()
    {
        //we use small values to avoid floating point precision errors.
        if (speedAcceleration > 0.001f || speedAcceleration < -0.001f)
        {
            speed += speedAcceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        }

        if (target == null)
        {
            if (zRotationAcceleration > 0.0001f || zRotationAcceleration < -0.0001f)
            {
                zRotationSpeed += zRotationAcceleration * Time.deltaTime;
            }

            if (zRotationSpeed > 0.0001f || zRotationSpeed < -0.0001f)
            {
                zRotation += zRotationSpeed * rotationCoef * Time.deltaTime;
                zRotation %= 360;
                movementDir = new Vector3(Mathf.Cos(Mathf.Deg2Rad * zRotation), Mathf.Sin(Mathf.Deg2Rad * zRotation), 0);
            }
        }
        else
        {
            movementDir = target.position - transform.position;
        }

        transform.position += movementCoef * movementDir * speed * Time.deltaTime;
    }

    private IEnumerator RotationStop()
    {
        yield return new WaitForSeconds(rotationTime);

        zRotationSpeed = zRotationAcceleration = 0;
    }

    public void SetZRotation(float z)
    {
        zRotation = z % 360;
        movementDir = new Vector3(Mathf.Cos(Mathf.Deg2Rad * zRotation), Mathf.Sin(Mathf.Deg2Rad * zRotation), 0);
    }

    /**
     * Set the orientation of the projectiles
     */
    public void SetOrientation(int rotationSense, int movementSense)
    {
        this.rotationCoef = rotationSense;
        this.movementCoef = movementSense;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

}


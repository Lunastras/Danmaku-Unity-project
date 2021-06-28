using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleWeaponPoints : MonoBehaviour
{
    public float points = 0.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerStats player = collision.GetComponent<PlayerStats>();
        if (player != null)
        {
            player.AddWeaponPoints(points);
        }

        Destroy(transform.parent.gameObject);
    }
}

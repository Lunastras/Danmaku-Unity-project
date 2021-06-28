using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : ObjectMovement
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyBehaviour enemy = other.GetComponent<EnemyBehaviour>();

        if(enemy != null)
        {
            enemy.TakeDamage(damage);
        } else
        {
            PlayerStats player = other.GetComponent<PlayerStats>();

            if(player != null)
            {
                player.Kill();
            } 
        }

       Destroy(gameObject);
    }
}

/*
    * normal - no target
    * homing - projectile will follow closest target
    * aiming - projectile will be aimed at closest target, but it will not follow it 
    */
public enum BulletTypes
{
    Normal,
    Homing,
    Aiming
}

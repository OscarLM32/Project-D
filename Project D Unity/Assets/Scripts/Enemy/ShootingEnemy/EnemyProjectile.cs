using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Generic class of a projectile created by an enemy
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    public AudioClip destroyClip;
    
    //public GameObject particlesObject;
    public float speed;
    
    /// <summary>
    /// Sets up the direction and the position for a new pooled projectile
    /// </summary>
    /// <see cref="EnemyProjectilePool"/>
    /// <param name="direction"> Direction the projectile has to follow: up, down, right or left </param>
    /// <param name="position"> Position from which it is going to be shot</param>
    public void SetUpForShooting(Vector2 direction, Vector3 position)
    {
        RotateProjectile(direction); 
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        transform.position = position;
    }

    /// <summary>
    /// It rotates the projectile to face in the proper direction
    /// </summary>
    /// <param name="direction">The direction it is being shot</param>
    private void RotateProjectile(Vector2 direction)
    {
        transform.rotation = new Quaternion();

        if (direction == Vector2.up)
        {
            transform.Rotate(new Vector3(0,0,90));
        }
        else if (direction == Vector2.left)
        {
            transform.Rotate(new Vector3(0,0,180));
        }
        else if (direction == Vector2.down)
        {
            transform.Rotate(new Vector3(0,0,270));
        }
        
    }
    
    /// <summary>
    /// Checks if the collided object is killable and in that case it kills it.
    /// After that, plays the proper sound, instantiates particles and destroys the object from the projectiles pool 
    /// </summary>
    private void OnCollisionEnter2D(Collision2D col)
    {
        var player = col.transform.GetComponent<Player.Player>();
        if (player)
        {
            player.Kill();
        }
        //InstantiateParticles();
        AudioSource.PlayClipAtPoint(destroyClip, transform.position, 0.5f);
        EnemyProjectilePool.I.DeleteProjectile(gameObject); 
    }
}

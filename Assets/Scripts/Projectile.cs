using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#nullable enable

/// <summary>
/// Projectile core for bullets, lasers, healing orbs, etc.
/// Create a subclass of this script to create a new type of projectile
/// Feel free to refer to other subclasses for help when trying to add a new projectile
/// </summary>

public class Projectile : MonoBehaviour
{
    /*<----------------Stats---------------->*/
    [NonSerialized] public float SPD = 15; // projectile speed
    [NonSerialized] public string? TARGET; // target entity tag (Player/Enemy)
    [NonSerialized] public Entity? Caster; // caster of this projectile
    public float? LIFE; // how long this projectile lasts for
    /*<---------------Movement-------------->*/
    public Vector2 Position { get; private set; } // current projectile position
    public Vector2 Direction { get; private set; } // projectile facing & moving direction
    /*<----------------Config--------------->*/
    public bool DISABLE_DELETE { get; protected set; } = false; // set to true to prevent self delete
    public bool DISABLE_MOVE { get; protected set; } = false; // disable moving
    /*<-----------------Misc---------------->*/
    private float? DONT_DELETE = 0; // same as disable delete but to prevent the projectile from killing itself on spawn
    protected float elapsed = 0; // time elapsed since projectile creation
    protected GameManager Game = null!; // core GameManager

    /*<------------Init Functions----------->*/
    protected virtual void Start()
    {
        // init game variable and projectile's scene position
        Game = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        transform.position = Position;
    }
    protected virtual void Update()
    {
        // increment elapsed and call other update functions
        elapsed += Time.deltaTime;

        CheckLife();
        UpdatePosition();
    }
    /*<----------------Class Functions---------------->*/

    // !! Call this function to destroy the projectile instead of Destroy(this.gameObject) !!
    // Plays on Projectile destroyed
    protected virtual void Destroyed()
    {
        Destroy(this.gameObject);
    }
    // Plays when the object is hit
    protected virtual void OnHit(Entity entity)
    {
        Debug.Log($"Hit entity: {entity}");
    }

    /*<----------------Collision Functions---------------->*/

    // Check if the tag is the same as the target
    // and if the collision has an entity component
    // Then call the OnHit function once everything has been checked
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != TARGET) { return; }

        Entity? entity = collision.gameObject.GetComponent<Entity>();
        if (entity == null ) { return; }

        OnHit(entity);
    }

    /*<----------------Update Functions---------------->*/

    // Moves the projectile by the direction
    // Then set the object position to the new position
    private void UpdatePosition()
    {
        if (DISABLE_MOVE) { return; } // Disable config
        var delta = Time.deltaTime;
        var speed = SPD * 5 * delta;
        Position += Direction * speed;

        transform.position = Position;
    }

    // Checks if the object is past its expiration
    // or is beyond boundaries
    private void CheckLife()
    {
        if (DISABLE_DELETE) { return; } // Disable config

        // Checks if the object has elapsed past its lifetime
        if (LIFE != null && elapsed > LIFE)
        {
            Destroyed();
        }

        // Deletes object if object is past boundaries
        // DONT_DELETE variable prevents the object from being deleted past boundaries
        // DONT_DELETE becomes NULL when the object is within boundaries
        // If not, yield for 2 seconds in case, cause objects can be stuck outside of boundaries forever
        // --> Very messy, will not fix
        if (
            Mathf.Abs(Position.x) > _settings.Screen.x + transform.localScale.x ||
            Mathf.Abs(Position.y) > _settings.Screen.y + transform.localScale.y
        )
        {
            if (DONT_DELETE != null && DONT_DELETE < 2)
            {
                DONT_DELETE += Time.deltaTime;
            } else
            {
                Destroyed();
            }
        } else
        {
            DONT_DELETE = null;
        }
    }

    /*<----------------Projectile Functions---------------->*/

    // Moves the projectile towards the destination
    // --> Does not stop until leaving the boundaries
    public void MoveTo(Vector2 destination)
    {
        MoveTo(destination, Position);
    }

    // Moves the projectile towards the direction between the destination and the origin vector
    // --> Does not stop until leaving the boundaries
    public void MoveTo(Vector2 destination, Vector2 origin)
    {
        Vector2 diff = (destination - origin);
        diff.Normalize();
        Direction = diff;

        // set projectile direction
        float angle = (Mathf.Atan2(Direction.y, Direction.x)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90);
    }

    // Sets the projectile position to the specified position
    public void SetPosition(Vector2 position)
    {
        Position = position;
        transform.position = position;
    }

    // Changes the object speed over the specified time
    // --> For regular speed change, just set the speed cause adding a function for that is too much effort now that I've made so many bullet patterns
    // --> if this was a company then good luck to that one guy having to replace every .SPD = # line
    public Tween Accelerate(float change, float duration)
    {
        // proxy float for creating the delta value to change the speed
        float proxy = 0f;

        // tween by adding/substracting to the original SPD instead of setting
        return DOTween.To(
            () => 0f,
            x => { SPD += x - proxy; proxy = x; }, 1.0f, duration
        ).SetEase(Ease.OutQuad);
    }
}

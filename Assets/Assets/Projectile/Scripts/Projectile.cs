using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#nullable enable

public class Projectile : MonoBehaviour
{
    /* Init Variables */

    /*<----------------Stats---------------->*/
    [NonSerialized] public float SPD = 15;
    public float? LIFE; // How long this projectile lasts for
    [NonSerialized] public string? TARGET; // Target entity tag (Player/Enemy)
    /*<---------------Movement-------------->*/
    [NonSerialized] public Vector2 Position;
    [NonSerialized] public Entity? Caster;
    public Vector2 Direction { get; private set; }
    private float? DONT_DELETE = 0;
    /*<------------------------------------->*/
    protected float elapsed = 0;
    protected virtual void Start()
    {
        transform.position = Position;
    }
    protected virtual void Update()
    {
        elapsed += Time.deltaTime;

        CheckLife();
        UpdatePosition();
    }
    /* Class Functions */
    protected virtual void OnHit(Entity entity)
    {
        Debug.Log($"Hit entity: {entity}");
    }

    /* Collision Functions */
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the tag is the same as the target
        if (collision.gameObject.tag != TARGET) { return; }

        // Check if the collision has an entity component
        Entity? entity = collision.gameObject.GetComponent<Entity>();
        if (entity == null ) { return; };

        OnHit(entity);
    }

    /* Update Functions */
    private void UpdatePosition()
    {
        // Moves the projectile by the direction
        var delta = Time.deltaTime;
        var speed = SPD * 5 * delta;
        Position += Direction * speed;

        // Set the object position to the new position
        transform.position = Position;
    }
    private void CheckLife()
    {
        // Checks if the object has elapsed past its lifetime, deletes if true
        if (LIFE != null && elapsed > LIFE)
        {
            Destroy(this.gameObject);
        }

        // Deletes object if object is past boundaries
        // Only triggers when the object is within boundaries at least once
        // If not, just wait 2 seconds
        if (
            Mathf.Abs(Position.x) > _settings.Screen.x + transform.localScale.x ||
            Mathf.Abs(Position.y) > _settings.Screen.y + transform.localScale.y
        )
        {
            if (DONT_DELETE != null && DONT_DELETE < 2)
            {
                // Timeout for 2.0s
                DONT_DELETE += Time.deltaTime;
            } else
            {
                Destroy(this.gameObject);
            }
        } else
        {
            DONT_DELETE = null;
        }
    }

    /* Projectile Functions */
    public void MoveTo(Vector2 destination) // Moves the projectile in the direction of the destination
    {
        Vector2 diff = (destination - Position);
        diff.Normalize();
        Direction = diff;

        // set projectile direction
        float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}

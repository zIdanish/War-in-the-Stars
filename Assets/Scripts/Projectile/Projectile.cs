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
    [NonSerialized] public float DMG = 10;
    [NonSerialized] public float SPD = 15;
    public float? LIFE; // How long this projectile lasts for
    [NonSerialized] public string? TARGET; // Target entity tag (Player/Enemy)
    /*<---------------Movement-------------->*/
    [NonSerialized] public Vector2 Position;
    [NonSerialized] public Entity? Caster;
    [NonSerialized] public Vector2 Direction;
    /*<------------------------------------->*/
    private float elapsed = 0;
    private void Start()
    {
        transform.position = Position;
    }
    private void Update()
    {
        elapsed += Time.deltaTime;

        CheckLife();
        UpdatePosition();
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
    private void OnHit(Entity entity)
    {
        // Deals damage to the entity and destroys this object
        if (entity.Invulnerable) { return; }
        entity.Damage(DMG, Caster);

        Destroy(this.gameObject);
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
        if (
            Mathf.Abs(Position.x) > _settings.Boundaries.x + transform.localScale.x ||
            Mathf.Abs(Position.y) > _settings.Boundaries.y + transform.localScale.y
        )
        {
            Destroy(this.gameObject);
        }
    }

    /* Projectile Functions */
    public void MoveTo(Vector2 destination) // Moves the projectile in the direction of the destination
    {
        Vector2 diff = (destination - Position);
        diff.Normalize();
        Direction = diff;
    }
}

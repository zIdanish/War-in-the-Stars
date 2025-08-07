using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Placeholder : MonoBehaviour
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 1;
    public float ProjectileSpeed = 10;
    public float Lifetime = 4;
    public GameObject projectile;
    /*<-------------------------------------->*/

    /* Init Variables */
    private Vector2 BasePosition;
    private Entity entity;
    private void Start()
    {
        entity = GetComponent<Entity>();
        StartCoroutine(Timeline()); // Start the timeline
    }

    /*<----------------Timeline--------------->*/
    public IEnumerator Timeline() // Behaviour timeline
    {
        // Wait until the entity stops moving

        while (entity.Moving)
        {
            yield return null;
        }

        /*<-------------------------------------->*/
        // Start the Attack Behaviour

        Coroutine AttackBehaviour = StartCoroutine(Attack());

        /*<-------------------------------------->*/
        // Wait until the Entity's Lifetime expires

        yield return new WaitForSeconds(Lifetime);

        /*<-------------------------------------->*/
        // Stop the Attack Behaviour and start Exit Behaviour after cooldown
        StopCoroutine(AttackBehaviour);
        yield return new WaitForSeconds(Cooldown);

        StartCoroutine(Exit());
    }
    public IEnumerator Exit() // Behaviour on enemy exit
    {
        // Move past the boundary
        Vector2 ExitPosition = new Vector2(entity.Position.x, _settings.Boundaries.y + 20);
        entity.MoveExit(ExitPosition);

        /*<-------------------------------------->*/
        // Wait until the entity stops moving
        while (entity.Moving)
        {
            yield return null;
        }

        /*<-------------------------------------->*/
        // Deletes the entity
        entity.Die(entity);
    }
    public IEnumerator Attack() // Behaviour when attacking the player
    {
        // Get the entity's base Position
        BasePosition = entity.Position;

        // Loops the attack every cooldown
        while (true)
        {
            // Move randomly around the position
            entity.MoveRandom(BasePosition, 5.0f);

            /*<-------------------------------------->*/
            // Shoot at the player
            var player = Entity.getPlayer();
            if (player == null) { yield break; }

            var bullet = (PJ_Damage)entity.Shoot(projectile, ProjectileSpeed, player.Position);
            bullet.DMG = entity.DMG;

            yield return new WaitForSeconds(Cooldown);
        }
    }
}

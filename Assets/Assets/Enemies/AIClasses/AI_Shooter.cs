using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Shooter : AI
{
    /*<-----------------Stats---------------->*/
    public float Cooldown = 1;
    public float ProjectileSpeed = 10;
    public float Lifetime = 4;
    /* Init Variables */
    protected Vector2 BasePosition;

    /*<----------------Timeline--------------->*/
    protected override IEnumerator Timeline() // Behaviour timeline
    {
        // Wait until the entity stops moving

        while (entity.Moving)
        {
            yield return null;
        }

        /*<-------------------------------------->*/
        // Set the entity's base Position
        BasePosition = entity.Position;

        // Start the Attack Behaviour

        onAttack();
        Coroutine AttackBehaviour = Call(Pattern());

        /*<-------------------------------------->*/
        // Wait until the Entity's Lifetime expires

        yield return new WaitForSeconds(Lifetime);

        /*<-------------------------------------->*/
        // Stop the Attack Behaviour and start Exit Behaviour after cooldown
        End(AttackBehaviour);
        yield return new WaitForSeconds(Cooldown);

        Call(Exit());
    }
    protected virtual IEnumerator Attack()
    {
        Debug.Log("No attack pattern found for AI");
        yield return null;
    }
    protected virtual Vector2 exitPosition()
    {
        return new Vector2(entity.Position.x, _settings.Boundaries.y + 20);
    }
    protected virtual void onAttack()
    {
    }
    protected virtual void onExit()
    {
    }
    private IEnumerator Exit() // Behaviour on enemy exit
    {
        onExit();

        // Move past the boundary
        Vector2 ExitPosition = exitPosition();
        entity.MoveExit(ExitPosition);

        /*<-------------------------------------->*/
        // Wait until the entity stops moving
        yield return Call(WaitUntilStationary());

        /*<-------------------------------------->*/
        // Deletes the entity
        entity.Die(entity);
    }
    private IEnumerator Pattern() // Behaviour when attacking the player
    {
        // Loops the attack every cooldown
        while (true)
        {
            yield return StartCoroutine(Attack());
        }
    }
}

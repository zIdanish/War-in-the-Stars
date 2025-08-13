using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Goon3 : AI_Shooter
{
    /*<-----------------Stats---------------->*/
    public float SpinSpeed = 5;
    private bool Moving = true;

    /* Init Variables */
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override void onAttack()
    {
        StartCoroutine(SpinAndMove());
    }
    protected override void onExit()
    {
        Moving = false;
    }
    private IEnumerator SpinAndMove()
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        entity.MoveTo(player.Position);
        float angle = 0;

        while (true)
        {
            angle += Time.deltaTime * SpinSpeed * 360;
            entity.Look(angle);

            if (Moving)
            {
                entity.MoveTo(player.Position);
            }

            yield return null;
        }
    }
    /*private IEnumerator MoveToPlayer(Entity player)
    {
        var Position = player.Position;
        yield return new WaitForSeconds(FollowDelay);
        if (!Moving) { yield break; }

        entity.MoveTo(Position);
    }*/
    protected override Vector2 exitPosition()
    {
        Vector2 diff = entity.Destination - entity.Position;
        diff.Normalize();

        return entity.Position + (_settings.Height * 2 * diff);
    }
    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(Cooldown);
    }
}

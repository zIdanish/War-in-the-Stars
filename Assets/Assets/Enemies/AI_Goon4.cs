using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Goon4 : AI_Shooter
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile;

    /* Init Variables */
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override void onAttack()
    {
        var player = Entity.getPlayer();
        if (player == null) { return; }

        entity.Look(player.transform);
    }
    protected override void onExit()
    {
        var player = Entity.getPlayer();
        if (player == null) { return; }

        entity.Look(player.Position);

        // Bull charge
        var spd = entity.SPD;
        entity.Stat("SPD", -spd);
        entity.TweenStat("SPD", spd * 2, 2);
    }
    protected override Vector2 exitPosition()
    {
        var player = Entity.getPlayer();
        if (player == null) { return new Vector2(entity.Position.x, _settings.Boundaries.y + 20); }

        var direction = player.Position - entity.Position;
        direction.Normalize();
        return player.Position + _settings.Height * 2 * direction;
    }
    protected override IEnumerator Attack()
    {
        var player = Entity.getPlayer();
        if (player == null) { yield return null; yield break; }

        var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, player.Position, UnityEngine.Random.Range(-5f,5f));
        bullet.transform.localScale *= .75f;
        bullet.DMG = entity.DMG;

        yield return new WaitForSeconds(Cooldown);
    }
}

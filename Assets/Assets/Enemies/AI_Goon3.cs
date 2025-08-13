using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AI_Goon3 : AI_Shooter
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile;
    private Action Accelerate;

    /* Init Variables */
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override void onAttack()
    {
        Accelerate = 
            entity.Stat("SPD", -entity.Default["SPD"]*.5f); // Decelerate for precise movement
    }
    protected override void onExit()
    {
        Accelerate(); // Accelerate again for exit
    }
    protected override Vector2 exitPosition()
    {
        
        return new Vector2(
            entity.Position.x > _settings.Boundaries.x/2 ? _settings.Boundaries.x + 20 
            : -_settings.Boundaries.x - 20, 
            entity.Position.y
        );
    }
    protected override IEnumerator Attack()
    {
        yield return StartCoroutine(Pattern1());
        yield return new WaitForSeconds(Cooldown);
        yield return StartCoroutine(Pattern2());
        yield return new WaitForSeconds(Cooldown);
    }
    private IEnumerator Pattern1()
    {
        entity.MoveRandom(BasePosition + new Vector2(-7.5f, 0), 2.0f);

        for (int i = 0; i < 3; i++)
        {
            for (float angle = -30; angle <= 30; angle += 30)
            {
                if (angle == 0) { continue; }

                var x = angle < 0 ? -10 : 10;
                var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, angle);
                bullet.SetPosition(entity.Position + new Vector2(x, 0));
                bullet.DMG = entity.DMG;
            }

            yield return new WaitForSeconds(.25f);
        }
    }
    private IEnumerator Pattern2()
    {
        entity.MoveRandom(BasePosition + new Vector2(7.5f, 0), 2.0f);

        for (int i = 0; i < 3; i++)
        {
            var player = Entity.getPlayer();
            if (player == null) { yield break; }

            for (float x = -10; x <= 10; x += 10f)
            {
                if (x == 0) { continue; }
                var bullet = (PJ_Damage)entity.Shoot(Projectile, ProjectileSpeed, player.Position);
                bullet.SetPosition(entity.Position + new Vector2(x, 0));
                bullet.DMG = entity.DMG;
            }

            yield return new WaitForSeconds(.25f);
        }
    }
}

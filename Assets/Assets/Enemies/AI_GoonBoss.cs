using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
#nullable enable
public class AI_GoonBoss : AI
{
    /*<-----------------Stats---------------->*/
    public GameObject Projectile = null!;
    public GameObject Projectile_Laser = null!;
    /* Init Variables */
    protected Vector2 BasePosition;
    private void Start()
    {
        Init();
    }

    /*<----------------Timeline--------------->*/
    protected override IEnumerator Timeline() // Behaviour timeline
    {
        // Wait until the entity stops moving
        entity.SetInvulnerable(true);

        while (entity.Moving)
        {
            yield return null;
        }

        entity.SetInvulnerable(false);
        BasePosition = entity.Position;

        /*<-------------------------------------->*/
        // Start the Attack Behaviour

        Coroutine AttackBehaviour = Call(Intro());

        yield return AttackBehaviour;
    }
    private IEnumerator Intro() // Beginning Attack Pattern
    {
        // Opening Zigzags
        yield return StartCoroutine(ZigZags(25));

        yield return new WaitForSeconds(1);

        // Laser Barrage
        {
            // Laser Beam
            Laser(2, 2, 0);

            yield return new WaitForSeconds(2);

            // Spray
            yield return StartCoroutine(MultiSpray(3));

            // Laser Beams
            Laser(2, 2, -30);
            Laser(2, 2, 30);

            yield return new WaitForSeconds(2);

            // Spray
            yield return StartCoroutine(MultiSpray(3));
        }

        // Homing Laser
        var laser = Laser(3, 2, 0);
        if (laser != null)
        {
            StartCoroutine(LaserFollow(laser, 2));

            yield return new WaitForSeconds(2);

            // Spray
            yield return StartCoroutine(MultiSpray(7));

            while (!laser.IsDestroyed()) { yield return null; }
        }

        entity.Look();
    }

    /*<----------------Pattern Attacks--------------->*/
    private IEnumerator ZigZags(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var mult = Mathf.Abs(5 - i % 9);
            for (int x = -5 + i % 2; x <= 5; x += 2)
            {
                var angle = x * 15 + mult * 2;
                var bullet = (PJ_Damage)entity.Shoot(Projectile, 7.5f, angle);
                bullet.transform.localScale *= 1.25f;
                bullet.DMG = entity.DMG;
            }
            yield return new WaitForSeconds(.15f);
        }
    }
    private IEnumerator MultiSpray(int count)
    {
        entity.MoveRandom(BasePosition, 10);
        for (int i = 0; i < count; i++)
        {
            Spray((i % 2) * 17.5f);
            yield return new WaitForSeconds(.333f);
        }
    }
    /*<----------------Projectile Presets--------------->*/
    private void Spray(float angle)
    {
        for (float x = -90; x < 90; x += 17.5f) {
            var bullet = (PJ_Damage)entity.Shoot(Projectile, 12.5f, x + angle);
            bullet.transform.localScale *= 1.25f;
            bullet.DMG = entity.DMG;
        }
    }
    private PJ_Laser? Laser(float warn, float life, float angle)
    {
        var player = Entity.getPlayer();
        if (player == null) { return null; }
        entity.Look(player.transform.position);

        var laser = (PJ_Laser)entity.Shoot(Projectile_Laser, 0, player.transform.position, angle);
        laser.WARN = warn;
        laser.DURATION = life;
        laser.DMG = entity.DMG;
        laser.PIVOT = transform;

        return laser;
    }
    private IEnumerator LaserFollow(PJ_Laser laser, float duration)
    {
        var player = Entity.getPlayer();
        if (player == null) { yield break; }

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // refresh orientation
            entity.Look(player.transform.position);
            laser.MoveTo(player.transform.position, entity.Position);
            laser.RefreshPosition();

            yield return null;
        }
    }
}
